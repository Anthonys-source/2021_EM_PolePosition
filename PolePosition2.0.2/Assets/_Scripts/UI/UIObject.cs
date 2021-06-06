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

        /// <summary>
        /// Show the UI object
        /// </summary>
        public void Show()
        {
            UIContainer.SetActive(true);
        }

        /// <summary>
        /// Hide the UI object
        /// </summary>
        public void Hide()
        {
            UIContainer.SetActive(false);
        }

        /// <summary>
        /// Initialize the UIObject
        /// </summary>
        public void SetUIManager(UIManager uiManager)
        {
            this.uiManager = uiManager;
        }

    }
}