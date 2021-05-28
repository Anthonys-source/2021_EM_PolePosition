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
        Camera.main.gameObject.SetActive(false);
    }
}
