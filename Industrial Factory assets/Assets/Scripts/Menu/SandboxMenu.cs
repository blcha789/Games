using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SandboxMenu : MonoBehaviour
{

    public Text wrenchCounter;
    public int unlockFreePlayCost = 10;

    public GameObject unlockPanel;
    public GameObject notEnoughtWrenchesPanel;
    public GameObject Store;
    public AdsManager adsManager;
    public Text unlockText;

    [Header("")]
    public GameObject[] sandboxButtonLockImage;

    private Fade fade;

    private void Start()
    {
        if (PlayerPrefs.GetInt("FreePlayScene") == 1) //remove after 5 updates, start update 1.14
            PlayerPrefs.SetInt("SandboxNormal", 1);


        fade = GameObject.Find("LevelChanger").GetComponent<Fade>();
        wrenchCounter.text = PlayerPrefs.GetInt("Wrench").ToString();

        bool allLevelsUnlockedNormal = true;
        for (int i = 1; i < 21; i++)
        {
            if (PlayerPrefs.GetInt("Level" + i) != 1)
                allLevelsUnlockedNormal = false;
        }

        if (allLevelsUnlockedNormal || PlayerPrefs.GetInt("SandboxNormal") == 1)
            sandboxButtonLockImage[0].SetActive(false);
        else
            sandboxButtonLockImage[0].SetActive(true);


        bool allLevelsUnlockedwithElectricity = true;
        for (int i = 1; i < 31; i++)
        {
            if (PlayerPrefs.GetInt("Level" + i) != 1)
                allLevelsUnlockedwithElectricity = false;
        }

        if (allLevelsUnlockedwithElectricity || PlayerPrefs.GetInt("SandboxWithElectricity") == 1)
            sandboxButtonLockImage[1].SetActive(false);
        else
            sandboxButtonLockImage[1].SetActive(true);
    }

    public void SandboxNormal()
    {
        bool allLevelsUnlocked = true;
        for (int i = 1; i < 21; i++)
        {
            if (PlayerPrefs.GetInt("Level" + i) != 1)
                allLevelsUnlocked = false;
        }

        if (allLevelsUnlocked || PlayerPrefs.GetInt("SandboxNormal") == 1)
            fade.FadeToLevel("SandboxNormal");
        else
        {
            unlockPanel.SetActive(true);
            unlockText.text = "You need  to complete all Levels to Level 20 or you can unlock it with wrenches.";
            unlockPanel.transform.GetChild(0).GetChild(0).GetComponent<Button>().onClick.RemoveAllListeners();
            unlockPanel.transform.GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(() => Unlock("SandboxNormal", 0));
        }
    }

    public void SandboxWithElectricity()
    {
        bool allLevelsUnlocked = true;
        for (int i = 1; i < 31; i++)
        {
            if (PlayerPrefs.GetInt("Level" + i) != 1)
                allLevelsUnlocked = false;
        }

        if (allLevelsUnlocked || PlayerPrefs.GetInt("SandboxWithElectricity") == 1)
            fade.FadeToLevel("SandboxWithElectricity");
        else
        {
            unlockPanel.SetActive(true);
            unlockText.text = "You need  to complete all Levels to Level 30 or you can unlock it with wrenches.";
            unlockPanel.transform.GetChild(0).GetChild(0).GetComponent<Button>().onClick.RemoveAllListeners();
            unlockPanel.transform.GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(() => Unlock("SandboxWithElectricity", 1));
        }
    }

    public void Back()
    {
        fade.FadeToLevel("LevelMenu");
    }

    public void Unlock(string levelName, int k)
    {
        if (PlayerPrefs.GetInt("Wrench") >= unlockFreePlayCost)
        {
            unlockPanel.SetActive(false);
            PlayerPrefs.SetInt(levelName, 1);

            int i = PlayerPrefs.GetInt("Wrench") - unlockFreePlayCost;
            PlayerPrefs.SetInt("Wrench", i);

            sandboxButtonLockImage[k].SetActive(false);
            wrenchCounter.text = PlayerPrefs.GetInt("Wrench").ToString();
        }
        else
        {
            unlockPanel.SetActive(false);
            notEnoughtWrenchesPanel.SetActive(true);
            notEnoughtWrenchesPanel.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = unlockFreePlayCost.ToString();
        }
    }

    public void Close()
    {
        unlockPanel.SetActive(false);
        notEnoughtWrenchesPanel.SetActive(false);
        Store.SetActive(false);
    }

    public void OpenStore()
    {
        Store.SetActive(true);
        unlockPanel.SetActive(false);
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
