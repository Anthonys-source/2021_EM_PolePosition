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

        public void UpdateSpeed(int speed)
        {
            textSpeed.text = "Speed " + speed + " Km/h";
        }

        public void UpdateLaps(int currentLap)
        {

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
    }
}