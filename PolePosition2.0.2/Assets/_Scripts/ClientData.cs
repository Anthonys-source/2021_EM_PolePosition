using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains the Client data that will be sent to the server on connect
/// </summary>
public class ClientData : MonoBehaviour
{
    public string playerName;
    public int carColorID;
    public bool carReady = false;
}
