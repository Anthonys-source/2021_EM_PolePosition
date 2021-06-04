﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class FinishRace : NetworkBehaviour
{
    //Cuando termina la carrera un jugador, vuelve al menu de inicio
    [Server]
    public void BackToMenu(PolePositionManager scriptManager)
    {
        this.transform.position = scriptManager.originalCameraPos;
        this.transform.rotation = scriptManager.originalCameraRot;
        this.GetComponent<CameraController>().m_Focus = null;

        for(int i = 0; i<scriptManager.playersList.Count; i++)
        {
            Destroy(scriptManager.playersList[i].gameObject);
            scriptManager.playersList[i].gameObject.GetComponent<PlayerNetworkComponent>().OnStopServer();
        }
    }
}