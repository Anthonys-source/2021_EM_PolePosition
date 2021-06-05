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
        [SerializeField] private Text readyText;
        [SerializeField] private Button buttonReady;
        public event Action<bool> onReadyStateChange = delegate { };

        private bool ready = false;

        private void Awake()
        {
            buttonReady.onClick.AddListener(ReadyUp);
            UpdateReadyUI();
        }

        public void ReadyUp()
        {
            ready = !ready;
            onReadyStateChange.Invoke(ready);
            UpdateReadyUI();
        }

        private void UpdateReadyUI()
        {
            if (!ready)
            {
                readyText.text = "Not Ready";
                buttonReady.GetComponentInChildren<Text>().text = "Ready Up";
            }
            else
            {
                readyText.text = "Ready to Race";
                buttonReady.GetComponentInChildren<Text>().text = "Unready";
            }
        }
    }
}
