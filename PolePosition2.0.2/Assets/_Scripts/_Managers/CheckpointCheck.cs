﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CheckpointCheck : NetworkBehaviour
{

    public int id;
    private GameObject manager;
    private PolePositionManager scriptManager;
    public int lastIndex; //El indice del ultimo checkpoint en la lista de checkpoints
    private PlayerInfo player;
    private bool respawn;
    private float maxTime = 2;
    private float actTime = 0;
    

    // Start is called before the first frame update
    void Awake()
    {
        manager = GameObject.FindGameObjectWithTag("MainManager");
        scriptManager = manager.GetComponent<PolePositionManager>();
        respawn = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isServer)
            return;
        if (respawn)
        {
            actTime += Time.deltaTime;
        }
        if(actTime > maxTime)
        {
            actTime = 0;
            player.gameObject.transform.position = player.spawnPos;
            player.gameObject.transform.rotation = player.spawnRot;
            respawn = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("CHECKPOINT!");
        int idCar = other.gameObject.GetComponent<PlayerInfo>().ID;
        Check(idCar);
    }

    [Server]
    private void Check(int idCar)
    {
        player = scriptManager.playersList[idCar];
        if (id == player.LastCheckpoint+1)
        {
            Debug.Log("Vas bien");
            player.LastCheckpoint = id;
            player.spawnPos = scriptManager.checkpointList[player.LastCheckpoint].transform.position;
            player.spawnRot = scriptManager.checkpointList[player.LastCheckpoint].transform.rotation;
        }
        else if (id == 0 && (player.LastCheckpoint == scriptManager.checkpointList[lastIndex].GetComponent<CheckpointCheck>().id))
        {
            Debug.Log("VUELTA COMPLETADA");
            player.LastCheckpoint = id;
        }
        else if(id < player.LastCheckpoint /*|| id == player.LastCheckpoint*/)
        {
            Debug.Log("Estás yendo al revés");
            respawn = true;
        }
    }
}
