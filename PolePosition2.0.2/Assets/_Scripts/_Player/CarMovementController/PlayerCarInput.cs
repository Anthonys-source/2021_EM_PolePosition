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
        get { return SteerValue; }
        private set
        {
            SteerValue = value;
            SteerEvent.Invoke(SteerValue);
        }
    }
    public float AccelerateValue
    {
        get { return AccelerateValue; }
        private set
        {
            AccelerateValue = value;
            AccelerateEvent.Invoke(AccelerateValue);
        }
    }
    public float HandbrakeValue
    {
        get { return HandbrakeValue; }
        private set
        {
            HandbrakeValue = value;
            HandbrakeEvent.Invoke(HandbrakeValue);
        }
    }

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

    //private void GenericFloatActionLogic(ref float value, Action<float> onChangeEvent, InputAction.CallbackContext context)
    //{
    //    if (context.phase == InputActionPhase.Performed)
    //    {
    //        value = context.ReadValue<float>();
    //        onChangeEvent.Invoke(value);
    //    }
    //    else if (context.phase == InputActionPhase.Canceled)
    //    {
    //        value = 0.0f;
    //        onChangeEvent.Invoke(value);
    //    }
    //}
}
