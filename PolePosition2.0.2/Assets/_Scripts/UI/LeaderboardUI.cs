using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace Game.UI{
    public class LeaderboardUI : UIObject
    {
        [Header("FinalLeaderboard")]
        [SerializeField] private GameObject textTimes;
        private Text[] textsTm;
        [SerializeField] private GameObject textPosition;
        private Text[] textsPos;
        [SerializeField] private GameObject textName;
        private Text[] textsNm;

        // Start is called before the first frame update
        void Start()
        {
            textsTm = new Text[textTimes.transform.childCount];
            textsPos = new Text[textPosition.transform.childCount];
            textsNm = new Text[textName.transform.childCount];
            for (int i = 0; i<textName.transform.childCount; i++)
            {
                textsTm[i] = textTimes.transform.GetChild(i).gameObject.GetComponent<Text>();
                textsPos[i] = textPosition.transform.GetChild(i).gameObject.GetComponent<Text>();
                textsNm[i] = textName.transform.GetChild(i).gameObject.GetComponent<Text>();
            }
        }

        public void FillFinalLeaderboard()
        {
            List<PlayerInfo> players = PolePositionManager.instance.playersList;
            List<float[]> playerTimes = PolePositionManager.instance.playerTimes;
            for (int i = 0; i<players.Count; i++)
            {
                textsNm[i].text = players[i].PlayerName;
                textsPos[i].text = players[i].CurrentPosition.ToString();
                float finalTime = 0;
                for(int j = 0; j<playerTimes.Count; j++)
                {
                    finalTime += playerTimes[i][j];
                }
                textsTm[i].text = finalTime.ToString();
            }
            
        }
    }
}