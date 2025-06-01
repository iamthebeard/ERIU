using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : CharacterManager
{
    [HideInInspector] public PlayerLocomotionManager playerLocomotionManager;
    [HideInInspector] public PlayerAnimatorManager playerAnimatorManager;
    [HideInInspector] public PlayerNetworkManager playerNetworkManager;
    [HideInInspector] public PlayerStatsManager playerStatsManager;

    protected override void Awake()
    {
        base.Awake();

        // Do more, only for the player character
        playerLocomotionManager = GetComponent<PlayerLocomotionManager>(); // It's a component on the same object, so we can fetch it.
        playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
        playerNetworkManager = GetComponent<PlayerNetworkManager>();
        playerStatsManager = GetComponent<PlayerStatsManager>();
    }

    protected override void Update()
    {
        base.Update();

        // If we are not the owner of this player, we do not control its movement
        if (IsOwner) {
            // Handle movement
            playerLocomotionManager.HandleAllMovement();
            // I added this in the "do it yourself" in episode 5
            // Handle animations
            playerAnimatorManager.UpdateAnimatorMovement(0, playerLocomotionManager.moveAmount, characterNetworkManager.isSprinting.Value);

            // Regen stamina
            playerStatsManager.RegenerateStamina();
        }
        // Handle movement during animations for *ALL* characters

        if(isRolling) { // I had to add this because my animation doesn't have built in motion
            // Keep moving in the direction we started rolling
            characterController.Move(transform.forward * playerLocomotionManager.rollingSpeed * Time.deltaTime);
            return;
        }
        if(isBackstepping) { // I had to add this because my animation doesn't have built in motion
            // Keep moving in the direction we started rolling
            characterController.Move((-transform.forward) * playerLocomotionManager.backstepSpeed * Time.deltaTime);
            return;
        }
    }

    // In Unity, all camera actions should happen in LateUpdate
    protected override void LateUpdate() {
        if(!IsOwner) return;

        base.LateUpdate();

        PlayerCamera.instance.HandleAllCameraActions();
    }

    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();

        // If this is the local player, assign the camera to us.
        if(IsOwner) {
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
        playerAnimatorManager.character = this;
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
}
