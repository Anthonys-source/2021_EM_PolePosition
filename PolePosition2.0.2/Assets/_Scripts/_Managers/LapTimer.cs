using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Game.UI;

public class LapTimer : NetworkBehaviour
{
    private GameObject manager;
    private PolePositionManager scriptManager;
    private PlayerInfo playerData;
    private bool start;

    // Start is called before the first frame update
    void Start()
    {
        if (isServer)
        {
            manager = GameObject.FindGameObjectWithTag("MainManager");
            scriptManager = manager.GetComponent<PolePositionManager>();
            lock (scriptManager.playersListLock)
            {
                playerData = scriptManager.playersList[this.GetComponent<PlayerInfo>().ID];
            }
            start = false;
        }
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        //Cuando inicia la vuelta, se inicia el temporizador y se muestra el valor actualizado en la interfaz
        if (!isServer)
            return;
        if (start)
        {
            playerData.CurrentLapTime += Time.deltaTime;
            playerData.CurrentRaceTime += Time.deltaTime;
            RpcUpdateTimeGUI(playerData.CurrentLapTime, playerData.CurrentRaceTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isServer)
            return;
        //Si el jugador llega a la linea de meta, se guarda el tiempo
        if (other.gameObject.tag == "FinishLine")
        {
            saveCurrentTime();
        }
    }

    //Para guardar los tiempos por vuelta y finalizar la carrera
    [Server]
    private void saveCurrentTime()
    {
        //Si ya ha dado la ultima vuelta
        if (scriptManager.maxLaps + 1 == playerData.CurrentLap)
        {
            playerData.times.Add(playerData.CurrentLapTime);

            // Reset laps for next connection
            scriptManager.RpcResetLapGUI();
            scriptManager.RpcResetTimeGUI();
            scriptManager.RpcResetSpeedGUI();

            // Reset Race
            PolePositionManager.instance.FinishRace();

            start = false;
            return;
        }
        //Si todavía está en la pole al inicio de la carrera, pone el valor inicial de la carrera en interfaz y actualiza el conteo de vueltas
        if (playerData.CurrentLap == 0)
        {
            start = true;
            playerData.CurrentLap++;
            RpcUpdateLapGUI(playerData.CurrentLap, scriptManager.maxLaps);
            return;
        }
        //Al finalizar una vuelta intermedia, añade el tiempo de la vuelta a la lista de tiempos del jugador y actualiza la interfaz de las vueltas
        //reseteando el tiempo de la vuelta actual
        else
        {
            playerData.times.Add(playerData.CurrentLapTime);
            RpcUpdateLapGUI(playerData.CurrentLap, scriptManager.maxLaps);
            playerData.CurrentLapTime = 0;
            start = true;
        }
    }



    [ClientRpc]
    private void RpcUpdateLapGUI(int currentLap, int maxLaps)
    {
        if (isLocalPlayer)
        {
            UIManager.instance.UpdateLaps(currentLap, maxLaps);
        }
    }

    [ClientRpc]
    private void RpcUpdateTimeGUI(float time, float raceTime)
    {
        if (isLocalPlayer)
        {
            UIManager.instance.UpdateLapTime(time);
            UIManager.instance.UpdateRaceTime(raceTime);
        }
    }
}
