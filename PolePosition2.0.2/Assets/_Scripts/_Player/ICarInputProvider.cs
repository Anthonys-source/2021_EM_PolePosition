using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICarInputProvider
{
    event Action<float> SteerEvent;
    event Action<float> AccelerateEvent;
    event Action<float> BrakeEvent;

    float SteerValue { get; }
    float AccelerateValue { get; }
    float BrakeValue { get; }
}
