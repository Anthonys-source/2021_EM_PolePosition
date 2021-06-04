using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MyNetworkManager : NetworkManager
{
    private UIManager ui;
    public override void Awake()
    {
        base.Awake();
        ui = UIManager.instance;
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        ui.ActivateInGameHUD();
    }
}
