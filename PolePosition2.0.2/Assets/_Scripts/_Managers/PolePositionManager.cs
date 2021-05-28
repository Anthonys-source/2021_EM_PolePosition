using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Mirror;
using UnityEngine;
using System.Linq;

public class PolePositionManager : NetworkBehaviour
{
    public int numPlayers;
    private MyNetworkManager _networkManager;
    private UIManager _uiManager;
    private CircuitController _circuitController;

    private readonly List<PlayerInfo> _players = new List<PlayerInfo>(4);

    private GameObject[] _debuggingSpheres;

    private void Awake()
    {
        //Doing this limits us to only having one component of each type per scene
        if (_networkManager == null) _networkManager = FindObjectOfType<MyNetworkManager>();
        if (_circuitController == null) _circuitController = FindObjectOfType<CircuitController>();
        if (_uiManager == null) _uiManager = FindObjectOfType<UIManager>();

        //Debugging positions in race
        _debuggingSpheres = new GameObject[_networkManager.maxConnections];
        for (int i = 0; i < _networkManager.maxConnections; ++i)
        {
            _debuggingSpheres[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            _debuggingSpheres[i].GetComponent<SphereCollider>().enabled = false;
        }
    }

    private void Update()
    {
        if (_players.Count == 0)
            return;

        if (isServer)
            UpdateRaceProgress();
    }

    [Server]
    public void AddPlayer(PlayerInfo player)
    {
        _players.Add(player);
    }

    [Server]
    public void RemovePlayer(PlayerInfo player)
    {
        _players.Remove(player);
    }

    #region Update Leaderboard

    [Server]
    public void UpdateRaceProgress()
    {
        // Update car arc-lengths
        float[] arcLengths = new float[_players.Count];

        for (int i = 0; i < _players.Count; ++i)
        {
            arcLengths[i] = ComputeCarArcLength(i);
        }

        // Copying the list every frame might be explensive but its good enough for now
        List<PlayerInfo> playerLeaderboard = _players.ToList<PlayerInfo>();

        playerLeaderboard.Sort(new PlayerInfoComparer(arcLengths));

        string[] newLeaderboardNames = new string[playerLeaderboard.Count];
        for (int i = 0; i < playerLeaderboard.Count; i++)
        {
            newLeaderboardNames[i] = playerLeaderboard[i].PlayerName;
        }

        //This is called just to update the leaderboard names in the server
        //It's just to see the leaderboard in the server game view
        _uiManager.UpdateLeaderboardNames(newLeaderboardNames);

        RpcUpdateUILeaderboard(newLeaderboardNames);
    }

    [Server]
    private float ComputeCarArcLength(int id)
    {
        // Compute the projection of the car position to the closest circuit 
        // path segment and accumulate the arc-length along of the car along
        // the circuit.
        Vector3 carPos = this._players[id].transform.position;

        int segIdx;
        float carDist;
        Vector3 carProj;

        float minArcL =
            this._circuitController.ComputeClosestPointArcLength(carPos, out segIdx, out carProj, out carDist);

        this._debuggingSpheres[id].transform.position = carProj;

        if (this._players[id].CurrentLap == 0)
        {
            minArcL -= _circuitController.CircuitLength;
        }
        else
        {
            minArcL += _circuitController.CircuitLength *
                       (_players[id].CurrentLap - 1);
        }

        return minArcL;
    }

    [ClientRpc]
    public void RpcUpdateUILeaderboard(string[] newLeaderboardNames)
    {
        _uiManager.UpdateLeaderboardNames(newLeaderboardNames);
    }

    private class PlayerInfoComparer : Comparer<PlayerInfo>
    {
        float[] _arcLengths;

        public PlayerInfoComparer(float[] arcLengths)
        {
            _arcLengths = arcLengths;
        }

        public override int Compare(PlayerInfo x, PlayerInfo y)
        {
            if (_arcLengths[x.ID] < _arcLengths[y.ID])
                return 1;
            else return -1;
        }
    }

    #endregion
}