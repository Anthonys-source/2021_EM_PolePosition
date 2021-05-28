using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("Main Menu")] [SerializeField] private GameObject mainMenuUIContainer;
    [SerializeField] public Button buttonHost;
    [SerializeField] public Button buttonClient;
    [SerializeField] public Button buttonServer;
    [SerializeField] private InputField inputFieldIP;
    public string SelectedIP { get => inputFieldIP.text; }

    [HideInInspector] public UIManager _uiManager;

    private void Start()
    {
        buttonHost.onClick.AddListener(SelectHost);
        buttonClient.onClick.AddListener(SelectClient);
        buttonServer.onClick.AddListener(SelectServer);
    }

    public void SetUIManager(UIManager uiManager)
    {
        _uiManager = uiManager;
    }

    public void Show()
    {
        mainMenuUIContainer.SetActive(true);
    }

    public void Hide()
    {
        mainMenuUIContainer.SetActive(false);
    }

    private void SelectHost()
    {
        _uiManager._selectedGameType = GameTypes.Host;
        _uiManager.ActivatePreGameUI();
    }

    private void SelectClient()
    {
        _uiManager._selectedGameType = GameTypes.Client;
        _uiManager.ActivatePreGameUI();
    }

    private void SelectServer()
    {
        _uiManager._selectedGameType = GameTypes.Server;
        _uiManager.m_NetworkManager.StartServer();
        _uiManager.ActivateServerUI();
    }
}
