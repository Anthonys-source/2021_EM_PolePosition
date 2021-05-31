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

        public void Show()
        {
            UIContainer.SetActive(true);
        }

        public void Hide()
        {
            UIContainer.SetActive(false);
        }
    }
}