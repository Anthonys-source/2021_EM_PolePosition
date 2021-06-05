using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace Game.UI
{
    public class FinalLeaderboardUI : UIObject
    {
        [Header("Final Leaderboard")]
        [SerializeField] private Text[] PositionText;
        [SerializeField] private Text[] TimeText;
        [SerializeField] private Button buttonReturn;
        //UIManager uiManager;


        public event Action<bool> onReadyStateChange = delegate { };


        private void Awake()
        {
            buttonReturn.onClick.AddListener(ReturnToMenu);
            PositionText = new Text[4];
            TimeText = new Text[4];
        }

        public void OnActivate()
        {

        }

        public void ReturnToMenu()
        {
            UIManager.instance.ActivateMainMenu();
        }
    }
}
