using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class CheckCrash : NetworkBehaviour
{
    private GameObject manager;
    private PolePositionManager scriptManager;
    private float maxTime = 2;
    float actTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (isServer)
        {
            manager = GameObject.FindGameObjectWithTag("MainManager");
            scriptManager = manager.GetComponent<PolePositionManager>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isServer)
            return;
        if (checkUp())
        {
            actTime += Time.deltaTime;
        }
        else
        {
            actTime = 0;
        }
        if(actTime > maxTime)
        {
            Respawn();
            actTime = 0;
        }
    }

    [Server]
    private bool checkUp()
    {
        Vector3 dir = this.transform.up;
        //Vector3 dir;
        //lock (scriptManager.playersListLock)
        //{
        //    dir = scriptManager.playersList[this.GetComponent<PlayerInfo>().ID].gameObject.transform.up;
        //}
        if (Mathf.Abs(Vector3.Angle(dir, Vector3.up)) > 40)
        {
            Debug.Log($"Has volcado {dir}");
            return true;
        }
        return false;
    }

    [Server]
    public void Respawn()
    {
        lock (scriptManager.playersListLock)
        {
            //scriptManager.playersList[this.GetComponent<PlayerInfo>().ID].gameObject.transform.position = scriptManager.playersList[this.GetComponent<PlayerInfo>().ID].spawnPos;
            //scriptManager.playersList[this.GetComponent<PlayerInfo>().ID].gameObject.transform.rotation = scriptManager.playersList[this.GetComponent<PlayerInfo>().ID].spawnRot;
            this.transform.position = scriptManager.playersList[this.GetComponent<PlayerInfo>().ID].spawnPos;
            this.transform.rotation = scriptManager.playersList[this.GetComponent<PlayerInfo>().ID].spawnRot;
        }
    }
}
