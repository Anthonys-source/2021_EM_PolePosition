using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class PreGameUI : UIObject
    {
        [Header("Pre Game")]
        [SerializeField] private InputField inputFieldPlayerName;
        [SerializeField] private Dropdown dropdownCarColorID;
        [SerializeField] private Button buttonGo;

        [SerializeField] private ClientData clientData;

        private void Awake()
        {
            buttonGo.onClick.AddListener(StartGame);
        }

        private void OnEnable()
        {
            inputFieldPlayerName.onValueChanged.AddListener(SetClientName);
            dropdownCarColorID.onValueChanged.AddListener(SetClientCarColorID);
        }

        private void OnDisable()
        {
            inputFieldPlayerName.onValueChanged.RemoveListener(SetClientName);
            dropdownCarColorID.onValueChanged.RemoveListener(SetClientCarColorID);
        }

        private void SetClientName(string call)
        {
            clientData.playerName = call;
        }

        private void SetClientCarColorID(int call)
        {
            clientData.carColorID = call;
        }

        public void StartGame()
        {
            switch (uiManager.selectedGameType)
            {
                case GameTypes.Client:

                    uiManager.gameSetupManager.StartClient(uiManager.mainMenuUI.SelectedIP);

                    break;
                case GameTypes.Host:

                    uiManager.gameSetupManager.StartHost();

                    break;
                default:
                    break;
            }

            uiManager.ActivateConnectingToServerUI();
        }
    }
}