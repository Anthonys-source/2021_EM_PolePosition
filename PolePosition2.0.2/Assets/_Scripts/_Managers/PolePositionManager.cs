using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Mirror;
using UnityEngine;
using System.Linq;

public class PolePositionManager : NetworkBehaviour
{
    private int numPlayers;
    [SerializeField] public List<PlayerInfo> playersList = new List<PlayerInfo>(4);
    private object playersListLock = new object();
    public GameObject checkpointManager;
    public List<GameObject> checkpointList = new List<GameObject>();

    public int maxLaps;
    public List<float[]> playerTimes = new List<float[]>(4);

    [SerializeField] private float leaderboardUpdateTime = 0.1f;
    private float timeSinceLeaderboardUpdate = 0.0f;

    private MyNetworkManager networkManager;
    private UIManager uiManager;
    private CircuitController circuitController;

    private GameObject[] debuggingSpheres;
    public GameObject camera;
    public Vector3 originalCameraPos;
    public Quaternion originalCameraRot;
    
    private void Awake()
    {
        //Guardar todos los chekcpoints de la carrera
        for(int i = 0; i<checkpointManager.transform.childCount; i++)
        {
            checkpointList.Add(checkpointManager.transform.GetChild(i).gameObject);
            checkpointList[i].GetComponent<CheckpointCheck>().id = i;
        }
        
        //Para saber cual es el ultimo checkpoint y usarlo cuando se cuenten las vueltas
        checkpointList[0].GetComponent<CheckpointCheck>().lastIndex = checkpointManager.transform.childCount - 1;
        
        //Instanciar los arrays para los tiempos por vuelta de cada jugador
        for(int i = 0; i<4; i++)
        {
            playerTimes.Add(new float[maxLaps+1]);
        }

        //Guardar los valores de la camara en el menu del inicio
        camera = GameObject.FindGameObjectWithTag("MainCamera");
        originalCameraPos = camera.transform.position;
        originalCameraRot = camera.transform.rotation;

        //Doing this limits us to only having one component of each type per scene
        if (networkManager == null) networkManager = FindObjectOfType<MyNetworkManager>();
        if (circuitController == null) circuitController = FindObjectOfType<CircuitController>();
        if (uiManager == null) uiManager = FindObjectOfType<UIManager>();

        //Initialize time since update to the update time
        //so the first update in the client happens immediatly
        timeSinceLeaderboardUpdate = leaderboardUpdateTime;

        //Debugging positions in race
        debuggingSpheres = new GameObject[networkManager.maxConnections];
        for (int i = 0; i < networkManager.maxConnections; ++i)
        {
            debuggingSpheres[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            debuggingSpheres[i].GetComponent<SphereCollider>().enabled = false;
        }
    }

    private void Update()
    {
        if (playersList.Count == 0)
            return;

        if (isServer)
        {
            UpdateRaceProgress();
            timeSinceLeaderboardUpdate += Time.deltaTime;
        }
    }

    [Server]
    public void AddPlayer(PlayerInfo player)
    {
        lock (playersListLock)
        {
            playersList.Add(player);
        }
    }

    [Server]
    public void RemovePlayer(PlayerInfo player)
    {
        lock (playersListLock)
        {
            playersList.Remove(player);
        }
    }

    #region Update Leaderboard

    [Server]
    public void UpdateRaceProgress()
    {
        List<PlayerInfo> playerLeaderboard;
        float[] arcLengths;

        lock (playersListLock)
        {
            // Update car arc-lengths
            arcLengths = new float[playersList.Count];

            for (int i = 0; i < playersList.Count; ++i)
            {
                arcLengths[i] = ComputeCarArcLength(i);
            }

            // Copying the list every frame might be explensive but its good enough for now
            playerLeaderboard = playersList.ToList<PlayerInfo>();
        }

        playerLeaderboard.Sort(new PlayerInfoComparer(arcLengths));

        string[] newLeaderboardNames = new string[playerLeaderboard.Count];
        for (int i = 0; i < playerLeaderboard.Count; i++)
        {
            newLeaderboardNames[i] = playerLeaderboard[i].PlayerName;
        }

        //Check if the leaderboard should be updated in the client
        if (timeSinceLeaderboardUpdate >= leaderboardUpdateTime)
        {
            RpcUpdateUILeaderboard(newLeaderboardNames);
            timeSinceLeaderboardUpdate = 0.0f;
        }
    }

    [Server]
    private float ComputeCarArcLength(int id)
    {
        // Compute the projection of the car position to the closest circuit 
        // path segment and accumulate the arc-length along of the car along
        // the circuit.
        Vector3 carPos = this.playersList[id].transform.position;

        int segIdx;
        float carDist;
        Vector3 carProj;

        float minArcL =
            this.circuitController.ComputeClosestPointArcLength(carPos, out segIdx, out carProj, out carDist);

        this.debuggingSpheres[id].transform.position = carProj;

        if (this.playersList[id].CurrentLap == 0)
        {
            minArcL -= circuitController.CircuitLength;
        }
        else
        {
            minArcL += circuitController.CircuitLength *
                       (playersList[id].CurrentLap - 1);
        }

        return minArcL;
    }

    [ClientRpc]
    public void RpcUpdateUILeaderboard(string[] newLeaderboardNames)
    {
        uiManager.UpdateLeaderboardNames(newLeaderboardNames);
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