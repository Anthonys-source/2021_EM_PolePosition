using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCarInput : MonoBehaviour, ICarInputProvider, GameMainControls.IGameplayActions
{
    private GameMainControls _gameMainControls;

    public event Action<float> SteerEvent = delegate { };
    public event Action<float> AccelerateEvent = delegate { };
    public event Action<float> BrakeEvent = delegate { };

    public float SteerValue { get; private set; } = 0.0f;
    public float AccelerateValue { get; private set; } = 0.0f;
    public float BrakeValue { get; private set; } = 0.0f;

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
        if (context.performed)
        {
            SteerValue = context.ReadValue<float>();
            SteerEvent.Invoke(SteerValue);
        }
        else
        {
            SteerValue = 0.0f;
            SteerEvent.Invoke(SteerValue);
        }
    }

    public void OnAccelerate(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            AccelerateValue = context.ReadValue<float>();
            AccelerateEvent.Invoke(AccelerateValue);
        }
        else
        {
            AccelerateValue = 0.0f;
            AccelerateEvent.Invoke(AccelerateValue);
        }
    }

    public void OnBrake(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            BrakeValue = context.ReadValue<float>();
            BrakeEvent.Invoke(BrakeValue);
        }
        else
        {
            BrakeValue = 0.0f;
            BrakeEvent.Invoke(BrakeValue);
        }
    }
}
