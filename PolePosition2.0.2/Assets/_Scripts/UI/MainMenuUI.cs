using System.Collections;
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

        private void Start()
        {
            buttonHost.onClick.AddListener(SelectHost);
            buttonClient.onClick.AddListener(SelectClient);
            buttonServer.onClick.AddListener(SelectServer);
        }

        private void SelectHost()
        {
            uiManager.selectedGameType = GameTypes.Host;
            uiManager.ActivatePreGameUI();
        }

        private void SelectClient()
        {
            uiManager.selectedGameType = GameTypes.Client;
            uiManager.ActivatePreGameUI();
        }

        private void SelectServer()
        {
            uiManager.gameSetupManager.StartServer();
            uiManager.ActivateServerUI();
        }
    }
}