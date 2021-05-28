using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    [Header("In-Game HUD")]
    [SerializeField]
    private GameObject inGameUIContainer;

    [SerializeField] private Text textSpeed;
    [SerializeField] private Text textLaps;
    [SerializeField] private Text textPosition;

    public void Show()
    {
        inGameUIContainer.SetActive(true);
    }

    public void Hide()
    {
        inGameUIContainer.SetActive(false);
    }

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
        foreach (string playerInfo in playerLeaderboard)
        {
            textPosition.text += playerInfo + "\n";
        }

        //Remove the last \n character
        textPosition.text = textPosition.text.Remove(textPosition.text.Length - 1);
    }
}
