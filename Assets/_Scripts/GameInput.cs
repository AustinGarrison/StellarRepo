using KinematicCharacterController;
using KinematicCharacterController.Examples;
using System;
using UnityEngine;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }

    public event EventHandler OnJumpAction;
    public event EventHandler OnCrouchAction;
    public event EventHandler OnStandAction;
    public event EventHandler OnInteractAction;
    public event EventHandler OnAltInteractAction;
    public event EventHandler OnInventoryToggled;

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
        playerInputActions.Player.Interact.performed += Interact_performed;
        playerInputActions.Player.AltInteract.performed += AltInteract_performed;
        playerInputActions.Player.Crouch.performed += Crouch_performed;
        playerInputActions.Player.Crouch.canceled += Crouch_canceled;
        playerInputActions.Player.InventoryToggle.performed += InventoryToggle_performed;

    }

    private void Jump_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnJumpAction?.Invoke(this, EventArgs.Empty);
    }

    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }

    private void AltInteract_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnAltInteractAction?.Invoke(this, EventArgs.Empty);
    }

    private void Crouch_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnCrouchAction?.Invoke(this, EventArgs.Empty);
    }

    private void Crouch_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnStandAction?.Invoke(this, EventArgs.Empty);
    }

    private void InventoryToggle_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnInventoryToggled?.Invoke(this, EventArgs.Empty);
    }

    private void OnDestroy()
    {
        playerInputActions.Player.Jump.performed -= Jump_performed;
        playerInputActions.Player.Interact.performed -= Interact_performed;
        playerInputActions.Player.AltInteract.performed -= Interact_performed;
        playerInputActions.Player.Crouch.performed -= Crouch_performed;
        playerInputActions.Player.Crouch.canceled -= Crouch_canceled;
        playerInputActions.Player.InventoryToggle.performed -= InventoryToggle_performed;

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
}
