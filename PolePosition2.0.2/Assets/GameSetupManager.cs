using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetupManager : MonoBehaviour
{
    [SerializeField] private MyNetworkManager m_NetworkManager;

    public void StartHost()
    {
        m_NetworkManager.StartHost();
    }

    public void StartClient(string serverIP)
    {
        m_NetworkManager.StartClient();
        m_NetworkManager.networkAddress = serverIP;
    }

    public void StartServer()
    {
        m_NetworkManager.StartServer();

        //Server optimizations
        Camera camera = Camera.main;
        camera.gameObject.SetActive(false);
        camera.transform.position = new Vector3(9999.0f, 9999.0f, 9999.0f);
    }
}
