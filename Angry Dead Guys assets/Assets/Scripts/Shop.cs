using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Shop : MonoBehaviour
{
    public GameObject shopPanel;
    public GameObject moneyShopPanel;
    public GameObject notEnoughtMoneyPanel;

    public Text shopMoneyText;
    public Text gameMoneyText;

    public GameObject shopListPrefab;
    public Transform shopListParent;

    private BuildAndDemolish buildAndDemolish;
    private GameLogic gameLogic;

    void Start()
    {
        buildAndDemolish = GetComponent<BuildAndDemolish>();
        gameLogic = GetComponent<GameLogic>();

        LoadShop();
    }

    public void OpenShop()
    {
        shopPanel.SetActive(true);
        LoadMoney();
    }

    public void OpenMoneyShop()
    {
        moneyShopPanel.SetActive(true);
    }

    public void CloseShop()
    {
        shopPanel.SetActive(false);
    }

    public void CloseMoneyShop()
    {
        moneyShopPanel.SetActive(false);
    }

    public void CloseNotEnoughtMoney()
    {
        notEnoughtMoneyPanel.SetActive(false);
    }

    private void LoadShop()
    {
        for (int i = 0; i < buildAndDemolish.buildingList.Count; i++)
        {
            GameObject b = Instantiate(shopListPrefab, shopListParent);

            b.name = i.ToString();//change building name to building tag (machine, comnveyor) with id 
            b.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = buildAndDemolish.buildingList[i].image;
            b.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = buildAndDemolish.buildingList[i].name;
            b.transform.GetChild(0).GetChild(3).GetComponent<Text>().text = buildAndDemolish.buildingList[i].cost.ToString();
            b.GetComponent<Button>().onClick.AddListener(() => BuyBuilding());//add click listener for picking building to build           

            buildAndDemolish.buildingList[i].amountTextShopList = b.transform.GetChild(0).GetChild(2).GetComponent<Text>();
            buildAndDemolish.buildingList[i].amountTextShopList.text = buildAndDemolish.buildingList[i].amount.ToString();
        }
    }

    public void BuyBuilding()
    {
        int id = int.Parse(EventSystem.current.currentSelectedGameObject.name);//get name of clicked object 

        if (gameLogic.money >= buildAndDemolish.buildingList[id].cost)
        {
            gameLogic.GetMoney(-buildAndDemolish.buildingList[id].cost);
            LoadMoney();

            ++buildAndDemolish.buildingList[id].amount;
            buildAndDemolish.buildingList[id].amountTextShopList.text = buildAndDemolish.buildingList[id].amount.ToString();
            buildAndDemolish.buildingList[id].amountTextBuildList.text = buildAndDemolish.buildingList[id].amount.ToString();
            PlayerPrefs.SetInt(buildAndDemolish.buildingList[id].name, buildAndDemolish.buildingList[id].amount);
        }
        else
        {
            notEnoughtMoneyPanel.SetActive(true);
        }
    }

    public void LoadMoney()
    {
        shopMoneyText.text = gameLogic.money.ToString();
        gameMoneyText.text = gameLogic.money.ToString();
    }
}
