using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class CheckCrash : NetworkBehaviour
{
    public Vector3 spawnPos;
    public Quaternion spawnRot;
    private float maxTime = 2;
    float actTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        spawnPos = this.transform.position;
        spawnRot = this.transform.rotation;
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
        if(Mathf.Abs(Vector3.Angle(dir, Vector3.up)) > 85)
        {
            Debug.Log($"Has volcado {dir}");
            return true;
        }
        return false;
    }

    [Server]
    public void Respawn()
    {
        this.transform.position = spawnPos;
        this.transform.rotation = spawnRot;
    }
}
