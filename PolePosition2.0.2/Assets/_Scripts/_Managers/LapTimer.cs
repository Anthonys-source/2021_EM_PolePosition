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

    [Server]
    private void saveCurrentTime()
    {
        if (scriptManager.maxLaps + 1 == playerData.CurrentLap)
        {
            Debug.Log("CARRERA TERMINADA");
            scriptManager.playerTimes[this.GetComponent<PlayerInfo>().ID][playerData.CurrentLap - 1] = playerData.CurrentLapTime;
            Debug.Log("Tiempo de vuelta: " + playerData.CurrentLapTime);
            start = false;
            return;
        }
        if (playerData.CurrentLap == 0)
        {
            start = true;
            playerData.CurrentLap++;
            return;
        }
        else
        {
            scriptManager.playerTimes[this.GetComponent<PlayerInfo>().ID][playerData.CurrentLap - 1] = playerData.CurrentLapTime;
            Debug.Log("Tiempo de vuelta " + (playerData.CurrentLap - 1) + " : " + playerData.CurrentLapTime);
            playerData.CurrentLapTime = 0;
            start = true;
        }
    }
}
