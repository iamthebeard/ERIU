using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class PlayerManager : CharacterManager
{
    [Header("Debug Menu")]
    [SerializeField] bool debugRespawnCharacter = false;
    [SerializeField] bool debugKillCharacter = false;
    [SerializeField] bool switchRightWeapon = false;
    [SerializeField] bool switchLeftWeapon = false;

    [HideInInspector] public PlayerLocomotionManager playerLocomotionManager;
    [HideInInspector] public PlayerAnimatorManager playerAnimatorManager;
    [HideInInspector] public PlayerNetworkManager playerNetworkManager;
    [HideInInspector] public PlayerStatsManager playerStatsManager;
    [HideInInspector] public PlayerInventoryManager playerInventoryManager;
    [HideInInspector] public PlayerEquipmentManager playerEquipmentManager;
    [HideInInspector] public PlayerCombatManager playerCombatManager;

    protected override void Awake()
    {
        base.Awake();

        // Do more, only for the player character
        playerLocomotionManager = GetComponent<PlayerLocomotionManager>(); // It's a component on the same object, so we can fetch it.
        playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
        playerNetworkManager = GetComponent<PlayerNetworkManager>();
        playerStatsManager = GetComponent<PlayerStatsManager>();
        playerInventoryManager = GetComponent<PlayerInventoryManager>();
        playerEquipmentManager = GetComponent<PlayerEquipmentManager>();
        playerCombatManager = GetComponent<PlayerCombatManager>();
    }

    protected override void Update()
    {
        base.Update();

        // If we are not the owner of this player, we do not control its movement
        if (IsOwner)
        {
            // Handle movement
            playerLocomotionManager.HandleAllMovement();
            // I added this in the "do it yourself" in episode 5
            // Handle animations
            playerAnimatorManager.UpdateAnimatorMovement(0, playerLocomotionManager.moveAmount, characterNetworkManager.isSprinting.Value);

            // Regen stamina
            playerStatsManager.RegenerateStamina();
        }
        // Handle movement during animations for *ALL* characters

        if (isRolling)
        { // I had to add this because my animation doesn't have built in motion
            // Keep moving in the direction we started rolling
            characterController.Move(transform.forward * playerLocomotionManager.rollingSpeed * Time.deltaTime);
            return;
        }
        if (isBackstepping)
        { // I had to add this because my animation doesn't have built in motion
            // Keep moving in the direction we started rolling
            characterController.Move((-transform.forward) * playerLocomotionManager.backstepSpeed * Time.deltaTime);
            return;
        }

        DebugMenu();
    }

    // In Unity, all camera actions should happen in LateUpdate
    protected override void LateUpdate()
    {
        if (!IsOwner) return;

        base.LateUpdate();

        PlayerCamera.instance.HandleAllCameraActions();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;

        // If this is the local player, assign the camera to us.
        if (IsOwner)
        {
            PlayerCamera.instance.player = this;
            PlayerInputManager.instance.player = this;
            WorldSaveGameManager.instance.player = this;

            // Update derived stats (and resources) when stats change
            playerNetworkManager.vitality.OnValueChanged += playerStatsManager.SetVitality;
            playerNetworkManager.endurance.OnValueChanged += playerStatsManager.SetEndurance;

            // Health bar listeners
            playerNetworkManager.currentHealth.OnValueChanged += PlayerUIManager.instance.playerUIHUDManager.SetNewHealthValue;

            // Stamina bar listeners
            playerNetworkManager.currentStamina.OnValueChanged += PlayerUIManager.instance.playerUIHUDManager.SetNewStaminaValue;
            playerNetworkManager.currentStamina.OnValueChanged += playerStatsManager.ResetStaminaRegenerationTimer;
        }
        // Why did I do this?
        playerAnimatorManager.character = this;

        // Stats
        playerNetworkManager.currentHealth.OnValueChanged += playerNetworkManager.CheckHP;

        // Equipment
        playerNetworkManager.currentRightHandWeaponID.OnValueChanged += playerNetworkManager.OnCurrentRightHandWeaponIDChange;
        playerNetworkManager.currentLeftHandWeaponID.OnValueChanged += playerNetworkManager.OnCurrentLeftHandWeaponIDChange;
        playerNetworkManager.currentWeaponBeingUsedID.OnValueChanged += playerNetworkManager.OnCurrentWeaponBeingUsedIDChange;

        if (IsOwner && !IsServer) // On connecting as a client
        {
            // We need to load this again in the client, because the game object was deleted and re-created.
            LoadGameDataFromCurrentCharacterData(ref WorldSaveGameManager.instance.currentCharacterSaveData);
        }
    }

    private void OnClientConnectedCallback(ulong clientID)
    {
        // Keep a list of active players in the game
        WorldGameSessionManager.instance.AddPlayerToActivePlayersList(this);

        // If we are the server, we are the host, so no need to sync with already-loaded players
        if (!IsServer && IsOwner)
        {
            foreach (var player in WorldGameSessionManager.instance.players)
            {
                if (player != this)
                {
                    player.LoadOtherPlayerCharacterWhenJoiningServer();
                }
            }
        }
    }

    public override IEnumerator ProcessDeathEvent(bool overrideDeathAnimation = false)
    {
        // For now, we'll do some different behaviour *before* the Character death event.
        // But most likely we'll need to segment the Character death event into functions so we can
        //  control the order.
        if (IsOwner)
        {
            PlayerUIManager.instance.popupManager.SendYouDiedPopup();
        }

        // Check for players that are alive (on the current team). Respawn if all are dead.

        return base.ProcessDeathEvent(overrideDeathAnimation);
    }

    public void SaveGameDataToCurrentCharacterData(ref CharacterSaveData currentCharacterSaveData)
    {
        currentCharacterSaveData.characterName = playerNetworkManager.characterName.Value.ToString();
        currentCharacterSaveData.timePlayed = playerNetworkManager.timePlayed.Value.ToString();

        // Location
        currentCharacterSaveData.sceneIndex = SceneManager.GetActiveScene().buildIndex;
        currentCharacterSaveData.xPosition = transform.position.x;
        currentCharacterSaveData.yPosition = transform.position.y;
        currentCharacterSaveData.zPosition = transform.position.z;

        // Stats
        currentCharacterSaveData.vitality = playerNetworkManager.vitality.Value;
        currentCharacterSaveData.endurance = playerNetworkManager.endurance.Value;

        // Resources
        currentCharacterSaveData.currentHealth = playerNetworkManager.currentHealth.Value;
        currentCharacterSaveData.currentStamina = playerNetworkManager.currentStamina.Value;
    }

    public void LoadGameDataFromCurrentCharacterData(ref CharacterSaveData currentCharacterSaveData)
    {
        playerNetworkManager.characterName.Value = currentCharacterSaveData.characterName;
        playerNetworkManager.timePlayed.Value = currentCharacterSaveData.timePlayed;
        Vector3 savedPosition = new Vector3(currentCharacterSaveData.xPosition, currentCharacterSaveData.yPosition, currentCharacterSaveData.zPosition);
        transform.position = savedPosition;

        // Health
        int maxHealth = playerStatsManager.CalculateMaxHealthBasedOnVitalityLevel(
            currentCharacterSaveData.vitality
        );
        PlayerUIManager.instance.playerUIHUDManager.SetMaxHealthValue(
            maxHealth
        );
        playerNetworkManager.maxHealth.Value = maxHealth;
        playerNetworkManager.currentHealth.Value = currentCharacterSaveData.currentHealth;

        // Stamina
        int maxStamina = playerStatsManager.CalculateMaxStaminaBasedOnEnduranceLevel(
            currentCharacterSaveData.endurance
        );
        PlayerUIManager.instance.playerUIHUDManager.SetMaxStaminaValue(
            maxStamina
        );
        playerNetworkManager.maxStamina.Value = maxStamina;
        playerNetworkManager.currentStamina.Value = currentCharacterSaveData.currentStamina;


    }

    public override void ReviveCharacter()
    {
        base.ReviveCharacter();

        if (IsOwner)
        {
            playerNetworkManager.currentHealth.Value = playerNetworkManager.maxHealth.Value;
            playerNetworkManager.currentStamina.Value = playerNetworkManager.maxStamina.Value;
            // TODO: Restore focus points

            // Play rebirth effects
            playerAnimatorManager.PlayTargetActionAnimation("Empty", false);


        }
    }

    private void LoadOtherPlayerCharacterWhenJoiningServer()
    {
        // This function makes up for the fact that other players' game worlds only update when
        //  a network variable is CHANGED. When we load in, the network variables are correct,
        //  but we need to run all the OnChanged actions.

        // Sync weapons
        playerNetworkManager.RunAllPersistentOnChangeEvents(
            playerNetworkManager.currentRightHandWeaponID.Value,
            playerNetworkManager.currentLeftHandWeaponID.Value
        );
    }

    // DEBUG, delete later
    private void DebugMenu()
    {
        if (debugRespawnCharacter)
        {
            debugRespawnCharacter = false;
            ReviveCharacter();

        }
        if (debugKillCharacter)
        {
            debugKillCharacter = false;
            playerNetworkManager.currentHealth.Value = 0;
        }

        if (switchRightWeapon)
        {
            switchRightWeapon = false;
            playerEquipmentManager.SwitchRightWeapon();
        }
        if (switchLeftWeapon)
        {
            switchLeftWeapon = false;
            playerEquipmentManager.SwitchLeftWeapon();
        }
    }
}
