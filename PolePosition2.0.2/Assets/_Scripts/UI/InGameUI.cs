using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class InGameUI : UIObject
    {
        [Header("In-Game HUD")]
        [SerializeField] private Text textSpeed;
        [SerializeField] private Text textLaps;
        [SerializeField] private Text textPosition;
        [SerializeField] private Text textTime;
        [SerializeField] private Text textRaceTime;
        [SerializeField] private GameObject panelRaceCountdown;
        [SerializeField] private Text textRaceCountdown;
        [SerializeField] private Button buttonExit;

        private void Awake()
        {
            buttonExit.onClick.AddListener(Exit);
        }

        private void Exit()
        {
            UIManager.instance.ActivateMainMenu();
            Camera.main.transform.position = PolePositionManager.instance.originalCameraPos;
            Camera.main.transform.rotation = PolePositionManager.instance.originalCameraRot;
            Camera.main.GetComponent<CameraController>().m_Focus = null;
            UIManager.instance.inGameUI.UpdateRaceTime(0);
            UIManager.instance.inGameUI.UpdateLapTime(0);
            UIManager.instance.inGameUI.UpdateLaps(0, 0);
            MyNetworkManager.singleton.StopClient();
        }

        /// <summary>
        /// Update UI Speed
        /// </summary>
        public void UpdateSpeed(int speed)
        {
            textSpeed.text = "Speed " + speed + " Km/h";
        }

        /// <summary>
        /// Update UI Laps
        /// </summary>
        public void UpdateLaps(int currentLap, int maxLaps)
        {
            textLaps.text = "Laps: " + currentLap.ToString() + "/" + maxLaps;
        }

        public void UpdateLapTime(float lapTime)
        {
            TimeSpan time = TimeSpan.FromSeconds(lapTime);
            textTime.text = $"Lap Time:\n {(int)time.TotalMinutes}:{time.Seconds:00}";
            //textTime.text = "Lap Time: " + ((int)lapTime).ToString() +  "  ";
        }

        public void UpdateRaceTime(float raceTime)
        {
            TimeSpan time = TimeSpan.FromSeconds(raceTime);
            textRaceTime.text = $"Race Time:\n {(int)time.TotalMinutes}:{time.Seconds:00}";
            //textRaceTime.text = "Race Time: " + ((int)raceTime).ToString();
        }

        /// <summary>
        /// Updates the Positions Text int the UI
        /// </summary>
        /// <param name="playerLeaderboard">Ordered list of players positions</param>
        public void UpdateLeaderboardNames(string[] playerLeaderboard)
        {
            textPosition.text = "";

            for (int i = 0; i < playerLeaderboard.Length; i++)
            {
                textPosition.text += $"{i + 1}º - {playerLeaderboard[i]}\n";
            }

            //Remove the last \n character
            textPosition.text = textPosition.text.Remove(textPosition.text.Length - 1);
        }

        public void StartRaceCountdown()
        {
            StartCoroutine(RaceCountdown());
        }

        private IEnumerator RaceCountdown()
        {
            panelRaceCountdown.SetActive(true);
            WaitForSeconds s = new WaitForSeconds(1.0f);
            for (int i = 3; i > 0; i--)
            {
                textRaceCountdown.text = i.ToString();
                yield return s;
            }
            textRaceCountdown.text = "GO!!";
            yield return s;
            textRaceCountdown.text = "";
            panelRaceCountdown.SetActive(false);
        }
    }
}