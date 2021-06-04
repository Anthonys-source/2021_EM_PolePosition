using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class CheckCrash : NetworkBehaviour
{
    /*public Vector3 spawnPos;
    public Quaternion spawnRot;*/
    private GameObject manager;
    private PolePositionManager scriptManager;
    private float maxTime = 2;
    float actTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (isServer)
        {
            /*spawnPos = this.transform.position;
            spawnRot = this.transform.rotation;*/
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
        if(Mathf.Abs(Vector3.Angle(dir, Vector3.up)) > 40)
        {
            Debug.Log($"Has volcado {dir}");
            return true;
        }
        return false;
    }

    [Server]
    public void Respawn()
    {
        /*this.transform.position = spawnPos;
        this.transform.rotation = spawnRot;*/
        this.transform.position = scriptManager.playersList[this.GetComponent<PlayerInfo>().ID].spawnPos;
        this.transform.rotation = scriptManager.playersList[this.GetComponent<PlayerInfo>().ID].spawnRot;
    }
}
