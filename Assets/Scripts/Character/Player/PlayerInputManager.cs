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

    [Header("Camera Inputs")]
    [SerializeField] Vector2 cameraInput;
    [SerializeField] public float cameraHorizontalInput;
    [SerializeField] public float cameraVerticalInput;
    // [SerializeField] public float moveAmount;

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

    // Start is called before the first frame update
    private void Start() {
        DontDestroyOnLoad(gameObject);

        // Subscribe to the 'activeSceneChanged' event
        SceneManager.activeSceneChanged += OnSceneChange;

        // This script only activates when the scene changes.
        instance.enabled = false;
    }

    private void OnSceneChange(Scene oldScene, Scene newScene) {
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

    private void OnEnable() {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();

            playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
            playerControls.PlayerCamera.Movement.performed += i => cameraInput = i.ReadValue<Vector2>();
        }

        playerControls.Enable();
    }

    private void OnDestroy() {
        SceneManager.activeSceneChanged -= OnSceneChange;
    }

    private void OnApplicationFocus(bool focus) {
        if (enabled) {
            // Disable player controls when switching windows,
            //  so we can have two windows open to test multiplayer.
            if (focus) {
                playerControls.Enable();
            } else {
                playerControls.Disable();
            }
        }
    }

    private void Update() {
        HandleMovementInput();
        HandleCameraMovementInput();
    }

    private void HandleMovementInput() {
        horizontalInput = movementInput.x;
        verticalInput = movementInput.y;

        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));

        if (snapMovement) {
            // Clamp movement to only allow half speed or full speed (for a souls-like feel)
            if (moveAmount <= 0.5 && moveAmount > 0) {
                moveAmount = 0.5f;
            }
            else if (moveAmount > 0.5 && moveAmount <= 1) {
                moveAmount = 1.0f;
            }
        }

        // // Only animate if a player has been loaded
        // if (player == null) return;

        // // Only use the animations for forward movement while not locked on
        // player.playerAnimatorManager.UpdateAnimatorMovement(0, moveAmount);
    }

    private void HandleCameraMovementInput() {
        cameraHorizontalInput = cameraInput.x;
        cameraVerticalInput = cameraInput.y;
    }
}
