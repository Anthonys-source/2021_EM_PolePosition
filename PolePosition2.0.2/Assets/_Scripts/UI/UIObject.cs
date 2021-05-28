using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
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