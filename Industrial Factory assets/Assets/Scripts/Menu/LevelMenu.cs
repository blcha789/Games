using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelMenu : MonoBehaviour {

    public Text wrenchCounter;
    public int unlockFreePlayCost = 10;

    public GameObject UnlockPanel;
    public GameObject notEnoughtWrenchesPanel;
    public GameObject freePlayButtonLockImage;
    public GameObject Store;
    public AdsManager adsManager;

    private Fade fade;

    private void Start()
    {
        fade = GameObject.Find("LevelChanger").GetComponent<Fade>();
        wrenchCounter.text = PlayerPrefs.GetInt("Wrench").ToString();

        bool allLevelsUnlocked = true;
        for (int i = 1; i < 21; i++)
        {
            if (PlayerPrefs.GetInt("Level" + i) != 1)
                allLevelsUnlocked = false;
        }

        if (allLevelsUnlocked || PlayerPrefs.GetInt("FreePlayScene") == 1)
            freePlayButtonLockImage.SetActive(false);
        else
            freePlayButtonLockImage.SetActive(true);
    }

    public void FreePlay()
    {
        bool allLevelsUnlocked = true;
        for (int i = 1; i < 21; i++)
        {
            if (PlayerPrefs.GetInt("Level" + i) != 1)
                allLevelsUnlocked = false;
        }

        if (allLevelsUnlocked || PlayerPrefs.GetInt("FreePlayScene") == 1)
            fade.FadeToLevel("SandboxScene");
        else
        {
            UnlockPanel.SetActive(true);
            UnlockPanel.transform.GetChild(0).GetChild(0).GetComponent<Button>().onClick.RemoveAllListeners();
            UnlockPanel.transform.GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(() => Unlock("FreePlayScene"));
        }
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

    public void Unlock(string levelName)
    {
        if (PlayerPrefs.GetInt("Wrench") >= unlockFreePlayCost)
        {
            UnlockPanel.SetActive(false);
            PlayerPrefs.SetInt(levelName, 1);

            int i = PlayerPrefs.GetInt("Wrench") - unlockFreePlayCost;
            PlayerPrefs.SetInt("Wrench", i);

            freePlayButtonLockImage.SetActive(false);
            wrenchCounter.text = PlayerPrefs.GetInt("Wrench").ToString();
        }
        else
        {
            UnlockPanel.SetActive(false);
            notEnoughtWrenchesPanel.SetActive(true);
            notEnoughtWrenchesPanel.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = unlockFreePlayCost.ToString();
        }
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
