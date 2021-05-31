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
        private UIManager uiManager;

        public string SelectedName { get => inputFieldPlayerName.text; }
        public int SelectedCarColorID { get => dropdownCarColorID.value; }

        private void Awake()
        {
            buttonGo.onClick.AddListener(StartGame);
        }

        public void SetUIManager(UIManager uiManager)
        {
            this.uiManager = uiManager;
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

            uiManager.ActivateInGameHUD();
        }
    }
}