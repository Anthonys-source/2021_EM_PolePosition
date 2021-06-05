using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCarInput : MonoBehaviour, ICarInputProvider, GameMainControls.IGameplayActions
{
    private GameMainControls _gameMainControls;

    public float SteerValue
    {
        get { return steerValue; }
        private set
        {
            steerValue = value;
            SteerEvent.Invoke(steerValue);
        }
    }
    public float AccelerateValue
    {
        get { return accelerateValue; }
        private set
        {
            accelerateValue = value;
            AccelerateEvent.Invoke(accelerateValue);
        }
    }
    public float HandbrakeValue
    {
        get { return handbrakeValue; }
        private set
        {
            handbrakeValue = value;
            HandbrakeEvent.Invoke(handbrakeValue);
        }
    }

    private float steerValue = 0.0f;
    private float accelerateValue = 0.0f;
    private float handbrakeValue = 0.0f;

    public event Action<float> SteerEvent = delegate { };
    public event Action<float> AccelerateEvent = delegate { };
    public event Action<float> HandbrakeEvent = delegate { };

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
