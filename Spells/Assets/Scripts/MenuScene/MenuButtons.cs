using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MenuButtons : MonoBehaviour {

    [Header("Main")]
    public GameObject MenuPanel;
    public GameObject PlayPanel;
    public GameObject SettingsPanel;

	
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Play()
    {
        MenuPanel.SetActive(false);
        PlayPanel.SetActive(true);
        GetComponent<CreateJoinRoom>().networkManager = NetworkManager.singleton;
        GetComponent<CreateJoinRoom>().networkManager.StartMatchMaker();
    }

    public void Settings()
    {
        MenuPanel.SetActive(false);
        SettingsPanel.SetActive(true);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void Menu()
    {
        MenuPanel.SetActive(true);
        PlayPanel.SetActive(false);
        SettingsPanel.SetActive(false);
    }

}
