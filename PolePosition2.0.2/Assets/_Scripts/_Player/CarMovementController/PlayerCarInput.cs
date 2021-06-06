using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Translates the Player input into Car Input
/// </summary>
public class PlayerCarInput : MonoBehaviour, ICarInputProvider, GameMainControls.IGameplayActions
{
    private GameMainControls _gameMainControls;

    public float SteerValue { get; private set; } = 0.0f;
    public float AccelerateValue { get; private set; } = 0.0f;
    public float HandbrakeValue { get; private set; } = 0.0f;

    private void OnEnable()
    {
        if (_gameMainControls == null)
        {
            _gameMainControls = new GameMainControls();
            _gameMainControls.Gameplay.SetCallbacks(this);
        }

        _gameMainControls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        _gameMainControls.Gameplay.Disable();
    }

    public void OnSteer(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            SteerValue = context.ReadValue<float>();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            SteerValue = 0.0f;
        }
    }

    public void OnAccelerate(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            AccelerateValue = context.ReadValue<float>();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            AccelerateValue = 0.0f;
        }
    }

    public void OnHandbrake(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            HandbrakeValue = context.ReadValue<float>();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            HandbrakeValue = 0.0f;
        }
    }
}
