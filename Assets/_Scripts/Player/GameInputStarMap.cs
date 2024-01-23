using System;
using UnityEngine;

public class GameInputStarMap : MonoBehaviour
{
    public static GameInputStarMap Instance { get; private set; }

    public event EventHandler OnAccelerate;
    public event EventHandler OnCrouchAction;
    public event EventHandler OnStandAction;

    private PlayerInputActions playerInputActions;

    private void Awake()
    {
        Instance = this;

        playerInputActions = new PlayerInputActions();
        playerInputActions.StarMap.Enable();
    }

    public float GetAccelerating()
    {
        float input = playerInputActions.StarMap.Accelerate.ReadValue<float>();
        return input;
    }

    public float GetRotateRight()
    {
        float input = playerInputActions.StarMap.RotateRight.ReadValue<float>();
        return input;
    }

    public float GetRotateLeft()
    {
        float input = playerInputActions.StarMap.RotateLeft.ReadValue<float>();
        return input;
    }

    private void OnDestroy()
    {
        playerInputActions.Dispose();
    }
}
