using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class FinishRace : NetworkBehaviour
{
    private Camera cam;
    private UIManager uiMan;

    private void Start()
    {
        cam = Camera.main;
        uiMan = UIManager.instance;
    }

    //Cuando termina la carrera un jugador, vuelve al menu de inicio
    [ClientRpc]
    public void BackToMenu(PolePositionManager scriptManager)
    {
        cam.transform.position = scriptManager.originalCameraPos;
        cam.transform.rotation = scriptManager.originalCameraRot;
        cam.GetComponent<CameraController>().m_Focus = null;
        uiMan.GetComponent<UIManager>().ActivateMainMenu();
        NetworkManager.singleton.StopClient();
        /*for (int i = 0; i < scriptManager.playersList.Count; i++)
        {
            Destroy(scriptManager.playersList[i].gameObject);
            //scriptManager.playersList[i].gameObject.GetComponent<PlayerNetworkComponent>().OnStopClient();
        }*/
    }

    [Server]
    public void ErasePlayers(PolePositionManager scriptManager)
    {
        for (int i = 0; i < scriptManager.playersList.Count; i++)
        {
            Destroy(scriptManager.playersList[i].gameObject);
            //scriptManager.playersList[i].gameObject.GetComponent<PlayerNetworkComponent>().OnStopClient();
        }
        scriptManager.playersList.Clear();
    }
}
