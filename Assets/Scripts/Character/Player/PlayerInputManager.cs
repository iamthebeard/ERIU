using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInputManager : MonoBehaviour
{
    // THINK ABOUT GOALS IN STEPS
    // 1. FIND A WAY TO READ THE VALUES OF A JOYSTICK/KEYS
    // 2. MOVE CHARACTER BASED ON THOSE VALUES

    public static PlayerInputManager instance;
    public PlayerManager player;
    PlayerControls playerControls;

    [Header("Control Options")]
    [SerializeField] bool snapMovement = true;

    [Header("Movement Inputs")]
    [SerializeField] Vector2 movementInput;
    [SerializeField] public float horizontalInput;
    [SerializeField] public float verticalInput;
    [SerializeField] public float moveAmount;

    [Header("Action Inputs")]
    [SerializeField] private bool dodgeInput = false;
    [SerializeField] private bool sprintInput = false;
    [SerializeField] private bool jumpInput = false;

    [Header("Attack Inputs")]
    [SerializeField] private bool rbInput = false;
    [SerializeField] private bool rtInput = false;
    [SerializeField] private bool rtChargedInput = false;

    [Header("Item Inputs")]
    [SerializeField] private bool switchRightWeaponInput = false;
    [SerializeField] private bool switchLeftWeaponInput = false;


    [Header("Camera Inputs")]
    [SerializeField] Vector2 cameraInput;
    [SerializeField] public float cameraHorizontalInput;
    [SerializeField] public float cameraVerticalInput;
    // [SerializeField] public float moveAmount;

    [Header("Lock On")]
    [SerializeField] private bool lockOnInput = false;
    [SerializeField] private bool lockOnSwitchLeftInput = false;
    [SerializeField] private bool lockOnSwitchRightInput = false;
    private Coroutine lockOnCoroutine;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        // Subscribe to the 'activeSceneChanged' event
        SceneManager.activeSceneChanged += OnSceneChange;

        // This script only activates when the scene changes.
        instance.enabled = false;
    }

    private void OnSceneChange(Scene oldScene, Scene newScene)
    {
        if (newScene.buildIndex == WorldSaveGameManager.instance.GetWorldSceneIndex())
        {
            // Make this script active only when we are on the World Scene
            instance.enabled = true;
        }
        else // We don't want the character to be controllable on the main menu screen, during character creation, etc.
        {
            instance.enabled = false;
        }
    }

    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();

            // Movement
            playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();

            // Actions
            playerControls.PlayerActions.Dodge.performed += i => dodgeInput = true;
            playerControls.PlayerActions.Sprint.performed += i => sprintInput = true; // Holding activates
            playerControls.PlayerActions.Sprint.canceled += i => sprintInput = false; // Releasing deactivates
            playerControls.PlayerActions.Jump.performed += i => jumpInput = true;

            // Item Actions
            playerControls.PlayerActions.SwitchRightWeapon.performed += i => switchRightWeaponInput = true;
            playerControls.PlayerActions.SwitchLeftWeapon.performed += i => switchLeftWeaponInput = true;

            // Attacks
            playerControls.PlayerActions.RB.performed += i => rbInput = true;
            playerControls.PlayerActions.RT.performed += i => rtInput = true;
            playerControls.PlayerActions.ChargedRT.performed += i => rtChargedInput = true;
            playerControls.PlayerActions.ChargedRT.canceled += i => rtChargedInput = false;

            // Camera
            playerControls.PlayerCamera.Movement.performed += i => cameraInput = i.ReadValue<Vector2>();
            playerControls.PlayerActions.LockOn.performed += i => lockOnInput = true;
            playerControls.PlayerActions.SwitchLockOnTargetLeft.performed += i => lockOnSwitchLeftInput = true;
            playerControls.PlayerActions.SwitchLockOnTargetRight.performed += i => lockOnSwitchRightInput = true;
        }

        playerControls.Enable();

    }
    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnSceneChange;
    }

    private void OnApplicationFocus(bool focus)
    {
        if (enabled)
        {
            // Disable player controls when switching windows,
            //  so we can have two windows open to test multiplayer.
            if (focus)
            {
                playerControls.Enable();
            }
            else
            {
                playerControls.Disable();
            }
        }
    }

    private void Update()
    {
        if (player == null) // Not sure if I need this, but it seems to fail to load the client without it.
        {
            return;
        }

        HandleMovementInput();

        HandleDodgeInput();
        HandleSprintInput();
        HandleJumpInput();

        HandleRBInput();
        HandleRTInput();
        HandleChargedRTInput();
        HandleWeaponSwitchingInput();

        HandleCameraMovementInput();
        HandleLockOnInput();
        HandleLockOnSwitchInput();
    }

    private void HandleMovementInput()
    {
        horizontalInput = movementInput.x;
        verticalInput = movementInput.y;

        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));

        if (snapMovement)
        {
            // Clamp movement to only allow half speed or full speed (for a souls-like feel)
            if (moveAmount <= 0.5 && moveAmount > 0)
            {
                moveAmount = 0.5f;
            }
            else if (moveAmount > 0.5 && moveAmount <= 1)
            {
                moveAmount = 1.0f;
            }
        }

        if (moveAmount != 0)
        {
            player.playerNetworkManager.isMoving.Value = true;
        }
        else
        {
            player.playerNetworkManager.isMoving.Value = false;
        }

        // This code was in the tutorial up until the "do it yourself" in episode 5
        // // Only animate if a player has been loaded
        // if (player == null) return;

        // // Only use the animations for forward movement while not locked on
        // player.playerAnimatorManager.UpdateAnimatorMovement(0, moveAmount);
    }

    private void HandleCameraMovementInput()
    {
        cameraHorizontalInput = cameraInput.x;
        cameraVerticalInput = cameraInput.y;
    }

    private void HandleDodgeInput()
    {
        if (dodgeInput)
        {
            dodgeInput = false;

            // Perform a dodge
            player.playerLocomotionManager.AttemptToPerformDodge();
            // Future note: don't perform a dodge if menu or UI is open
        }
    }

    private void HandleSprintInput()
    {
        if (sprintInput)
        {
            player.playerLocomotionManager.HandleSprinting();
        }
        else
        {
            player.playerLocomotionManager.HandleStopSprinting();
        }
    }

    private void HandleJumpInput()
    {
        if (jumpInput)
        {
            jumpInput = false;

            // If we have a UI window open, exit without doing anything

            // Attempt to perform a jump
            player.playerLocomotionManager.AttemptToPerformJump();
        }
    }

    private void HandleRBInput ()
    {
        if (rbInput && !rtInput)
        {
            rbInput = false; // Only trigger once

            // TODO: If we have a UI window open, exit

            player.playerNetworkManager.SetCharacterActionHand(true /*Right*/);

            // TODO: If we are two-handed, run the two-handed action

            // If we are one-handing, run the one-handed action
            player.playerCombatManager.PerformWeaponBasedAction(player.playerInventoryManager.currentRightHandWeapon.rb_Action_OneHanded, player.playerInventoryManager.currentRightHandWeapon);
        }
    }

    private void HandleRTInput ()
    {
        if (rtInput)
        {
            rtInput = false; // Only trigger once

            // TODO: If we have a UI window open, exit

            player.playerNetworkManager.SetCharacterActionHand(true /*Right*/);

            // TODO: If we are two-handed, run the two-handed action

            // If we are one-handing, run the one-handed action
            player.playerCombatManager.PerformWeaponBasedAction(player.playerInventoryManager.currentRightHandWeapon.rt_Action_OneHanded, player.playerInventoryManager.currentRightHandWeapon);
        }
    }

    private void HandleChargedRTInput ()
    {
        if (player.isPerformingAction)
        {
            // We only care about charged inputs if they've already starting their attack
            if (player.playerNetworkManager.isUsingRightHand.Value)
            {
                player.playerNetworkManager.isChargingAttack.Value = rtChargedInput;
            }
            // else if (player.playerNetworkManager.isUsingLeftHand.Value)
            // {
            //     player.playerNetworkManager.isChargingAttack.Value
            // }
        }
    }

    private void HandleWeaponSwitchingInput()
    {
        if (switchRightWeaponInput)
        {
            switchRightWeaponInput = false;

            player.playerEquipmentManager.SwitchRightWeapon();
        }

        if (switchLeftWeaponInput)
        {
            switchLeftWeaponInput = false;

            player.playerEquipmentManager.SwitchLeftWeapon();
        }
    }

    private void HandleLockOnInput()
    {
        // Is our current target dead?
        if (player.playerNetworkManager.isLockedOn.Value)
        {
            if (player.playerCombatManager.currentLockOnTarget == null) return;

            if (player.playerCombatManager.currentLockOnTarget.isDead.Value)
            {
                player.playerNetworkManager.isLockedOn.Value = false;

                // Attempt to find new target, or unlock.
                if (lockOnCoroutine != null)
                    StopCoroutine(lockOnCoroutine); // Cancel any still waiting
                lockOnCoroutine = StartCoroutine(PlayerCamera.instance.WaitThenFindNewTarget());
            }

        }

        if (lockOnInput)
        {
            lockOnInput = false;

            // Are we locked on to a target already? If so, unlock.
            if (player.playerNetworkManager.isLockedOn.Value)
            {
                // Disable lock on
                player.playerNetworkManager.isLockedOn.Value = false;
                player.playerCombatManager.SetLockOnTarget(null);
                Debug.Log("Unlocking LockOn");
                PlayerCamera.instance.ClearLockOnTargets();
                return;
            }

            // If aiming with a ranged weapon, don't allow lock on (return from function)
            
            // Attempt to find a target
            // Enable lock on
            PlayerCamera.instance.HandleLocatingLockOnTargets();

            if (PlayerCamera.instance.nearestLockOnTarget != null)
            {
                // Assign as our target
                player.playerCombatManager.SetLockOnTarget(PlayerCamera.instance.nearestLockOnTarget);
                player.playerNetworkManager.isLockedOn.Value = true;
                // player.playerNetworkManager.lockOnTargetID = PlayerCamera.instance.nearestLockOnTarget.
            }
        }   
    }

    private void HandleLockOnSwitchInput()
    {
        if (lockOnSwitchLeftInput)
        {
            lockOnSwitchLeftInput = false;

            if (player.playerNetworkManager.isLockedOn.Value)
            {
                PlayerCamera.instance.HandleLocatingLockOnTargets();

                if (PlayerCamera.instance.leftLockOnTarget != null)
                {
                    player.playerCombatManager.SetLockOnTarget(PlayerCamera.instance.leftLockOnTarget);
                }
            }
        }

        if (lockOnSwitchRightInput)
        {
            lockOnSwitchRightInput = false;

            if (player.playerNetworkManager.isLockedOn.Value)
            {
                PlayerCamera.instance.HandleLocatingLockOnTargets();

                if (PlayerCamera.instance.rightLockOnTarget != null)
                {
                    player.playerCombatManager.SetLockOnTarget(PlayerCamera.instance.rightLockOnTarget);
                }
            }
        }
    }
}
