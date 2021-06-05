using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LapTimer : NetworkBehaviour
{
    private GameObject manager;
    private PolePositionManager scriptManager;
    private PlayerInfo playerData;
    private bool start;
    private float counter;
    private GameObject aux;
    //private GameObject uiMan;
    // Start is called before the first frame update
    void Start()
    {
        if (isServer)
        {
            manager = GameObject.FindGameObjectWithTag("MainManager");
            scriptManager = manager.GetComponent<PolePositionManager>();
            aux = GameObject.FindGameObjectWithTag("FinishRace");
            lock (scriptManager.playersListLock)
            {
                playerData = scriptManager.playersList[this.GetComponent<PlayerInfo>().ID];
            }
            //uiMan = GameObject.FindGameObjectWithTag("UIManager");
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
        //Cuando inicia la vuelta, se inicia el temporizador
        if (!isServer)
            return;
        if (start)
        {
            playerData.CurrentLapTime += Time.deltaTime;
            playerData.CurrentRaceTime += Time.deltaTime;
            RpcUpdateTimeGUI(playerData.CurrentLapTime,playerData.CurrentRaceTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isServer)
            return;

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
            Debug.Log("CARRERA TERMINADA");
            scriptManager.playerTimes[this.GetComponent<PlayerInfo>().ID][playerData.CurrentLap - 1] = playerData.CurrentLapTime;

            // Reset laps for next connection
            RpcUpdateLapGUI(0, scriptManager.maxLaps);

            // Reset Race
            PolePositionManager.instance.raceStarted = false;

            aux.GetComponent<FinishRace>().BackToMenu();
            //aux.GetComponent<FinishRace>().DisconnectAllPlayers(scriptManager);

            //scriptManager.camera.GetComponent<FinishRace>().BackToMenuClient(scriptManager);
            Debug.Log("Tiempo de vuelta: " + playerData.CurrentLapTime);
            start = false;
            return;
        }
        //Si todavía está en la pole al inicio de la carrera
        if (playerData.CurrentLap == 0)
        {
            start = true;
            playerData.CurrentLap++;
            RpcUpdateLapGUI(playerData.CurrentLap, scriptManager.maxLaps);
            return;
        }
        //Al finalizar una vuelta intermedia
        else
        {
            scriptManager.playerTimes[this.GetComponent<PlayerInfo>().ID][playerData.CurrentLap - 1] = playerData.CurrentLapTime;
            Debug.Log("Tiempo de vuelta " + (playerData.CurrentLap - 1) + " : " + playerData.CurrentLapTime);
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
