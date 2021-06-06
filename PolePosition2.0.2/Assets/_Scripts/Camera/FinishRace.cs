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

    //Cuando un jugador termina la carrera, se muestra tanto en este como en el resto de jugadores la tabla de resultados final
    [ClientRpc]
    public void ShowFinalLeaderboard()
    {
        cam.transform.position = PolePositionManager.instance.originalCameraPos;
        cam.transform.rotation = PolePositionManager.instance.originalCameraRot;

        cam.GetComponent<CameraController>().m_Focus = null;

        UIManager.instance.GetComponent<UIManager>().ActivateFinalLeaderboard();
        NetworkManager.singleton.StopClient();
    }
}
