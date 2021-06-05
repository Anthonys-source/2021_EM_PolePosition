using System;
using System.Collections;
using System.Collections.Generic;
using Game.UI;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameSetupManager gameSetupManager;
    public MainMenuUI mainMenuUI;
    public ConnectingToServerUI connectingToServerUI;
    public PreGameUI preGameUI;
    public InGameUI inGameUI;
    public ServerUI serverUI;
    public LobbyUI lobbyUI;
    public LeaderboardUI leaderboardUI;

    private List<UIObject> uiElements = new List<UIObject>();

    // Singleton Instance
    public static UIManager instance;

    // Current selected game "Type" (Host,Client,Server) that the game
    // instance will try to start
    [HideInInspector] public GameTypes selectedGameType = GameTypes.Host;

    private void Awake()
    {
        // Initialize Singleton
        if (instance == null)
            instance = this;
        else
            Debug.LogWarning("There is more than one UIManager object");

        // Initialize all UIObjects
        uiElements.Add(mainMenuUI);
        uiElements.Add(connectingToServerUI);
        uiElements.Add(preGameUI);
        uiElements.Add(inGameUI);
        uiElements.Add(serverUI);
        uiElements.Add(lobbyUI);
        uiElements.Add(leaderboardUI);

        foreach (UIObject uIObject in uiElements)
        {
            uIObject.SetUIManager(this);
        }
    }

    private void Start()
    {
        ActivateMainMenu();
    }

    #region InGame UI

    public void UpdateSpeed(int speed)
    {
        inGameUI.UpdateSpeed(speed);
    }

    public void UpdateLaps(int newLap, int maxLaps)
    {
        inGameUI.UpdateLaps(newLap, maxLaps);
    }

    public void UpdateLapTime(float lapTime)
    {
        inGameUI.UpdateLapTime(lapTime);
    }

    public void UpdateRaceTime(float raceTime)
    {
        inGameUI.UpdateRaceTime(raceTime);
    }

    public void UpdateLeaderboardNames(string[] playerLeaderboard)
    {
        inGameUI.UpdateLeaderboardNames(playerLeaderboard);
    }

    public void FillFinalLeaderboard()
    {
        leaderboardUI.FillFinalLeaderboard();
    }

    #endregion

    #region Activate menu sections methods

    public void HideAll()
    {
        foreach (UIObject ui in uiElements)
        {
            ui.Hide();
        }
    }

    public void ActivateMainMenu()
    {
        HideAll();
        mainMenuUI.Show();
    }

    public void ActivatePreGameUI()
    {
        HideAll();
        preGameUI.Show();
    }

    public void ActivateLobbyUI()
    {
        HideAll();
        lobbyUI.Show();
        lobbyUI.OnActivate();
    }

    public void ActivateInGameHUD()
    {
        HideAll();
        inGameUI.Show();
    }

    public void ActivateServerUI()
    {
        HideAll();
        serverUI.Show();
    }

    public void ActivateConnectingToServerUI()
    {
        HideAll();
        connectingToServerUI.Show();
    }

    public void ActivateFinalLeaderboard()
    {
        HideAll();
        leaderboardUI.Show();
    }

    #endregion
}

public enum GameTypes
{
    Client,
    Host,
    Server
}