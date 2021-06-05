using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MyNetworkManager : NetworkManager
{
    public override void Awake()
    {
        base.Awake();
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        UIManager.instance.ActivateLobbyUI();
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);

        // Should activate a menu where the client is notified that
        // there are too many players
        UIManager.instance.ActivateMainMenu();
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        PolePositionManager p = PolePositionManager.instance;
        int currentPlayerNumber = 0;

        lock (p.playersListLock)
        {
            currentPlayerNumber = p.playersList.Count;
        }

        // Check if there are free spots on the server
        if (currentPlayerNumber >= p.maxPlayers)
        {
            conn.identity.GetComponent<PlayerNetworkComponent>();
            conn.Disconnect();
        }
        else
        {
            base.OnServerConnect(conn);
            UIManager.instance.ActivateLobbyUI();
        }
    }
}
