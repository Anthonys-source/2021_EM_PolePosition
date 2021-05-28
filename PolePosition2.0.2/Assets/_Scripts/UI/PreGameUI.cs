﻿using System.Collections;
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
        [SerializeField] public Button buttonGo;

        [HideInInspector] public UIManager _uiManager;

        public string SelectedName { get => inputFieldPlayerName.text; }
        public int SelectedCarColorID { get => dropdownCarColorID.value; }

        private void Awake()
        {
            buttonGo.onClick.AddListener(StartGame);
        }

        public void SetUIManager(UIManager uiManager)
        {
            _uiManager = uiManager;
        }

        public void StartGame()
        {
            switch (_uiManager._selectedGameType)
            {
                case GameTypes.Client:

                    _uiManager.gameSetupManager.StartClient(_uiManager._mainMenuUI.SelectedIP);

                    break;
                case GameTypes.Host:

                    _uiManager.gameSetupManager.StartHost();

                    break;
                default:
                    break;
            }

            _uiManager.ActivateInGameHUD();
        }
    }
}