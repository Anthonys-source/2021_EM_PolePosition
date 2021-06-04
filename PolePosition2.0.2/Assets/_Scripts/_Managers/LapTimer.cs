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
    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.FindGameObjectWithTag("MainManager");
        scriptManager = manager.GetComponent<PolePositionManager>();
        playerData = scriptManager.playersList[this.GetComponent<PlayerInfo>().ID];
        start = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Cuando inicia la vuelta, se inicia el temporizador
        if (start)
        {
            playerData.CurrentLapTime += Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "FinishLine")
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
            scriptManager.camera.GetComponent<FinishRace>().BackToMenu(scriptManager);
            Debug.Log("Tiempo de vuelta: " + playerData.CurrentLapTime);
            start = false;
            return;
        }
        //Si todavía está en la pole al inicio de la carrera
        if (playerData.CurrentLap == 0)
        {
            start = true;
            playerData.CurrentLap++;
            return;
        }
        //Al finalizar una vuelta intermedia
        else
        {
            scriptManager.playerTimes[this.GetComponent<PlayerInfo>().ID][playerData.CurrentLap - 1] = playerData.CurrentLapTime;
            Debug.Log("Tiempo de vuelta " + (playerData.CurrentLap - 1) + " : " + playerData.CurrentLapTime);
            playerData.CurrentLapTime = 0;
            start = true;
        }
    }
}
