using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;

public class MenuManager : MonoBehaviour
{
    public NetworkManager networkManager;
    public GameObject MainMenu;
    public GameObject GameMenu;
    public GameObject PauseMenu;
    public GameObject HostGameMenuObject;
    public GameObject JoinGameMenuObject;
    public GameObject Player;
    public string PlayerName = "Player";
    public Transport KCPTransport;
    public Transport FizzySteamworksTransport;
    public Animator MenuAnimation;

    bool paused = false;
    bool InMenu = true;
    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEntered;
    private const string HostAddressKey = "HostAddress";

    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 165;

        MainMenu.SetActive(true);
        GameMenu.SetActive(false);
        paused = false;

        if (!SteamManager.Initialized)
        {
            return;
        }

        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
    }

    void Update()
    {
        if (NetworkServer.active || NetworkClient.active)
        {
            if (InMenu == true)
            {
                GameMenu.SetActive(true);
                MainMenu.SetActive(false);
                InMenu = false;

                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                paused = !paused;

                if (paused == true)
                {
                    PauseMenu.SetActive(true);

                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }

                else
                {
                    PauseMenu.SetActive(false);

                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
            }

            if (Player == null)
            {
                Player[] Players = GameObject.FindObjectsOfType<Player>();

                foreach (Player player in Players)
                {
                    if (player.gameObject.transform.GetComponent<NetworkIdentity>().isLocalPlayer == true)
                    {
                        Player = player.transform.gameObject;

                        player.CmdSetupPlayer(PlayerName);
                    }
                }
            }
        }

        else
        {
            if (InMenu == false)
            {
                MainMenu.SetActive(true);
                GameMenu.SetActive(false);
                InMenu = true;

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }

    public void HostGameMenu()
    {
        HostGameMenuObject.SetActive(true);

        JoinGameMenuObject.SetActive(false);

        MenuAnimation.SetBool("EnableMenu", true);
    }

    public void JoinGameMenu()
    {
        JoinGameMenuObject.SetActive(true);

        HostGameMenuObject.SetActive(false);

        MenuAnimation.SetBool("EnableMenu", true);
    }

    public void HostGame()
    {
        Transport.activeTransport = KCPTransport;
        networkManager.transport = KCPTransport;

        networkManager.StartHost();

        GameMenu.SetActive(true);
        MainMenu.SetActive(false);
        paused = false;
        InMenu = false;
    }

    public void SteamHostGame()
    {
        PlayerName = "SteamPlayer";

        Transport.activeTransport = FizzySteamworksTransport;
        networkManager.transport = FizzySteamworksTransport;

        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, networkManager.maxConnections);

        GameMenu.SetActive(true);
        MainMenu.SetActive(false);
        paused = false;
        InMenu = false;
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            MainMenu.SetActive(true);
            GameMenu.SetActive(false);
            InMenu = true;

            return;
        }

        networkManager.StartHost();

        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey, SteamUser.GetSteamID().ToString());
    }

    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        PlayerName = "SteamPlayer";

        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        if (NetworkServer.active)
        {
            return;
        }

        string hostAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey);

        networkManager.networkAddress = hostAddress;
        networkManager.StartClient();

        MainMenu.SetActive(false);
        GameMenu.SetActive(true);
        paused = false;
        InMenu = false;
    }

    public void JoinGame()
    {
        Transport.activeTransport = KCPTransport;
        networkManager.transport = KCPTransport;

        networkManager.StartClient();

        GameMenu.SetActive(true);
        MainMenu.SetActive(false);
        paused = false;
        InMenu = false;
    }

    public void SetIp(string ip)
    {
        networkManager.networkAddress = ip;
    }

    public void SetName(string name)
    {
        PlayerName = name;
    }

    public void Back()
    {
        MenuAnimation.SetBool("EnableMenu", false);
    }

    public void Stop()
    {
        if (networkManager.mode == NetworkManagerMode.Host)
        {
            networkManager.StopHost();
        }

        if (networkManager.mode == NetworkManagerMode.ClientOnly)
        {
            networkManager.StopClient();
        }

        paused = false;
    }
}
