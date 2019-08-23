using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GameButtons : MonoBehaviour
{
    public GameObject pausePanel;
    public Fade fade;

    private GameLogic gameLogic;
    private BuildAndDemolish buildAndDemolish;
    private Shop shop;
    private GameAdManager adManager;
    private CameraMovement cameraMovement;


    private void Start()
    {
        gameLogic = GetComponent<GameLogic>();
        buildAndDemolish = GetComponent<BuildAndDemolish>();
        shop = GetComponent<Shop>();
        adManager = GetComponent<GameAdManager>();
        cameraMovement = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMovement>();
        Time.timeScale = 1;
    }

    public void PlayNight()
    {
        gameLogic.StartNight();
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Demolish()
    {
        buildAndDemolish.PickDemolish();
    }

    public void ToMenu()
    {
        fade.FadeToLevel("MainMenu");
    }

    public void Again()
    {
        string name = SceneManager.GetActiveScene().name;
        fade.FadeToLevel(name);
    }

    public void Pause()
    {
        cameraMovement.enabled = false;
        Time.timeScale = 0;
        pausePanel.SetActive(true);
    }

    public void Continue()
    {
        cameraMovement.enabled = true;
        Time.timeScale = 1;
        pausePanel.SetActive(false);
    }

    public void CloseNotEnoughtMoney()
    {
        shop.CloseNotEnoughtMoney();
    }

    public void Shop()
    {
        cameraMovement.enabled = false;
        shop.OpenShop();
    }

    public void CloseShop()
    {
        cameraMovement.enabled = true;
        shop.CloseShop();
    }

    public void MoneyShop()
    {
        CloseNotEnoughtMoney();
        shop.OpenMoneyShop();
    }

    public void CloseMoneyShop()
    {
        shop.CloseMoneyShop();
    }

    public void GetFreeMoney()
    {
        adManager.ShowRewardMoneyVideoAd();
    }
}
