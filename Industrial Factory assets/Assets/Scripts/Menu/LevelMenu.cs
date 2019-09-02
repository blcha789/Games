using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelMenu : MonoBehaviour {

    public Text wrenchCounter;

    public GameObject UnlockPanel;
    public GameObject notEnoughtWrenchesPanel;
    public GameObject Store;
    public AdsManager adsManager;

    private Fade fade;

    private void Start()
    {
        fade = GameObject.Find("LevelChanger").GetComponent<Fade>();
        wrenchCounter.text = PlayerPrefs.GetInt("Wrench").ToString();       
    }

    public void Sandbox()
    {
        fade.FadeToLevel("SandboxMenu");
    }

    public void Back()
    {
        fade.FadeToLevel("MainMenu");
    }

    public void Level1_10()
    {
            fade.FadeToLevel("Level1-10");
    }

    public void Level11_20()
    {
            fade.FadeToLevel("Level11-20");
    }

    public void Level21_30()
    {
        fade.FadeToLevel("Level21-30");
    }

    public void Close()
    {
        UnlockPanel.SetActive(false);
        notEnoughtWrenchesPanel.SetActive(false);
        Store.SetActive(false);
    }

    public void OpenStore()
    {
        Store.SetActive(true);
        UnlockPanel.SetActive(false);
        notEnoughtWrenchesPanel.SetActive(false);
    }

    public void ShowAds()
    {
        adsManager.ShowRewardVideoAd();
        notEnoughtWrenchesPanel.SetActive(false);
    }

    public void ResetPrefs()
    {
        PlayerPrefs.DeleteAll();
    }

    public void AddWrench()
    {
        PlayerPrefs.SetInt("Wrench", 50);
        wrenchCounter.text = PlayerPrefs.GetInt("Wrench").ToString();
    }
}
