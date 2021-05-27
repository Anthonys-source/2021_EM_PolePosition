using System;
using System.Text.RegularExpressions;
using Mirror;
using UnityEngine;
using Random = System.Random;

/*
	Documentation: https://mirror-networking.com/docs/Guides/NetworkBehaviour.html
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkBehaviour.html
*/

public class SetupPlayer : NetworkBehaviour
{
    [SyncVar] private int _id;
    [SyncVar(hook = nameof(HandleNameUpdate))] private string _name;
    [SyncVar(hook = nameof(HandleCarColorUpdate))] private int _carColorID;

    private UIManager _uiManager;
    private MyNetworkManager _networkManager;
    private PlayerController _playerController;
    private PlayerInfo _playerInfo;
    private PolePositionManager _polePositionManager;

    #region Start & Stop Callbacks

    /// <summary>
    /// This is invoked for NetworkBehaviour objects when they become active on the server.
    /// <para>This could be triggered by NetworkServer.Listen() for objects in the scene, or by NetworkServer.Spawn() for objects that are dynamically created.</para>
    /// <para>This will be called for objects on a "host" as well as for object on a dedicated server.</para>
    /// </summary>
    public override void OnStartServer()
    {
        base.OnStartServer();
        _id = NetworkServer.connections.Count - 1;
    }

    /// <summary>
    /// Called on every NetworkBehaviour when it is activated on a client.
    /// <para>Objects on the host have this function called, as there is a local client on the host. The values of SyncVars on object are guaranteed to be initialized correctly with the latest state from the server when this function is called on the client.</para>
    /// </summary>
    public override void OnStartClient()
    {
        base.OnStartClient();
        CmdSetPlayerName(_uiManager.EnteredPlayerName);
        CmdSetCarColor(_uiManager.EnteredCarColorID);
        _playerInfo.ID = _id;
        _playerInfo.CurrentLap = 0;
        _polePositionManager.AddPlayer(_playerInfo);
    }

    /// <summary>
    /// Called when the local player object has been set up.
    /// <para>This happens after OnStartClient(), as it is triggered by an ownership message from the server. This is an appropriate place to activate components or functionality that should only be active for the local player, such as cameras and input.</para>
    /// </summary>
    public override void OnStartLocalPlayer()
    {
        _playerController.enabled = true;
        _playerController.OnSpeedChangeEvent += OnSpeedChangeEventHandler;
        ConfigureCamera();
    }

    #endregion

    #region Set Name Methods
    [Server]
    public void SetName(string name)
    {
        if (name.Length >= 2 && name.Length <= 9 && Regex.IsMatch(name, @"^[A-Za-z0-9]+$"))
        {
            _name = name;
        }
        else
        {
            _name = "Driver " + _playerInfo.ID;
        }
    }

    private void HandleNameUpdate(string oldName, string newName)
    {
        _playerInfo.Name = newName;
    }

    [Command]
    public void CmdSetPlayerName(string name)
    {
        SetName(name);
    }
    #endregion

    #region Set Car Color Methods
    [Server]
    public void SetCarColor(int colorID)
    {
        if (colorID >= 0 && colorID <= 3)
        {
            _carColorID = colorID;
            _playerInfo.CarColorID = _carColorID;
        }
        else
        {
            _carColorID = 0;
            _playerInfo.CarColorID = _carColorID;
        }
    }

    private void HandleCarColorUpdate(int oldID, int newID)
    {
        _playerInfo.CarColorID = newID;
    }

    [Command]
    public void CmdSetCarColor(int carColorID)
    {
        SetCarColor(carColorID);
    }
    #endregion

    private void Awake()
    {
        _playerInfo = GetComponent<PlayerInfo>();
        _playerController = GetComponent<PlayerController>();
        _networkManager = FindObjectOfType<MyNetworkManager>();
        _polePositionManager = FindObjectOfType<PolePositionManager>();
        _uiManager = FindObjectOfType<UIManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    void OnSpeedChangeEventHandler(float speed)
    {
        _uiManager.UpdateSpeed((int)speed * 5); // 5 for visualization purpose (km/h)
    }

    void ConfigureCamera()
    {
        if (Camera.main != null) Camera.main.gameObject.GetComponent<CameraController>().m_Focus = this.gameObject;
    }
}