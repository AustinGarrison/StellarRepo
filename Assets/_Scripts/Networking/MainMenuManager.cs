using FishNet.Component.Spawning;
using Steamworks;
using System;
using System.Security.Cryptography;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    private static MainMenuManager Instance;
    [SerializeField] private GameObject menuScreen, lobbyScreen;
    [SerializeField] private TMP_InputField lobbyInput;
    [SerializeField] private TextMeshProUGUI lobbyTitleText, lobbyIDText;
    [SerializeField] private Button startGameButton;

    public static event EventHandler OnLobbyInit;

    private void Awake() => Instance = this;

    private void Start()
    {
        OpenMainMenu();
    }

    public void CreateLobby()
    {
        BootstrapManager.CreateLobby();
        OnLobbyInit?.Invoke(this, EventArgs.Empty);
    }

    public void OpenMainMenu()
    {
        CloseAllScreens();
        menuScreen.SetActive(true);
    }

    public void OpenLobby()
    {
        CloseAllScreens();
        lobbyScreen.SetActive(true);
    }

    public static void LobbyEntered(string lobbyName, bool isHost)
    {
        Debug.Log("IsHost: " + isHost);
        Debug.Log("LobbyNAme: " + lobbyName);
        Instance.lobbyTitleText.text = lobbyName;
        Instance.startGameButton.gameObject.SetActive(isHost);
        Instance.lobbyIDText.text = BootstrapManager.CurrentLobbyID.ToString();
        Instance.OpenLobby();
    }


    void CloseAllScreens()
    {
        menuScreen.SetActive(false);
        lobbyScreen.SetActive(false);
    }

    public void JoinLobby()
    {

        CSteamID steamID = new CSteamID(Convert.ToUInt64(lobbyInput.text));
        BootstrapManager.JoinByID(steamID);
        OnLobbyInit?.Invoke(this, EventArgs.Empty);
    }

    public void LeaveLobby()
    {
        OpenMainMenu();
    }

    public void StartGame()
    {
        string[] scenesToClose = new string[] { "MenuSceneSteam" };
        BootstrapNetworkManager.ChangeNetworkSceneMain("SteamGameScene", scenesToClose);
    }

    public void GetPlayer()
    {

    }
}
