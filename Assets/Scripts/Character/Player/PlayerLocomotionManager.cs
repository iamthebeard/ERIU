using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotionManager : CharacterLocomotionManager
{
    PlayerManager player;
    // Will be taken from the PlayerInputManager
    [HideInInspector] public float verticalMovement;
    [HideInInspector] public float horizontalMovement;
    [HideInInspector] public float moveAmount;

    [Header("Movement Settings")]
    public Vector3 moveDirection;
    public Vector3 targetRotationDirection;

    [SerializeField] public float walkingSpeed = 1.5f;
    [SerializeField] public float runningSpeed = 4.5f;
    [SerializeField] public float sprintingSpeed = 6.5f;
    [SerializeField] public float rotationSpeed = 15;

    [Header("Dodging")]

    [SerializeField] public float rollingSpeed = 7;
    [SerializeField] public float backstepSpeed = 6;

    [Header("Jumping")]
    [SerializeField] public float jumpHeight = 2;
    private Vector3 jumpDirection;
    [SerializeField] public float jumpingMomentumSpeed = 3;
    [SerializeField] public float freeFallControlledMovementSpeed = 1.5f;

    [Header("Stamina Values")]
    [SerializeField] public float sprintingStaminaCost = 10; // Per second?
    [SerializeField] public float dodgeStaminaCost = 25;
    [SerializeField] public float jumpStaminaCost = 25;

    [Header("Player Actions")]
    private Vector3 rollDirection;
    private Vector3 backstepDirection;

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>();
    }

    protected override void Update()
    {
        base.Update();

        // He added a bunch here after the "do it yourself" in episode 5, but mine is in the PlayerManager and CharacterManager
    }

    public void HandleAllMovement()
    {
        // Grounded Movement
        HandleGroundedMovement();

        // Aerial/Jumping Movement
        HandleJumpingMovement();
        HandleFreeFallMovement();

        // Gravity
        // Rotation
        HandleRotation();
    }

    private void GetHorizontalAndVerticalInputs()
    {
        // These are being brought in from the PlayerInputManager and can be used interchangeably,
        //  so long as GetHorizontalAndVerticalInputs is called.
        horizontalMovement = PlayerInputManager.instance.horizontalInput;
        verticalMovement = PlayerInputManager.instance.verticalInput;
        moveAmount = PlayerInputManager.instance.moveAmount;

        // Clamp the movements (for animations)
    }

    private void HandleGroundedMovement()
    {
        GetHorizontalAndVerticalInputs();
        if (!player.canMove) return;

        // Movement direction is based on camera perspective and inputs
        moveDirection = PlayerCamera.instance.transform.forward * verticalMovement;
        moveDirection = moveDirection + PlayerCamera.instance.transform.right * horizontalMovement;
        moveDirection.Normalize();
        moveDirection.y = 0;

        if (player.playerNetworkManager.isSprinting.Value)
        {
            player.characterController.Move(moveDirection * sprintingSpeed * Time.deltaTime);
        }
        else if (PlayerInputManager.instance.moveAmount > 0.5f)
        {
            // Move at a running speed
            player.characterController.Move(moveDirection * runningSpeed * Time.deltaTime);

        }
        else if (PlayerInputManager.instance.moveAmount <= 0.5f)
        {
            // Move at a walking speed
            player.characterController.Move(moveDirection * walkingSpeed * Time.deltaTime);
        }
    }

    private void HandleJumpingMovement()
    {
        if (player.isJumping)
        {
            // "Momentum" from the jump
            player.characterController.Move(jumpDirection * jumpingMomentumSpeed * Time.deltaTime);
            // Slight movement control during free-fall is handled in HandleFreeFallMovement
        }
    }

    // Give the player slight movement control during a jump or a fall to increase the feel of handling
    private void HandleFreeFallMovement()
    {
        if (!player.isGrounded)
        {
            Vector3 freeFallDirection = PlayerCamera.instance.transform.forward * verticalMovement;
            freeFallDirection += PlayerCamera.instance.transform.right * horizontalMovement;
            freeFallDirection.y = 0;
            freeFallDirection.Normalize();

            player.characterController.Move(freeFallDirection * freeFallControlledMovementSpeed * Time.deltaTime);
        }
    }

    private void HandleRotation()
    {
        if (!player.canRotate) return;

        targetRotationDirection = Vector3.zero;
        targetRotationDirection = PlayerCamera.instance.cameraObject.transform.forward * verticalMovement;
        targetRotationDirection = targetRotationDirection + PlayerCamera.instance.cameraObject.transform.right * horizontalMovement;
        targetRotationDirection.Normalize();
        targetRotationDirection.y = 0;

        if (targetRotationDirection == Vector3.zero)
        {
            targetRotationDirection = transform.forward;
        }

        Quaternion newRotation = Quaternion.LookRotation(targetRotationDirection);
        Quaternion targetRotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
        transform.rotation = targetRotation;
    }

    public void AttemptToPerformDodge()
    {
        if (player.isPerformingAction)
            return;

        if (player.playerNetworkManager.currentStamina.Value <= 0)
            return; // No dodging when out of stamina

        if (moveAmount > 0)
        {
            // Dodge while moving is a roll
            rollDirection = PlayerCamera.instance.cameraObject.transform.forward * verticalMovement;
            // Right now we shouldn't take horizontal movement into account, maybe?
            rollDirection += PlayerCamera.instance.cameraObject.transform.right * horizontalMovement;
            rollDirection.y = 0;
            rollDirection.Normalize();

            Quaternion playerRotation = Quaternion.LookRotation(rollDirection);
            player.transform.rotation = playerRotation;

            // Perform a roll animation
            player.isRolling = true;
            player.playerAnimatorManager.PlayTargetActionAnimation("Roll_Forward_01", true);
        }
        else
        {
            // Dodge while still is a backstep
            backstepDirection = -player.transform.forward;

            // Perform a backstep animation
            player.isBackstepping = true;
            player.playerAnimatorManager.PlayTargetActionAnimation("Back_Step_01", true);
        }

        player.playerNetworkManager.currentStamina.Value -= dodgeStaminaCost;
    }

    public void HandleSprinting()
    {
        if (player.isPerformingAction)
        {
            // Set sprinting to false and return (don't sprint)
            player.playerNetworkManager.isSprinting.Value = false;
        }

        // If we are out of stamina, set sprinting to false and return
        else if (player.playerNetworkManager.currentStamina.Value <= 0)
        {
            player.playerNetworkManager.isSprinting.Value = false;
        }

        // If we are stationary/walking slowly, set sprinting to false and return
        else if (moveAmount <= 0.5)
        {
            player.playerNetworkManager.isSprinting.Value = false;
        }
        // If we are moving, set sprinting to true
        else
        {
            player.playerNetworkManager.isSprinting.Value = true;
        }

        if (player.playerNetworkManager.isSprinting.Value)
        {
            player.playerNetworkManager.currentStamina.Value -= sprintingStaminaCost * Time.deltaTime;
        }
    }

    public void HandleStopSprinting()
    {
        player.playerNetworkManager.isSprinting.Value = false;
    }

    public void AttemptToPerformJump()
    {
        if (player.isPerformingAction) // Will need some expansion when combat is added
            return;

        if (player.playerNetworkManager.currentStamina.Value <= 0)
            return; // No jumping when out of stamina

        if (player.isJumping)
            return;

        if (!player.isGrounded)
            return;

        // Play jumping animation (1H or 2H)
        if (moveAmount > 0.5f) // Running jump
            player.playerAnimatorManager.PlayTargetActionAnimation("JumpMove", false, false, false, false);
        else player.playerAnimatorManager.PlayTargetActionAnimation("JumpWithLaunch", false, false, false, false);
        player.isJumping = true;

        // Stamina cost
        player.playerNetworkManager.currentStamina.Value -= jumpStaminaCost;

        // Forward movement during jump due to starting momentum
        jumpDirection = PlayerCamera.instance.cameraObject.transform.forward * verticalMovement;// PlayerInputManager.instance.verticalInput;
        jumpDirection += PlayerCamera.instance.cameraObject.transform.right * horizontalMovement; // PlayerInputManager.instance.horizontalInput;
        jumpDirection.y = 0;
        jumpDirection.Normalize();

        if (player.playerNetworkManager.isSprinting.Value)
            jumpDirection *= 1.25f;
        else if (moveAmount > 0.5f)
            jumpDirection *= 1;
        else if (moveAmount > 0)
            jumpDirection *= 0.5f;
        // else // Not moving
        //     jumpDirection *= 0;
        // Should be fine because jumpDirection will equal Vector3.zero
    }

    public void ApplyJumpingVelocity()
    {
        // Apply an upward velocity depending on forces from our game (from the animator)
        yVelocity.y = Mathf.Sqrt(jumpHeight * -2 * gravityForce);
    }
}
