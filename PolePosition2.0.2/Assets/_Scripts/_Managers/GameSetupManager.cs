using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetupManager : MonoBehaviour
{
    [SerializeField] private MyNetworkManager networkManager;

    public void StartHost()
    {
        networkManager.StartHost();
    }

    public void StartClient(string serverIP)
    {
        networkManager.StartClient();
        networkManager.networkAddress = serverIP;

        // This will be replaced by the LobbyUI when the client has succesfuly connected
        UIManager.instance.ActivateConnectingToServerUI();
    }

    public void StopClient()
    {
        networkManager.StopClient();
    }

    public void StartServer()
    {
        networkManager.StartServer();

        //Server optimizations
        Camera camera = Camera.main;
        camera.gameObject.SetActive(false);
        camera.transform.position = new Vector3(9999.0f, 9999.0f, 9999.0f);
    }
}
