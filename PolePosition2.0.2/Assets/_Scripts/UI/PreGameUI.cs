using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreGameUI : MonoBehaviour
{
    [Header("Pre Game")] [SerializeField] private GameObject preGameUIContainer;
    [SerializeField] private InputField inputFieldPlayerName;
    [SerializeField] private Dropdown dropdownCarColorID;
    [SerializeField] public Button buttonGo;

    [HideInInspector] public UIManager _uiManager;

    public string SelectedName { get => inputFieldPlayerName.text; }
    public int SelectedCarColorID { get => dropdownCarColorID.value; }

    private void Awake()
    {
        buttonGo.onClick.AddListener(StartGame);
    }

    public void SetUIManager(UIManager uiManager)
    {
        _uiManager = uiManager;
    }

    public void Show()
    {
        preGameUIContainer.SetActive(true);
    }

    public void Hide()
    {
        preGameUIContainer.SetActive(false);
    }

    public void StartGame()
    {
        switch (_uiManager._selectedGameType)
        {
            case GameTypes.Client:

                _uiManager.m_NetworkManager.StartClient();
                _uiManager.m_NetworkManager.networkAddress = _uiManager._mainMenuUI.SelectedIP;

                break;
            case GameTypes.Host:

                _uiManager.m_NetworkManager.StartHost();

                break;
            default:
                break;
        }

        _uiManager.ActivateInGameHUD();
    }
}
