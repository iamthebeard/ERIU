using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotionManager : CharacterLocomotionManager
{
    PlayerManager player;
    // Will be taken from the PlayerInputManager
    public float verticalMovement;
    public float horizontalMovement;
    public float moveAmount;

    private Vector3 moveDirection;
    private Vector3 targetRotationDirection;

    [SerializeField] float walkingSpeed = 2;
    [SerializeField] float runningSpeed = 5;
    [SerializeField] float rotationSpeed = 15;

    protected override void Awake() {
        base.Awake();

        player = GetComponent<PlayerManager>();
    }

    protected override void Update()
    {
        base.Update();

        // He added a bunch here after the "do it yourself" in episode 6, but mine is in the PlayerManager and CharacterManager
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
        horizontalMovement = PlayerInputManager.instance.horizontalInput;
        verticalMovement = PlayerInputManager.instance.verticalInput;
        moveAmount = PlayerInputManager.instance.moveAmount;

        // Clamp the movements (for animations)
    }

    private void HandleGroundedMovement() {
        GetHorizontalAndVerticalInputs();

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
}
