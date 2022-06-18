using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MenuManager : MonoBehaviour
{
    public NetworkManager networkManager;

    public GameObject MainMenu;

    public GameObject GameMenu;

    public GameObject PauseMenu;

    bool paused = false;

    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 165;

        MainMenu.SetActive(true);
        GameMenu.SetActive(false);

        paused = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            paused = !paused;
        }

        if (NetworkServer.active || NetworkClient.active)
        {
            Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = paused;

            if (paused == true)
            {
                PauseMenu.SetActive(true);
            }

            else
            {
                PauseMenu.SetActive(false);
            }
        }

        else
        {
            MainMenu.SetActive(true);
            GameMenu.SetActive(false);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void HostGame()
    {
        networkManager.StartHost();

        GameMenu.SetActive(true);
        MainMenu.SetActive(false);
        paused = false;
    }

    public void JoinGame()
    {
        networkManager.StartClient();

        GameMenu.SetActive(true);
        MainMenu.SetActive(false);
        paused = false;
    }

    public void SetIp(string ip)
    {
        networkManager.networkAddress = ip;
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
