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
    public float CurrentLapTime;
    public float CurrentRaceTime;
    public int LastCheckpoint;
    //Saber la posicion y rotación que debe tener el jugador cuando vuelque o se salte un checkpoint o vaya en direccion contraria y spawnee
    public Vector3 spawnPos;
    public Quaternion spawnRot;
    public bool playerReady = false;
    //Tiempos por vuelta del jugador
    public List<float> times;
}