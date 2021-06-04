using Game.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConnectingToServerUI : UIObject
{
    [SerializeField] public Button buttonGoBack;

    private void Awake()
    {
        buttonGoBack.onClick.AddListener(() =>
        {
            uiManager.ActivateMainMenu();
            MyNetworkManager.singleton.StopClient();
        });
    }
}
