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
    public MainMenuUI _mainMenuUI;
    public PreGameUI _preGameUI;
    public InGameUI _inGameUI;
    public ServerUI _serverUI;

    [HideInInspector] public GameTypes _selectedGameType = GameTypes.Host;

    //Used to setup player
    //Not the cleanest aproach
    public string EnteredPlayerName { get => _preGameUI.SelectedName; }
    public int EnteredCarColorID { get => _preGameUI.SelectedCarColorID; }

    private void Awake()
    {
        if (gameSetupManager == null)
        {
            Debug.LogError("Null reference");
        }

        _mainMenuUI.SetUIManager(this);
        _preGameUI.SetUIManager(this);
    }

    private void Start()
    {
        ActivateMainMenu();
    }

    #region InGame UI

    public void UpdateSpeed(int speed)
    {
        _inGameUI.UpdateSpeed(speed);
    }

    public void UpdateLaps(int newLap)
    {
        _inGameUI.UpdateLaps(newLap);
    }

    public void UpdateLeaderboardNames(string[] playerLeaderboard)
    {
        _inGameUI.UpdateLeaderboardNames(playerLeaderboard);
    }

    #endregion

    public void ActivateMainMenu()
    {
        _mainMenuUI.Show();
        _preGameUI.Hide();
        _inGameUI.Hide();
    }

    public void ActivatePreGameUI()
    {
        _mainMenuUI.Hide();
        _preGameUI.Show();
        _inGameUI.Hide();
    }

    public void ActivateInGameHUD()
    {
        _mainMenuUI.Hide();
        _preGameUI.Hide();
        _inGameUI.Show();
    }

    public void ActivateServerUI()
    {
        _mainMenuUI.Hide();
        _preGameUI.Hide();
        _inGameUI.Hide();
        _serverUI.Show();
    }
}

public enum GameTypes
{
    Client,
    Host,
    Server
}