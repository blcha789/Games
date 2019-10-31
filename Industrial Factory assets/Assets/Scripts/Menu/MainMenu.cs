using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

    public Text builderFactoriesCounter;
    public Text apkVersion;

    private string levelToLoad;

    private Fade fade;

    private void Start()
    {
        fade = GameObject.Find("LevelChanger").GetComponent<Fade>();
        PlayerPrefs.SetInt("Level1", 1);
        builderFactoriesCounter.text = PlayerPrefs.GetInt("BuildedFactories").ToString();
        apkVersion.text = "v" + Application.version;
        //latestVersion
    }

    public void Play()
    {
        fade.FadeToLevel("LevelMenu");
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void Shop()
    {

    }

    public void Sound()
    {

    }

    public void Info()
    {

    }

    public void Fb()
    {

    }
    
    public void ReportBug()
    {
        
    }
    
    public void ShowUpdateChanges()
    {
        
    }

    public void MoreFromUs()
    {
        Application.OpenURL("market://details?id=com.ShuterStudio.AngryDeadGuys");
    }
}
