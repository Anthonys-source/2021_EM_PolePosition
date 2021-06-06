using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class CheckCrash : NetworkBehaviour
{
    private PolePositionManager polePositionManager;
    private float maxTime = 2;
    private float currentRolledOverTime = 0;

    void Start()
    {
        if (isServer)
        {
            polePositionManager = FindObjectOfType<PolePositionManager>();
        }
    }

    void Update()
    {
        // Only Execute in Server
        if (!isServer)
            return;

        // Check if the player UP vector
        if (CheckUp())
            // If the player is rolled over advance in the timer
            currentRolledOverTime += Time.deltaTime;
        else
            // Reset the timer if its not the case
            currentRolledOverTime = 0;

        // If Given a max time the player car object its still rolled
        // respawn the player
        if (currentRolledOverTime > maxTime)
        {
            Respawn();
            currentRolledOverTime = 0;
        }
    }

    [Server]
    private bool CheckUp()
    {
        Vector3 dir = transform.up;
        if (Mathf.Abs(Vector3.Angle(dir, Vector3.up)) > 40)
        {
            return true;
        }
        return false;
    }

    [Server]
    private void Respawn()
    {
        lock (polePositionManager.playersListLock)
        {
            transform.position = polePositionManager.playersList[GetComponent<PlayerInfo>().ID].spawnPos;
            transform.rotation = polePositionManager.playersList[GetComponent<PlayerInfo>().ID].spawnRot;
        }
    }
}
