using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera instance;

    public Camera cameraObject;
    public PlayerManager player;
    [SerializeField] Transform cameraPivotTransform;

    [Header("Camera Settings")] // Can change to impact camera operation
    [SerializeField] float cameraSmoothSpeed = 1; // The time it takes for the camera to reach its final position during movement
    [SerializeField] float leftAndRightRotationSpeed = 220;
    [SerializeField] float upAndDownRotationSpeed = 220;
    [SerializeField] float minimumPivot = -30; // Lowest angle you can look
    [SerializeField] float maximumPivot = 60; // Highest angle you can look
    [SerializeField] float cameraCollisionRadius = 0.2f; // Minimum distance from objects
    [SerializeField] LayerMask collideWithLayers;

    [Header("Camera Values")] // Just to display internal camera values
    [SerializeField] Vector3 cameraVelocity;
    [SerializeField] float leftAndRightLookAngle;
    [SerializeField] float upAndDownLookAngle;
    // For camera collision
    [SerializeField] Vector3 cameraObjectPosition;
    [SerializeField] float cameraZPosition;
    [SerializeField] float targetCameraZPosition;

    private void Awake() {
        if (instance == null)
        {
            instance = this;
        }
        else 
        {
            Destroy(gameObject);
        }
    }

    private void Start() {
        DontDestroyOnLoad(gameObject);
        cameraZPosition = cameraObject.transform.localPosition.z;
    }

    public void HandleAllCameraActions() {
        if (player != null) {
            // 1. Follow the player
            HandleFollowTarget();
            // 2. Rotate around the player
            HandleRotations();
            // 3. Collide with the environment
            HandleCollisions();
        }
    }

    private void HandleFollowTarget() {
        Vector3 targetCameraPosition =
            Vector3.SmoothDamp(
                transform.position,
                player.transform.position,
                ref cameraVelocity,
                cameraSmoothSpeed * Time.deltaTime
            );
        transform.position = targetCameraPosition;
    }

    private void HandleRotations() {
        // Normal rotation based on camera movement inputs
        leftAndRightLookAngle += (PlayerInputManager.instance.cameraHorizontalInput * leftAndRightRotationSpeed) * Time.deltaTime;
        upAndDownLookAngle += (PlayerInputManager.instance.cameraVerticalInput * upAndDownRotationSpeed) * Time.deltaTime;

        upAndDownLookAngle = Mathf.Clamp(upAndDownLookAngle, minimumPivot, maximumPivot);

        Vector3 cameraRotation;
        Quaternion targetRotation;
        // Rotate this game object left and right
        cameraRotation = Vector3.zero;
        cameraRotation.y = leftAndRightLookAngle; // Rotation *about* the y axis is left-right
        targetRotation = Quaternion.Euler(cameraRotation);
        transform.rotation = targetRotation;

        // Rotate the camera pivot object up and down
        cameraRotation = Vector3.zero;
        cameraRotation.x = upAndDownLookAngle; // Rotation *about* the x asix is up-down
        targetRotation = Quaternion.Euler(cameraRotation);
        cameraPivotTransform.localRotation = targetRotation;

        // If locked on, force rotation towards target.


    }

    private void HandleCollisions() {
        targetCameraZPosition = cameraZPosition;
        RaycastHit hit;
        Vector3 direction = cameraObject.transform.position - cameraPivotTransform.position;
        direction.Normalize();

        // Check if there is an object in the way of our desired direction
        if (Physics.SphereCast(
                cameraPivotTransform.position,
                cameraCollisionRadius,
                direction,
                out hit,
                Mathf.Abs(targetCameraZPosition),
                collideWithLayers
            )) {
                // If the object is too close, back off from it.
                float distanceFromHitObject = Vector3.Distance(cameraPivotTransform.position, hit.point);
                targetCameraZPosition = -(distanceFromHitObject - cameraCollisionRadius);
        }

        // Snap back
        if (Mathf.Abs(targetCameraZPosition) < cameraCollisionRadius) {
            targetCameraZPosition = -cameraCollisionRadius;
        }

        // Lerp for smooth movement (time of 0.2f)
        cameraObjectPosition.z = Mathf.Lerp(cameraObject.transform.localPosition.z, targetCameraZPosition, 0.2f);
        cameraObject.transform.localPosition = cameraObjectPosition;
    }
}
