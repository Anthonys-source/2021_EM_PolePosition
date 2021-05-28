﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class MainMenuUI : UIObject
    {
        [Header("Main Menu")]
        [SerializeField] public Button buttonHost;
        [SerializeField] public Button buttonClient;
        [SerializeField] public Button buttonServer;
        [SerializeField] private InputField inputFieldIP;
        public string SelectedIP { get => inputFieldIP.text; }

        [HideInInspector] public UIManager _uiManager;

        private void Start()
        {
            buttonHost.onClick.AddListener(SelectHost);
            buttonClient.onClick.AddListener(SelectClient);
            buttonServer.onClick.AddListener(SelectServer);
        }

        public void SetUIManager(UIManager uiManager)
        {
            _uiManager = uiManager;
        }

        private void SelectHost()
        {
            _uiManager._selectedGameType = GameTypes.Host;
            _uiManager.ActivatePreGameUI();
        }

        private void SelectClient()
        {
            _uiManager._selectedGameType = GameTypes.Client;
            _uiManager.ActivatePreGameUI();
        }

        private void SelectServer()
        {
            _uiManager.gameSetupManager.StartServer();
            _uiManager.ActivateServerUI();
        }
    }
}