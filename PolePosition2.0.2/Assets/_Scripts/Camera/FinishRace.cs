using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class FinishRace : NetworkBehaviour
{
    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }

    //Cuando termina la carrera un jugador, vuelve al menu de inicio
    [ClientRpc]
    public void BackToMenu()
    {
        cam.transform.position = PolePositionManager.instance.originalCameraPos;
        cam.transform.rotation = PolePositionManager.instance.originalCameraRot;

        cam.GetComponent<CameraController>().m_Focus = null;

        //UIManager.instance.GetComponent<UIManager>().ActivateMainMenu();
        UIManager.instance.GetComponent<UIManager>().ActivateFinalLeaderboard();
        NetworkManager.singleton.StopClient();
        /*for (int i = 0; i < scriptManager.playersList.Count; i++)
        {
            Destroy(scriptManager.playersList[i].gameObject);
            //scriptManager.playersList[i].gameObject.GetComponent<PlayerNetworkComponent>().OnStopClient();
        }*/
    }

    [Server]
    public void DisconnectAllPlayers(PolePositionManager poleManager)
    {
        //lock (poleManager.playersListLock)
        //{
        //    for (int i = 0; i < poleManager.playersList.Count; i--)
        //    {
        //        //Destroy(scriptManager.playersList[i].gameObject);
        //        //scriptManager.playersList[i].gameObject.GetComponent<PlayerNetworkComponent>().OnStopClient();
        //        poleManager.playersList[i].gameObject.GetComponent<PlayerNetworkComponent>().connectionToClient.Disconnect();
        //    }
        //}
    }
}
