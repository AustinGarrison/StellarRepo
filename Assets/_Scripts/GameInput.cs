using KinematicCharacterController;
using KinematicCharacterController.Examples;
using System;
using UnityEngine;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }

    [Header("Player")]
    public PlayerController player;
    public PlayerCamera playerCamera;

    public event EventHandler OnJumpAction;
    public event EventHandler OnCrouchAction;
    public event EventHandler OnStandAction;

    public enum Binding
    {
        Move_Up,
        Move_Down,
        Move_Left,
        Move_Right,
        Jump,
        Crouch,
    }

    private PlayerInputActions playerInputActions;

    private void Awake()
    {
        Instance = this;

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();

        playerInputActions.Player.Jump.performed += Jump_performed;
        playerInputActions.Player.Crouch.performed += Crouch_performed;
        playerInputActions.Player.Crouch.canceled += Crouch_canceled;
    }


    private void Crouch_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnCrouchAction?.Invoke(this, EventArgs.Empty);
    }

    private void Crouch_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnStandAction?.Invoke(this, EventArgs.Empty);
    }


    private void Jump_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnJumpAction?.Invoke(this, EventArgs.Empty);
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        // Tell camera to follow transform
        playerCamera.SetFollowTransform(player.cameraFollowPoint);

        // Ignore the character's collider(s) for camera obstruction checks
        playerCamera.IgnoredColliders.Clear();
        playerCamera.IgnoredColliders.AddRange(player.GetComponentsInChildren<Collider>());
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        HandleCharacterInput();
    }

    private void LateUpdate()
    {
        if (playerCamera.RotateWithPhysicsMover && player.Motor.AttachedRigidbody != null)
        {
            playerCamera.PlanarDirection = player.Motor.AttachedRigidbody.GetComponent<PhysicsMover>().RotationDeltaFromInterpolation * playerCamera.PlanarDirection;
            playerCamera.PlanarDirection = Vector3.ProjectOnPlane(playerCamera.PlanarDirection, player.Motor.CharacterUp).normalized;
        }

        HandleCameraInput();
        GetScrollAxis();
    }

    private void OnDestroy()
    {

        playerInputActions.Player.Jump.performed -= Jump_performed;
        playerInputActions.Player.Crouch.performed -= Crouch_performed;
        playerInputActions.Player.Crouch.canceled -= Crouch_canceled;

        playerInputActions.Dispose();
    }


    public Vector2 GetMovementVectorNormalized()
    {
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();
        inputVector = inputVector.normalized;
        return inputVector;
    }

    public Vector2 GetLookVector()
    {
        return playerInputActions.Player.Look.ReadValue<Vector2>();
    }

    public float GetScrollAxis()
    {
        return playerInputActions.Player.MouseScrollY.ReadValue<float>();
    }

    private void HandleCameraInput()
    {
        Vector2 look = GameInput.Instance.GetLookVector();

        Vector3 lookInputVector = new Vector3(look.x / 20, look.y / 20, 0f);

        // Prevent moving the camera while the cursor isn't locked
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            lookInputVector = Vector3.zero;
        }

        // Input for zooming the camera (disabled in WebGL because it can cause problems)
        float scrollInput = -GameInput.Instance.GetScrollAxis() / 1000;

#if UNITY_WEBGL
        scrollInput = 0f;
#endif

        // Apply inputs to the camera
        playerCamera.UpdateWithInput(Time.deltaTime, scrollInput, lookInputVector);

        // Handle toggling zoom level
        if (Input.GetMouseButtonDown(1))
        {
            playerCamera.TargetDistance = (playerCamera.TargetDistance == 0f) ? playerCamera.DefaultDistance : 0f;
        }
    }

    private void HandleCharacterInput()
    {
        PlayerInputs playerInputs = new PlayerInputs();

        playerInputs.CameraRotation = playerCamera.Transform.rotation;

        // Apply inputs to character
        //player.SetInputs(ref playerInputs);
    }
}
