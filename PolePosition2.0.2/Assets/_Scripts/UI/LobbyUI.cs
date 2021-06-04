using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace Game.UI
{
    public class LobbyUI : UIObject
    {
        [Header("Lobby")]
        [SerializeField] private Button buttonGo;
        public event Action<bool> onReadyStateChange = delegate { };

        //public string SelectedName { get => inputFieldPlayerName.text; }
        //public int SelectedCarColorID { get => dropdownCarColorID.value; }

        private void Awake()
        {
            buttonGo.onClick.AddListener(ReadyUp);
        }

        public void ReadyUp()
        {
            //switch (uiManager.selectedGameType)
            //{
            //    case GameTypes.Client:

            //        uiManager.gameSetupManager.StartClient(uiManager.mainMenuUI.SelectedIP);

            //        break;
            //    case GameTypes.Host:

            //        uiManager.gameSetupManager.StartHost();

            //        break;
            //    default:
            //        break;
            //}
            onReadyStateChange.Invoke(true);
            //uiManager.ActivateInGameHUD();
        }
    }
}
