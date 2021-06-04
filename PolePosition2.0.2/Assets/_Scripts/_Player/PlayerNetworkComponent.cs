using System;
using System.Text.RegularExpressions;
using Mirror;
using UnityEngine;

public class PlayerNetworkComponent : NetworkBehaviour
{
    private PlayerInfo _playerInfo;
    private CarColorComponent _carColorComponent;
    private PlayerController _playerController;

    private PolePositionManager _polePositionManager;
    private UIManager _uiManager;
    private ClientData clientData;

    [SyncVar] private int _id;
    [SyncVar] private int _currentLap;
    [SyncVar(hook = nameof(HandleNameUpdate))] private string _name;
    [SyncVar(hook = nameof(HandleCarColorUpdate))] private int _carColorID;
    [SyncVar] private bool _carReady;
    public object _nameLock = new object();

    private void Awake()
    {
        _playerInfo = GetComponent<PlayerInfo>();
        _carColorComponent = GetComponent<CarColorComponent>();
        _playerController = GetComponent<PlayerController>();

        _polePositionManager = FindObjectOfType<PolePositionManager>();
        _uiManager = FindObjectOfType<UIManager>();
        clientData = FindObjectOfType<ClientData>();
    }

    #region Start & Stop Mirror Callbacks

    /// <summary>
    /// This is invoked for NetworkBehaviour objects when they become active on the server.
    /// <para>This could be triggered by NetworkServer.Listen() for objects in the scene, or by NetworkServer.Spawn() for objects that are dynamically created.</para>
    /// <para>This will be called for objects on a "host" as well as for object on a dedicated server.</para>
    /// </summary>
    public override void OnStartServer()
    {
        //Setup the Player information in the server
        base.OnStartServer();
        _id = NetworkServer.connections.Count - 1;
        _currentLap = 0;
        _playerInfo.CurrentLap = 0;
        _playerInfo.ID = _id;
        _playerInfo.CurrentLap = _currentLap;
        _playerInfo.LastCheckpoint = -1;
        _polePositionManager.AddPlayer(_playerInfo);

        _playerController.enabled = true;
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        _polePositionManager.RemovePlayer(_playerInfo);
    }

    /// <summary>
    /// Called on every NetworkBehaviour when it is activated on a client.
    /// <para>Objects on the host have this function called, as there is a local client on the host. The values of SyncVars on object are guaranteed to be initialized correctly with the latest state from the server when this function is called on the client.</para>
    /// </summary>
    public override void OnStartClient()
    {
        base.OnStartClient();
    }

    /// <summary>
    /// Called when the local player object has been set up.
    /// <para>This happens after OnStartClient(), as it is triggered by an ownership message from the server. This is an appropriate place to activate components or functionality that should only be active for the local player, such as cameras and input.</para>
    /// </summary>
    public override void OnStartLocalPlayer()
    {
        //In each client this only executes in its own player car (Local Player)
        CmdSetPlayerName(clientData.playerName);
        CmdSetCarColor(clientData.carColorID);
        UIManager.instance.lobbyUI.onReadyStateChange += CmdSetPlayerReady;

        _playerController.enabled = true;
        _playerController.OnSpeedChangeEvent += OnSpeedChangeEventHandler;
        ConfigureCamera();
    }

    #endregion

    #region Set Name Methods
    [Server]
    public void SetName(string name)
    {
        //Lock the name in case the client sends multiple SetName methods
        //The client shouldnt be able to do this normally but its a security meassure
        lock (_nameLock)
        {
            if (name.Length >= 2 && name.Length <= 9 && Regex.IsMatch(name, @"^[A-Za-z0-9]+$"))
            {
                _name = name;
            }
            else
            {
                _name = "Driver " + _playerInfo.ID;
            }
            _playerInfo.PlayerName = _name;
        }
    }

    private void HandleNameUpdate(string oldName, string newName)
    {
        _playerInfo.PlayerName = newName;
    }

    [Command]
    public void CmdSetPlayerName(string name)
    {
        SetName(name);
    }
    #endregion

    [Server]
    public void SetPlayerReady(bool ready)
    {
        _carReady = ready;
        _playerInfo.playerReady = ready;
        PolePositionManager.instance.CheckPlayersReady();
    }

    [Command]
    public void CmdSetPlayerReady(bool ready)
    {
        SetPlayerReady(ready);
    }

    #region Set Car Color Methods
    [Server]
    public void SetCarColor(int colorID)
    {
        if (colorID >= 0 && colorID <= 3)
        {
            _carColorID = colorID;
        }
        else
        {
            _carColorID = 0;
        }
        _playerInfo.CarColorID = _carColorID;
    }

    private void HandleCarColorUpdate(int oldID, int newID)
    {
        _playerInfo.CarColorID = newID;
        _carColorComponent.SetCarColor(_playerInfo.CarColorID);
    }

    [Command]
    public void CmdSetCarColor(int carColorID)
    {
        SetCarColor(carColorID);
    }

    //[Command]
    //public void CmdSetPlayerReady()
    #endregion

    [Client]
    void OnSpeedChangeEventHandler(int speed)
    {
        _uiManager.UpdateSpeed(speed * 5); // 5 for visualization purpose (km/h)
    }

    [Client]
    void ConfigureCamera()
    {
        if (Camera.main != null) Camera.main.gameObject.GetComponent<CameraController>().m_Focus = this.gameObject;
    }
}