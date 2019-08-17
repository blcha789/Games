using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum ConstructionOperation { None, Build, Demolish }

public class GameLogic : MonoBehaviour
{
    public bool isPlaying = false; //play mode or build mode
    public ConstructionOperation constructionOperation = ConstructionOperation.None; //construction operation(build, demolish, none)
    public bool isThisLastLevel = false; //if this is last level of all levels

    public GameObject pickedBuilding;
    public Text fpsCounter;

    [Header("Panels")]
    public GameObject assemblyPanel;
    public GameObject endLevelPanel;
    public GameObject endFreePlayPanel;
    public GameObject pausePanel;
    public GameObject helpPanel;
    public GameObject somethingWrongPanel;

    private LevelSetup levelSetup;
    private bool levelComplete = false;

    private float deltaTime = 0.0f;

    private void Start()
    {
        levelSetup = GetComponent<LevelSetup>();
    }

    private void Update()
    {
        if(isPlaying && !SceneManager.GetActiveScene().name.Equals("BuildingSceneLevel"))//whe in play mode we are checking if level is completed
        {
            if (!levelComplete)
                CheckIfLevelDone();
        }

        FpsCounter();
    }

    private void CheckIfLevelDone()
    {
        int buyerCountDone = 0; //set buyers counter to 0

        //go thru every buyer in scene
        for (int i = 0; i < levelSetup.buyerInput.Length; i++)
        {
            if (levelSetup.buyerInput[i].buyer.itemCount <= 0 && levelSetup.buyerInput[i].buyer.fluidCount <= 0) //check if we get all item / fluid that he needed
                //if he have every item he needed , we increase done buyers
                buyerCountDone++;
        }

        //if done buyer counter is equal to all buyers in scene 
        if (buyerCountDone == levelSetup.buyerInput.Length)
        {
            GetComponent<GameButtons>().pause();//set pause
            UnlockNextLevel();//unlock next level
            levelComplete = true;//set level is completed


            //increase builded factories
            int i = PlayerPrefs.GetInt("BuildedFactories") + 1;
            PlayerPrefs.SetInt("BuildedFactories", i);

            if (!levelSetup.sandbox) //chcek if is not this level sandbox
            {
                endLevelPanel.SetActive(true); //enable end panel
                if (isThisLastLevel) //if this is last level of all levels
                    endLevelPanel.transform.GetChild(0).GetChild(0).gameObject.SetActive(false); //then dont show next level button
            }
            else
                endFreePlayPanel.SetActive(true);//enable end panele for sandbox scene
        }
    }

    private void UnlockNextLevel()
    {
        if (!levelSetup.sandbox)
        {
            int i = SceneManager.GetActiveScene().buildIndex + 1;//get next level scene number

            string path = SceneUtility.GetScenePathByBuildIndex(i);
            int slash = path.LastIndexOf('/');
            string name = path.Substring(slash + 1);
            int dot = name.LastIndexOf('.');

            string sceneName = name.Substring(0, dot);

            //save that we unlocked next level
            PlayerPrefs.SetInt(sceneName, 1);
        }
    }

    private void FpsCounter()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        fpsCounter.text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
    }
}
