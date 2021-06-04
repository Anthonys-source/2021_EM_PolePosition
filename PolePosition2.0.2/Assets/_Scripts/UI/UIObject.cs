using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    /// <summary>
    /// Base class for all UI Objects part of the UIManager
    /// </summary>
    public class UIObject : MonoBehaviour
    {
        [SerializeField] protected GameObject UIContainer;
        [HideInInspector] protected UIManager uiManager;

        public void Show()
        {
            UIContainer.SetActive(true);
        }

        public void Hide()
        {
            UIContainer.SetActive(false);
        }

        public void SetUIManager(UIManager uiManager)
        {
            this.uiManager = uiManager;
        }

    }
}