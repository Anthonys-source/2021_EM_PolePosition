using System.Collections;
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
    void Start()
    {
        if (isServer)
        {
            manager = GameObject.FindGameObjectWithTag("MainManager");
            scriptManager = manager.GetComponent<PolePositionManager>();
            respawn = false;
        }
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
            //Mover al jugador al ultimo checkpoint correcto por el que pasó
            player.gameObject.transform.position = player.spawnPos;
            player.gameObject.transform.rotation = player.spawnRot;
            respawn = false;
        }
    }

    
    private void OnTriggerEnter(Collider other)
    {
        if (!isServer)
            return;
        int idCar = other.gameObject.GetComponent<PlayerInfo>().ID;
        Check(idCar);
    }

    [Server]
    private void Check(int idCar)
    {
        lock (scriptManager.playersListLock)
        {
            player = scriptManager.playersList[idCar];
        }
        //Vas pasando correctamente los checkpoints
        if (id == player.LastCheckpoint+1)
        {
            player.LastCheckpoint = id;
            //Guardar la posicion y rotacion del checkpoint para cuando haya que respawnear al jugador
            player.spawnPos = scriptManager.checkpointList[player.LastCheckpoint].transform.position;
            player.spawnRot = scriptManager.checkpointList[player.LastCheckpoint].transform.rotation;
        }
        //Ya has llegado a la linea de meta
        else if (id == 0 && (player.LastCheckpoint == scriptManager.checkpointList[lastIndex].GetComponent<CheckpointCheck>().id))
        {
            player.CurrentLap++;
            //El ultimo checkpoint por el que pasa el jugador vuelve a ser el inicio
            player.LastCheckpoint = id;
        }
        //Estas yendo en direccion contraria, hay que respawnear
        else if(id < player.LastCheckpoint)
        {
            respawn = true;
        }
        //Te has saltado un checkpoint, hay que respawnear
        else if(id != player.LastCheckpoint)
        {
            respawn = true;
        }
    }
}
