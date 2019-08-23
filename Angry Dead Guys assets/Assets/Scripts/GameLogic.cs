using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameLogic : MonoBehaviour
{
    public StatusMode statusMode;
    public GameObject mapGrid;

    public int money;
    public int night = 0;

    [Header("Lights")]
    public Light dayLight;
    public GameObject nightLight;

    [Header("FogOfWar")]
    public Renderer fogOfWar;
    public float maxFog = 50f;
    public float minFog = 19f;

    [Header("Canvas")]
    public GameObject gamePanel;
    public GameObject buildingListPanel;
    public GameObject joystick;
    public Text moneyText;
    public Text fuelText;
    public Text nightText;
    public GameObject popUpText;

    [Header("GameOver")]
    public GameObject gameOverPanel;
    public Text currentNights;
    public Text bestNights;

    [Header("PickUpMoney")]
    public int minMoney = 5;
    public int maxMoney = 50;

    private CharacterStats characterStats;
    private WaveSpawner waveSpawner;
    private AmmoBoxSpawner ammoBoxSpawner;
    private BuildAndDemolish buildAndDemolish;
    private Shop shop;

    void Start()
    {
        characterStats = GameObject.FindGameObjectWithTag("Character").GetComponent<CharacterStats>();
        waveSpawner = GetComponent<WaveSpawner>();
        ammoBoxSpawner = GetComponent<AmmoBoxSpawner>();
        buildAndDemolish = GetComponent<BuildAndDemolish>();
        shop = GetComponent<Shop>();

        gamePanel.SetActive(false);
        StartGame();
    }

    public void StartGame()
    {
        money = PlayerPrefs.GetInt("Money");
        moneyText.text = money.ToString();

        fuelText.text = PlayerPrefs.GetInt("FuelCanister").ToString();

        night = 0;
        nightText.text = night.ToString();

        StartDay();
    }

    public void StartNight()
    {
        statusMode = StatusMode.Play;
        buildAndDemolish.SetBuildMode(BuildMode.None);
        buildAndDemolish.ShowPicked("", -2);

        mapGrid.SetActive(false);
        StartCoroutine(WaitToStartSpawning(3));

        StartCoroutine(DayLight(1, 0, 1.5f, false));
        StartCoroutine(FogOfWar(maxFog, minFog, 1.5f));
        nightLight.SetActive(true);

        gamePanel.SetActive(true);
        buildingListPanel.SetActive(false);

        characterStats.ResetCharacter();
    }

    public void StartDay()
    {
        waveSpawner.waveState = WaveState.Waiting;
        StartCoroutine(DayLight(0, 1, 1.5f, true));
        StartCoroutine(FogOfWar(minFog, maxFog, 1.5f));
    }

    public void GameOver()
    {
        StartCoroutine(WaitToShowGameOverScreen());
    }

    public void GetMoney(int m)
    {
        money += m;
        moneyText.text = money.ToString();
        shop.LoadMoney();
        PlayerPrefs.SetInt("Money", money);
    }

    public void GetFuel()
    {
        int i = PlayerPrefs.GetInt("FuelCanister");
        i++;
        PlayerPrefs.SetInt("FuelCanister", i);
        fuelText.text = i.ToString();
    }

    private void ClearMap()
    {
        GameObject[] enemy = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < enemy.Length; i++)
        {
            Destroy(enemy[i]);
        }

        GameObject[] mine = GameObject.FindGameObjectsWithTag("Building/Mine");
        for (int i = 0; i < mine.Length; i++)
        {
            if (mine[i].transform.childCount == 0)
                Destroy(mine[i]);
        }

        GameObject[] mines = GameObject.FindGameObjectsWithTag("Building/Mines");
        for (int i = 0; i < mines.Length; i++)
        {
            if (mines[i].transform.childCount == 0)
                Destroy(mines[i]);
        }

        GameObject[] ammo = GameObject.FindGameObjectsWithTag("AmmoBox");
        for (int i = 0; i < ammo.Length; i++)
        {
            Destroy(ammo[i]);
        }

        GameObject[] bullet = GameObject.FindGameObjectsWithTag("Bullet");
        for (int i = 0; i < bullet.Length; i++)
        {
            Destroy(bullet[i]);
        }

        GameObject[] moneyO = GameObject.FindGameObjectsWithTag("Money");
        for (int i = 0; i < moneyO.Length; i++)
        {
            int m = Random.Range(minMoney, maxMoney);
            GetMoney(m);
            Destroy(moneyO[i]);
        }

        GameObject[] healthBox = GameObject.FindGameObjectsWithTag("HealthBox");
        for (int i = 0; i < healthBox.Length; i++)
        {
            Destroy(healthBox[i]);
        }
    }

    private void ResetGame()
    {
        gameOverPanel.SetActive(true);
        gamePanel.SetActive(false);

        currentNights.text = night.ToString();

        string name = SceneManager.GetActiveScene().name;

        if (PlayerPrefs.GetInt(name + "_Score") < night)
            PlayerPrefs.SetInt(name + "_Score", night);
        bestNights.text = PlayerPrefs.GetInt(name + "_Score").ToString();

        statusMode = StatusMode.Waiting;

        GameObject[] buildings = GameObject.FindGameObjectsWithTag("Building");
        for (int i = 0; i < buildings.Length; i++)
        {
            Destroy(buildings[i]);
        }

        for (int i = 0; i < buildAndDemolish.buildingList.Count; i++)
        {
            buildAndDemolish.buildingList[i].amount = 0;
            buildAndDemolish.buildingList[i].amountTextBuildList.text = "0";
            buildAndDemolish.buildingList[i].amountTextShopList.text = "0";
        }
    }

    private void StartingDay()
    {
        statusMode = StatusMode.Build;
        mapGrid.SetActive(true);

        nightLight.SetActive(false);

        gamePanel.SetActive(false);
        buildingListPanel.SetActive(true);

        characterStats.StopCharacter();

        nightText.text = night.ToString();

        ClearMap();
    }

    private IEnumerator DayLight(float a, float b, float duration, bool isDay)
    {
        float counter = 0f;

        while (counter < duration)
        {
            counter += Time.deltaTime;

            dayLight.intensity = Mathf.Lerp(a, b, counter / duration);

            yield return null;
        }

        if (isDay)
        {          
            StartingDay();
        }
        yield break;
    }

    private IEnumerator FogOfWar(float a, float b, float duration)
    {
        float counter = 0f;

        while (counter < duration)
        {
            counter += Time.deltaTime;

            fogOfWar.sharedMaterial.SetFloat("_FogRadius", Mathf.Lerp(a, b, counter / duration));

            yield return null;
        }
    }

    private IEnumerator WaitToStartSpawning(float time)
    {
        int counter = (int)time;
        while (counter > 0)
        {
            GameObject popUp = Instantiate(popUpText, gamePanel.transform);
            popUp.GetComponent<Text>().text = counter.ToString();
            Destroy(popUp, 1f);

            yield return new WaitForSeconds(1);
            counter--;
        }

        waveSpawner.SpawnWave();
        ammoBoxSpawner.SpawnAmmoBox();
    }

    private IEnumerator WaitToShowGameOverScreen()
    {
        yield return new WaitForSeconds(1.5f);

        ResetGame();
        ClearMap();
    }
}
