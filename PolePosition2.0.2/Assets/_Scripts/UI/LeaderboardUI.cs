using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace Game.UI
{
    public class LeaderboardUI : UIObject
    {
        [Header("FinalLeaderboard")]
        [SerializeField] private GameObject textTimes;
        public Text[] textsTm;
        [SerializeField] private GameObject textPosition;
        public Text[] textsPos;
        [SerializeField] private GameObject textName;
        public Text[] textsNm;
        [SerializeField] private Button buttonReturn;

        // Start is called before the first frame update
        void Start()
        {
            textsTm = new Text[textTimes.transform.childCount];
            textsPos = new Text[textPosition.transform.childCount];
            textsNm = new Text[textName.transform.childCount];
            for (int i = 0; i < textName.transform.childCount; i++)
            {
                textsTm[i] = textTimes.transform.GetChild(i).gameObject.GetComponent<Text>();
                textsPos[i] = textPosition.transform.GetChild(i).gameObject.GetComponent<Text>();
                textsNm[i] = textName.transform.GetChild(i).gameObject.GetComponent<Text>();
            }
            buttonReturn.onClick.AddListener(() => UIManager.instance.ActivateMainMenu());
        }


        public string[] FillFinalLeaderboard()
        {
            List<PlayerInfo> players = PolePositionManager.instance.playerLeaderboard;
            //List<float[]> playerTimes = PolePositionManager.instance.playerTimes;

            string[] salida = new string[players.Count];

            for (int i = 0; i < players.Count; i++)
            {
                string aux = "";
                aux += players[i].PlayerName;
                aux += "/";
                aux += (players[i].CurrentPosition + 1).ToString();
                aux += "/";

                //float finalTime = 0;
                //for (int j = 0; j < players[i].times.Count; j++)
                //{
                //    finalTime += players[i].times[j];
                //}

                aux += players[i].CurrentRaceTime.ToString();
                salida[i] = aux;
            }
            return salida;
        }

    }
}