﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Mirror;
using UnityEngine;
using System.Linq;
using Game.UI;

public class PolePositionManager : NetworkBehaviour
{
    // Singleton Instance
    public static PolePositionManager instance;

    // Race State
    public bool raceStarted = false;
    public int minPlayersReady = 1;
    public int maxPlayers = 4;

    // Player List
    [SerializeField] public List<PlayerInfo> playersList = new List<PlayerInfo>(4);
    public List<PlayerInfo> playerLeaderboard = new List<PlayerInfo>(4);
    public object playersListLock = new object();

    // Checkpoints
    public GameObject checkpointManager;
    public List<GameObject> checkpointList = new List<GameObject>();

    // Laps and LapTime
    public int maxLaps;

    // Leaderboard Update Period
    [SerializeField] private float leaderboardUpdateTime = 0.1f;
    private float timeSinceLeaderboardUpdate = 0.0f;

    // Dependencies
    private MyNetworkManager networkManager;
    private UIManager uiManager;
    private CircuitController circuitController;

    // Debug
#if UNITY_EDITOR
    private GameObject[] debuggingSpheres;
#endif

    // Original Camera
    public Vector3 originalCameraPos;
    public Quaternion originalCameraRot;

    private void Awake()
    {
        // Singleton Declaration
        if (instance == null)
            instance = this;
        else
            Debug.LogWarning("There is more than one " + nameof(PolePositionManager));

        //Guardar todos los checkpoints de la carrera
        for (int i = 0; i < checkpointManager.transform.childCount; i++)
        {
            checkpointList.Add(checkpointManager.transform.GetChild(i).gameObject);
            checkpointList[i].GetComponent<CheckpointCheck>().id = i;
        }

        //Para saber cual es el ultimo checkpoint y usarlo cuando se cuenten las vueltas
        checkpointList[0].GetComponent<CheckpointCheck>().lastIndex = checkpointManager.transform.childCount - 1;

        //Guardar los valores de la camara en el menu del inicio
        Camera camera = Camera.main;
        originalCameraPos = camera.transform.position;
        originalCameraRot = camera.transform.rotation;

        //Doing this limits us to only having one component of each type per scene
        if (networkManager == null) networkManager = FindObjectOfType<MyNetworkManager>();
        if (circuitController == null) circuitController = FindObjectOfType<CircuitController>();
        if (uiManager == null) uiManager = FindObjectOfType<UIManager>();

        //Initialize time since update to the update time
        //so the first update in the client happens immediatly
        timeSinceLeaderboardUpdate = leaderboardUpdateTime;

#if UNITY_EDITOR
        //Debugging positions in race
        debuggingSpheres = new GameObject[networkManager.maxConnections];
        for (int i = 0; i < networkManager.maxConnections; ++i)
        {
            debuggingSpheres[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            debuggingSpheres[i].GetComponent<SphereCollider>().enabled = false;
        }
#endif
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
    public void CheckPlayersReady()
    {
        int playersReady = 0;

        lock (playersListLock)
        {
            foreach (PlayerInfo player in playersList)
            {
                if (player.playerReady)
                    playersReady++;
            }
        }

        if (playersReady >= minPlayersReady)
        {
            StartCoroutine(RaceStartTimer());
            RpcActivateRaceCountdown();
        }
    }

    private IEnumerator RaceStartTimer()
    {
        yield return new WaitForSeconds(3.0f);
        raceStarted = true;
    }

    //Se ejecuta en el cliente que se queda solo en la carrera cuando el resto de jugadores se van
    [Server]
    public void FinishRace()
    {
        raceStarted = false;

        //Rellena la tabla de clasificación con los resultados
        string[] aux2 = UIManager.instance.GetComponent<UIManager>().FillFinalLeaderboard();
        RpcUpdateFinalLeaderboard(aux2);
        ShowFinalLeaderboard();

        //Resetea los valores de la interfaz para la siguiente carrera
        RpcResetLapGUI();
        RpcResetTimeGUI();
        RpcResetSpeedGUI();
    }

    [ClientRpc]
    public void RpcResetLapGUI()
    {
        UIManager.instance.UpdateLaps(0, maxLaps);
    }

    [ClientRpc]
    public void RpcResetTimeGUI()
    {
        UIManager.instance.UpdateLapTime(0);
        UIManager.instance.UpdateRaceTime(0);
    }

    [ClientRpc]
    public void RpcResetSpeedGUI()
    {
        UIManager.instance.UpdateSpeed(0);
    }

    [ClientRpc]
    private void RpcUpdateFinalLeaderboard(string[] data)
    {
        for (int i = 0; i < 4; i++)
        {
            //Recibe la cadena con los resultados de los jugadores y va separando los datos y metiendolos en el texto correspondiente de la interfaz
            LeaderboardUI l = UIManager.instance.finalLeaderboardUI;
            if (i < data.Length)
            {
                string[] aux = data[i].Split('/');
                l.textsNm[i].text = aux[0];
                l.textsPos[i].text = aux[1];

                //Si no hay un valor de mejor vuelta para el jugador, muestra N/A
                if (aux[2] != "-1")
                    l.textsTm[i].text = aux[2];
                else
                    l.textsTm[i].text = "N/A";
            }
            else
            {
                l.textsNm[i].text = "";
                l.textsPos[i].text = "";
                l.textsTm[i].text = "";
            }
        }
    }

    //Resetea la posicion de la camara para devolver al jugador al fondo del menú principal y muestra la tabla de clasificación
    [ClientRpc]
    public void ShowFinalLeaderboard()
    {
        Camera.main.transform.position = originalCameraPos;
        Camera.main.transform.rotation = originalCameraRot;

        Camera.main.GetComponent<CameraController>().m_Focus = null;

        UIManager.instance.GetComponent<UIManager>().ActivateFinalLeaderboard();
        //Desconecta al cliente del servidor 
        NetworkManager.singleton.StopClient();
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
            //Si solo queda un jugador en partida, esta termina
            if (playersList.Count <= 1 && raceStarted)
            {
                FinishRace();
            }
        }
    }

    //Activar la cuenta atras de inicio de carrera
    [ClientRpc]
    public void RpcActivateRaceCountdown()
    {
        UIManager.instance.ActivateInGameHUD();
        UIManager.instance.inGameUI.StartRaceCountdown();
    }
    #region Update Leaderboard

    [Server]
    public void UpdateRaceProgress()
    {
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

        lock (playersListLock)
        {
            for (int i = 0; i < playerLeaderboard.Count; i++)
            {
                playerLeaderboard[i].CurrentPosition = i;
            }
        }

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

#if UNITY_EDITOR
        //Para borrar las esferas en las builds, hacemos que estas solo estén presentes en el editor
        this.debuggingSpheres[id].transform.position = carProj;
#endif

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