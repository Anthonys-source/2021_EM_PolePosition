using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class FinishRace : NetworkBehaviour
{
    private GameObject camera;
    private GameObject uiMan;

    private void Start()
    {
        camera = GameObject.FindGameObjectWithTag("MainCamera");
        uiMan = GameObject.FindGameObjectWithTag("UIManager");
    }

    //Cuando termina la carrera un jugador, vuelve al menu de inicio
    [ClientRpc]
    public void BackToMenu(PolePositionManager scriptManager)
    {
        camera.transform.position = scriptManager.originalCameraPos;
        camera.transform.rotation = scriptManager.originalCameraRot;
        camera.GetComponent<CameraController>().m_Focus = null;
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
