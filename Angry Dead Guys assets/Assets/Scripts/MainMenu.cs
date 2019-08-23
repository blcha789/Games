using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    public MapList[] map;
    public Text fuelCanisterCounter;
    public Fade fade;
    public MenuAdManager adManager;
    public Text gameVersion;
    public ScrollSnapRect scrollSnapRect;

    [Header("MapList")]
    public GameObject mapPrefab;
    public GameObject mapPanel;
    public Transform mapParent;

    public GameObject pageIndicator;
    public Transform pageList;

    [Header("Panels")]
    public GameObject unlockMapPanel;
    public GameObject notEnoughtMoneyPanel;
    public GameObject shopPanel;

    private List<Map> maps =  new List<Map>();

    void Start()
    {
        //PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("Map_0", 1);
        gameVersion.text = "v" + Application.version;

        CreateMapList();
        RefreshFuelCanister(0);
    }

    private void CreateMapList()
    {    
        int counter = 0;

        for (int i = 0; i < map.Length; i++)
        {
            Transform _mapPanel = Instantiate(mapPanel, mapParent).transform;
            Instantiate(pageIndicator, pageList);

            for (int j = 0; j < map[i].mapList.Length; j++)
            {
                GameObject m = Instantiate(mapPrefab, _mapPanel);
                m.name = "Map_" + counter;

                m.transform.GetChild(0).GetComponent<Text>().text = map[i].mapList[j].name;
                m.GetComponent<Image>().sprite = map[i].mapList[j].image;
                m.GetComponent<Button>().onClick.AddListener(() => ChooseMap());

                map[i].mapList[j].obj = m.transform;
                maps.Add(map[i].mapList[j]);

                if (PlayerPrefs.GetInt("Map_" + counter) == 1)//unlocked
                {
                    m.transform.GetChild(1).gameObject.SetActive(false);

                    m.transform.GetChild(2).gameObject.SetActive(true);
                    m.transform.GetChild(3).gameObject.SetActive(false);

                    m.transform.GetChild(2).GetComponentInChildren<Text>().text = PlayerPrefs.GetInt("Map_" + counter + "_Score").ToString();
                }
                else
                {
                    m.transform.GetChild(2).gameObject.SetActive(false);
                    m.transform.GetChild(3).gameObject.SetActive(true);

                    m.transform.GetChild(3).GetComponentInChildren<Text>().text = map[i].mapList[j].price.ToString();
                }
                counter++;
            }
        }
        scrollSnapRect.enabled = true;
    }

    private void RefreshMapList()
    {
        for (int i = 0; i < maps.Count; i++)
        {
            if (PlayerPrefs.GetInt("Map_" + i) == 1)//unlocked
            {
                maps[i].obj.GetChild(1).gameObject.SetActive(false);

                maps[i].obj.transform.GetChild(2).gameObject.SetActive(true);
                maps[i].obj.transform.GetChild(3).gameObject.SetActive(false);

                maps[i].obj.transform.GetChild(2).GetComponentInChildren<Text>().text = PlayerPrefs.GetInt("Map_" + i + "_Score").ToString();
            }
            else
            {
                maps[i].obj.transform.GetChild(2).gameObject.SetActive(false);
                maps[i].obj.transform.GetChild(3).gameObject.SetActive(true);

                maps[i].obj.transform.GetChild(3).GetComponentInChildren<Text>().text = maps[i].price.ToString();
            }
        }
    }

    public void RefreshFuelCanister(int i)
    {
        int f = PlayerPrefs.GetInt("FuelCanister");
        f += i;
        PlayerPrefs.SetInt("FuelCanister", f);
        fuelCanisterCounter.text = f.ToString();
    }

    public void ChooseMap()
    {
        string name = EventSystem.current.currentSelectedGameObject.name;
        string[] splittedName = name.Split('_');
        int i = int.Parse(splittedName[1]);

        if (PlayerPrefs.GetInt(name) == 0)
        {
            unlockMapPanel.SetActive(true);

            unlockMapPanel.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "Unlock " + maps[i].name + "?";
            unlockMapPanel.transform.GetChild(0).GetChild(1).GetComponent<Image>().sprite = maps[i].image;
            unlockMapPanel.transform.GetChild(0).GetChild(2).GetChild(1).GetComponent<Text>().text = maps[i].price.ToString();
            unlockMapPanel.transform.GetChild(0).GetChild(2).GetComponent<Button>().onClick.RemoveAllListeners();
            unlockMapPanel.transform.GetChild(0).GetChild(2).GetComponent<Button>().onClick.AddListener(() => UnlockMap(i));
        }
        else
            fade.FadeToLevel(name);
    }

    public void UnlockMap(int i)
    {
        if (maps[i].price <= PlayerPrefs.GetInt("FuelCanister"))
        {
            PlayerPrefs.SetInt("Map_" + i, 1);
            RefreshFuelCanister(-maps[i].price);
            RefreshMapList();
            unlockMapPanel.SetActive(false);
        }
        else
        {
            unlockMapPanel.SetActive(false);
            notEnoughtMoneyPanel.SetActive(true);
        }
    }

    public void OpenShop()
    {
        notEnoughtMoneyPanel.SetActive(false);
        shopPanel.SetActive(true);
    }

    public void GetFuelCanister() //button
    {
        adManager.ShowRewardFuelVideoAd();
    }

    public void GetFuelCanTest()
    {
        RefreshFuelCanister(500);
        PlayerPrefs.SetInt("Money", 13125);
    }

    public void Close()
    {
        unlockMapPanel.SetActive(false);
        notEnoughtMoneyPanel.SetActive(false);
        shopPanel.SetActive(false);
    }

    public void Exit()
    {
        Application.Quit();
    }
}

[System.Serializable]
public class MapList
{
    public string name;
    public Map[] mapList;
}

[System.Serializable]
public class Map
{
    public string name;
    public Sprite image;
    public int price;
    public Transform obj;
}
