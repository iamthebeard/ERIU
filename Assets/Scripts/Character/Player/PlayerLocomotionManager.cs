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

    [SerializeField] public float walkingSpeed = 1.5;
    [SerializeField] public float runningSpeed = 4.5;
    [SerializeField] public float rotationSpeed = 15;
    [SerializeField] public float rollingSpeed = 7;
    [SerializeField] public float backstepSpeed = 6;

    [Header("Player Actions")]
    private Vector3 rollDirection;
    private Vector3 backstepDirection;

    protected override void Awake() {
        base.Awake();

        player = GetComponent<PlayerManager>();
    }

    protected override void Update()
    {
        base.Update();

        // He added a bunch here after the "do it yourself" in episode 5, but mine is in the PlayerManager and CharacterManager
    }

    public void HandleAllMovement() {
        // Grounded Movement
        HandleGroundedMovement();

        // Aerial/Jumping Movement
        // Gravity
        // Rotation
        HandleRotation();
    }

    private void GetHorizontalAndVerticalInputs() {
        // These are being brought in from the PlayerInputManager and can be used interchangeably,
        //  so long as GetHorizontalAndVerticalInputs is called.
        horizontalMovement = PlayerInputManager.instance.horizontalInput;
        verticalMovement = PlayerInputManager.instance.verticalInput;
        moveAmount = PlayerInputManager.instance.moveAmount;

        // Clamp the movements (for animations)
    }

    private void HandleGroundedMovement() {
        GetHorizontalAndVerticalInputs();
        if(!player.canMove) return;

        // Movement direction is based on camera perspective and inputs
        moveDirection = PlayerCamera.instance.transform.forward * verticalMovement;
        moveDirection = moveDirection + PlayerCamera.instance.transform.right * horizontalMovement;
        moveDirection.Normalize();
        moveDirection.y = 0;

        if (PlayerInputManager.instance.moveAmount > 0.5f) {
            // Move at a running speed
            player.characterController.Move(moveDirection * runningSpeed * Time.deltaTime);

        } else if (PlayerInputManager.instance.moveAmount <= 0.5f) {
            // Move at a walking speed
            player.characterController.Move(moveDirection * walkingSpeed * Time.deltaTime);
        }
    }

    private void HandleRotation() {
        if(!player.canRotate) return;

        targetRotationDirection = Vector3.zero;
        targetRotationDirection = PlayerCamera.instance.cameraObject.transform.forward * verticalMovement;
        targetRotationDirection = targetRotationDirection + PlayerCamera.instance.cameraObject.transform.right * horizontalMovement;
        targetRotationDirection.Normalize();
        targetRotationDirection.y = 0;

        if (targetRotationDirection == Vector3.zero) {
            targetRotationDirection = transform.forward;
        }

        Quaternion newRotation = Quaternion.LookRotation(targetRotationDirection);
        Quaternion targetRotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
        transform.rotation = targetRotation;
    }

    public void AttemptToPerformDodge() {
        if (player.isPerformingAction) {
            return;
        }

        if (moveAmount > 0) {
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
        } else {
            // Dodge while still is a backstep
            backstepDirection = -player.transform.forward;

            // Perform a backstep animation
            player.isBackstepping = true;
            player.playerAnimatorManager.PlayTargetActionAnimation("Back_Step_01", true);
        }
    }
}
