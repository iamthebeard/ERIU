using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    [Header("Lock On")]
    [SerializeField] private float lockOnRadius = 20;
    [SerializeField] private float lockOnFieldOfView = 50; // +-, so between -50 and 50
    [SerializeField] private float lockOnTargetFollowSpeed = 0.2f;
    // private List<CharacterManager> availableLockOnTargets = new List<CharacterManager>(); // I don't think we need this. Just keep track of the closest?
    [SerializeField] public List<CharacterManager> potentialLockOnTargets = new List<CharacterManager>();
    [SerializeField] public CharacterManager nearestLockOnTarget;
    [SerializeField] public CharacterManager leftLockOnTarget;
    [SerializeField] public CharacterManager rightLockOnTarget;
    

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

        if (player.playerNetworkManager.isLockedOn.Value)
        {
            // If locked on, force rotation towards target.
            LookAt(player.playerCombatManager.currentLockOnTarget.characterCombatManager.lockOnAnchor);
        }
        else
        {
            Vector3 cameraRotation;
            Quaternion targetRotation;
            
            // Normal rotation based on camera movement inputs
            leftAndRightLookAngle += (PlayerInputManager.instance.cameraHorizontalInput * leftAndRightRotationSpeed) * Time.deltaTime;
            upAndDownLookAngle += (PlayerInputManager.instance.cameraVerticalInput * upAndDownRotationSpeed) * Time.deltaTime;

            upAndDownLookAngle = Mathf.Clamp(upAndDownLookAngle, minimumPivot, maximumPivot);

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
        }
    }

    public void LookAt(Transform target)
    {
        Vector3 cameraRotation;
        Quaternion targetRotation;
        // Rotate camera game object
        cameraRotation = target.position - transform.position; // Based on target's lockOnAnchor
        cameraRotation.Normalize();
        cameraRotation.y = 0;
        targetRotation = Quaternion.LookRotation(cameraRotation);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lockOnTargetFollowSpeed);

        // Rotate camera pivot object
        cameraRotation = target.position - cameraPivotTransform.position;
        cameraRotation.Normalize();
        targetRotation = Quaternion.LookRotation(cameraRotation);
        cameraPivotTransform.transform.rotation = Quaternion.Slerp(cameraPivotTransform.rotation, targetRotation, lockOnTargetFollowSpeed);

        // Save our rotation values (so that when you unlock, the camera doesn't suddenly move)
        leftAndRightLookAngle = transform.eulerAngles.y;
        upAndDownLookAngle = transform.eulerAngles.x;
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

    public void HandleLocatingLockOnTargets()
    {
        float shortestDistance = Mathf.Infinity;
        nearestLockOnTarget = null;
        // potentialLockOnTargets.Clear();
        float shortestRightDistance = Mathf.Infinity; // Will be used to find "next" target to the right
        float shortestLeftDistance = -Mathf.Infinity; // Closest "next" target to the left along the horizontal view axis (-)
        leftLockOnTarget = null;
        rightLockOnTarget = null;

        Collider[] colliders = Physics.OverlapSphere(player.transform.position, lockOnRadius, WorldUtilityManager.Instance.GetCharacterLayers());

        for (int i = 0; i < colliders.Length; i++)
        {
            CharacterManager lockOnTarget = colliders[i].GetComponent<CharacterManager>(); // > Does this need to be Get InParent?
            // Check if it the collision is with a character (object with a CharacterManager)
            if (lockOnTarget != null)
            {
                if (lockOnTarget.isDead.Value)
                    continue;

                if (lockOnTarget.transform.root == player.transform.root)
                // Try if (lockOnTarge == player)
                    // Don't lock onto self
                    continue;

                // Check if they are within our field of view
                Vector3 lockOnTargetDirection = lockOnTarget.transform.position - player.transform.position;
                float angleBetweenLockOnTargetAndCamera = Vector3.Angle(lockOnTargetDirection, cameraObject.transform.forward);
                // bool inFieldOfView = !(-lockOnFieldOfView <= angleBetweenLockOnTargetAndCamera && angleBetweenLockOnTargetAndCamera <= lockOnFieldOfView);
                // Or 
                bool inFieldOfView = angleBetweenLockOnTargetAndCamera < -lockOnFieldOfView || lockOnFieldOfView < angleBetweenLockOnTargetAndCamera;
                if (inFieldOfView)
                    // Don't lock onto targets outside of field of view
                    continue;
                
                // Check if the potential target is blocked by the environment
                RaycastHit hit;
                // TODO: Only check the environment layers
                if (Physics.Linecast(player.playerCombatManager.lockOnAnchor.position, lockOnTarget.characterCombatManager.lockOnAnchor.position,
                                        out hit, WorldUtilityManager.Instance.GetEnvironmentLayers()))
                {
                    // We hit something, so there is no line of sight between us and our target.
                    // This cannot be our lockon target.
                    continue;
                }

                // Update shortest distance
                float distanceFromTarget = Vector3.Distance(player.transform.position, lockOnTarget.transform.position);
                if (distanceFromTarget < shortestDistance)
                {
                    shortestDistance = distanceFromTarget;
                    nearestLockOnTarget = lockOnTarget;
                }

                Debug.Log("Potential lockon target: character " + lockOnTarget.NetworkObjectId + " at distance " + distanceFromTarget + " and angle " + angleBetweenLockOnTargetAndCamera + ".");
                potentialLockOnTargets.Add(lockOnTarget);

                // Additionally, keep track of closest target left and right of our current target, if we have one.
                if (player.playerNetworkManager.isLockedOn.Value)
                {
                    if (lockOnTarget == player.playerCombatManager.currentLockOnTarget)
                        continue; // Don't count the current target

                    // // Determine the position of the enemy in our own reference frame.
                    // Vector3 relativeEnemyPosition = player.transform.InverseTransformPoint(lockOnTarget.transform.position);
                    // float distanceFromTargetLeft = relativeEnemyPosition.x;
                    // float distanceFromTargetRight = relativeEnemyPosition.x;
                    // ### Note: I had to change from his x-axis distance system to an angular system,
                    //      because his assumes some things about the direction the model is facing.
                    //      I'm just using the angle between the line to the current target 
                    //      and the line to the potential target as we're looping through.
                    Vector3 toCurrentTarget = player.playerCombatManager.currentLockOnTarget.transform.position - player.transform.position;
                    Vector3 toPotentialTarget = lockOnTarget.transform.position - player.transform.position;
                    float angleFromCurrentTarget = Vector3.SignedAngle(toCurrentTarget, toPotentialTarget, Vector3.up); // Find angle between two vectors on the XZ (horizontal) plane

                    // Keep track of the closest on the left and the right
                    // if (relativeEnemyPosition.x <= 0.0f && distanceFromTargetLeft > shortestLeftDistance) // to the left, and closest so far
                    if (angleFromCurrentTarget < 0 && angleFromCurrentTarget > shortestLeftDistance)
                    {
                        // shortestLeftDistance = distanceFromTargetLeft;
                        shortestLeftDistance = angleFromCurrentTarget;
                        leftLockOnTarget = lockOnTarget;
                    }
                    // else if (relativeEnemyPosition.x > 0.0f && distanceFromTargetRight < shortestRightDistance) // to the right, and closest so far
                    else if (angleFromCurrentTarget > 0 && angleFromCurrentTarget < shortestRightDistance)
                    {
                        // shortestRightDistance = distanceFromTargetRight;
                        shortestRightDistance = angleFromCurrentTarget;
                        rightLockOnTarget = lockOnTarget;
                    }
                }
            }
        }
        if (leftLockOnTarget != null)
            Debug.Log("Closest alternate to left: " + leftLockOnTarget.NetworkObjectId + ".");
        if (rightLockOnTarget != null)
            Debug.Log("Closest alternate to right: " + rightLockOnTarget.NetworkObjectId + ".");

        if (nearestLockOnTarget != null)
        {
            Debug.Log("Closest LockOn target:" + nearestLockOnTarget.NetworkObjectId + " at distance " + shortestDistance + ".");
        }
        else
        {
            Debug.Log("No LockOn targets found.");
            player.playerNetworkManager.isLockedOn.Value = false;
            ClearLockOnTargets();
        }
    }

    public void ClearLockOnTargets()
    {
        potentialLockOnTargets.Clear();
        nearestLockOnTarget = null;
        leftLockOnTarget = null;
        rightLockOnTarget = null;
    }
}
