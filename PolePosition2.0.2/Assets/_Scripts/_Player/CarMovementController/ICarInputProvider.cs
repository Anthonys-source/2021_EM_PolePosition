using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface for Car Controller Inputs
/// </summary>
public interface ICarInputProvider
{
    float SteerValue { get; }
    float AccelerateValue { get; }
    float HandbrakeValue { get; }
}
