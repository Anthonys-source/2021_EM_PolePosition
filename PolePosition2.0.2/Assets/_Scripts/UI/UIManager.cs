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
    public PreGameUI preGameUI;
    public InGameUI inGameUI;
    public ServerUI serverUI;
    public LobbyUI lobbyUI;

    // Current selected game "Type" (Host,Client,Server) that the game
    // instance will try to start
    [HideInInspector] public GameTypes selectedGameType = GameTypes.Host;

    // The UI has the configuration of the player that will be passed
    // on connection with the server, This is horrible
    public string EnteredPlayerName { get => preGameUI.SelectedName; }
    public int EnteredCarColorID { get => preGameUI.SelectedCarColorID; }

    private void Awake()
    {
        mainMenuUI.SetUIManager(this);
        preGameUI.SetUIManager(this);
        lobbyUI.SetUIManager(this);
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

    public void UpdateLaps(int newLap)
    {
        inGameUI.UpdateLaps(newLap);
    }

    public void UpdateLeaderboardNames(string[] playerLeaderboard)
    {
        inGameUI.UpdateLeaderboardNames(playerLeaderboard);
    }

    #endregion

    #region Activate menu sections methods

    public void ActivateMainMenu()
    {
        mainMenuUI.Show();
        preGameUI.Hide();
        lobbyUI.Hide();
        inGameUI.Hide();
    }

    public void ActivatePreGameUI()
    {
        mainMenuUI.Hide();
        preGameUI.Show();
        lobbyUI.Hide();
        inGameUI.Hide();
    }

    public void ActivateLobbyUI()
    {
        mainMenuUI.Hide();
        preGameUI.Hide();
        lobbyUI.Show();
        inGameUI.Hide();
    }

    public void ActivateInGameHUD()
    {
        mainMenuUI.Hide();
        preGameUI.Hide();
        lobbyUI.Hide();
        inGameUI.Show();
    }

    public void ActivateServerUI()
    {
        mainMenuUI.Hide();
        preGameUI.Hide();
        lobbyUI.Hide();
        inGameUI.Hide();
        serverUI.Show();
    }

    #endregion
}

public enum GameTypes
{
    Client,
    Host,
    Server
}