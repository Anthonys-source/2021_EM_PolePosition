using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerUI : MonoBehaviour
{
    [SerializeField] private GameObject serverUIContainer;

    public void Show()
    {
        serverUIContainer.SetActive(true);
    }

    public void Hide()
    {
        serverUIContainer.SetActive(false);
    }
}
