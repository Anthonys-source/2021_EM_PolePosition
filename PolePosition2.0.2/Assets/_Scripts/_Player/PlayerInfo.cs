using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public string PlayerName;
    public int ID;
    public int CarColorID;
    public int CurrentPosition;
    public int CurrentLap;
    public int CurrentLapTime;
    public int CurrentRaceTime;
    public int LastCheckpoint;
    public Vector3 spawnPos;
    public Quaternion spawnRot;
}