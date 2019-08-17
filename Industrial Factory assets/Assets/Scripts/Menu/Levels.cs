using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Levels : MonoBehaviour {

    public int levelModifier;//0 , 10, 20, 30, ...
    public int unlockLevelCost = 3;
    public GameObject[] level;
    public Text wrenchCounter;
    public GameObject UnlockPanel;
    public GameObject notEnoughtWrenchesPanel;
    public GameObject Store;
    public AdsManager adsManager;

    [Header("ShopStuff")]
    public GameObject removeAdsWatchVideoButton;
    public GameObject removeAdsPuchasedButton;

    private Fade fade;

    private void Start()
    {
        fade = GameObject.Find("LevelChanger").GetComponent<Fade>();
        wrenchCounter.text = PlayerPrefs.GetInt("Wrench").ToString();

        if (PlayerPrefs.GetInt("AdsRemoved") == 1)
        {
            removeAdsPuchasedButton.SetActive(true);
            removeAdsWatchVideoButton.SetActive(false);
        }

        for (int i = 0; i < 10; i++)
        {
            if(PlayerPrefs.GetInt("Level" + (i + 1 + levelModifier)) == 1)
            {
                level[i].transform.GetChild(1).gameObject.SetActive(false);
            }
        }
    }

    public void Back()
    {
        fade.FadeToLevel("LevelMenu");
    }

    public void ChooseLevel(int i)
    {
        string levelName = EventSystem.current.currentSelectedGameObject.name;

        if (PlayerPrefs.GetInt(levelName) == 1)//unlocknutý
            fade.FadeToLevel(levelName);
        else//locknutý
        {
            UnlockPanel.SetActive(true);
            UnlockPanel.transform.GetChild(0).GetChild(0).GetComponent<Button>().onClick.RemoveAllListeners();
            UnlockPanel.transform.GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(() => Unlock(levelName, i));
        }
    }

    public void Unlock(string levelName, int l)
    {
        int i = PlayerPrefs.GetInt("Wrench");

        if (i >= unlockLevelCost)
        {
            level[l].transform.GetChild(1).gameObject.SetActive(false);
            UnlockPanel.SetActive(false);
            PlayerPrefs.SetInt(levelName, 1);

            i = i - unlockLevelCost;
            PlayerPrefs.SetInt("Wrench", i);

            wrenchCounter.text = PlayerPrefs.GetInt("Wrench").ToString();
        }
        else
        {
            UnlockPanel.SetActive(false);
            notEnoughtWrenchesPanel.SetActive(true);
            notEnoughtWrenchesPanel.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = unlockLevelCost.ToString();
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
}
