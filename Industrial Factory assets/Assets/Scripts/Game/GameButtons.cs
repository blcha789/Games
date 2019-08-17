using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;


//Script for every Ui button in scene
public class GameButtons : MonoBehaviour {

    public Animator buildingListPanel;
    public GameObject deleteBuildingPanel;
    public Fade fade;
    public GameObject tileMap;
    public Transform rotateCamera;
    public TouchCamera touchCamera;
    public bool isInputsOutputsShowed = false;

    [Header("Objects parent")]
    public Transform itemParent;
    public Transform buildingParent;
    public Transform depositParent;

    [Header("Buttons")]
    public GameObject playButton;
    public GameObject stopButton;
    public Button undoButton;

    [Header("")]
    public Image showInputsOutputsIcon;
    public Sprite[] showInputsOutputsIcons;

    private float rotPosCamera;

    private GameLogic gameLogic;
    private UndoSystem undoSystem;
    private BuildingList buildingList;

    private bool rotating = false;

    private void Start()
    {
        gameLogic = GetComponent<GameLogic>();
        undoSystem = GetComponent<UndoSystem>();
        buildingList = GetComponent<BuildingList>();
    }

    public void Pause()
    {
        pause();
        gameLogic.pausePanel.SetActive(true);
    } //pause game 

    public void NextLevel() //open next level when we complete this level
    {
        //we want to get scene number of current scene then increse scene number and load next scene
        int i = SceneManager.GetActiveScene().buildIndex + 1; 
        string path = SceneUtility.GetScenePathByBuildIndex(i);
        int slash = path.LastIndexOf('/');
        string name = path.Substring(slash + 1);
        int dot = name.LastIndexOf('.');

        string sceneName = name.Substring(0, dot);

        Time.timeScale = 1;
        AudioListener.pause = false;
        fade.FadeToLevel(sceneName);
    }

    public void ToMenu()
    {
        Time.timeScale = 1;
        fade.FadeToLevel("LevelMenu");
    }

    public void Again()
    {
        Time.timeScale = 1;
        Scene scene = SceneManager.GetActiveScene();
        fade.FadeToLevel(scene.name);
    }

    public void Continue()
    {
        Time.timeScale = 1;
        touchCamera.enabled = true;
        gameLogic.pausePanel.SetActive(false);
        AudioListener.pause = false;
        StartCoroutine(wait());
    }

    public void CloseAssemblyList()
    {
        gameLogic.assemblyPanel.SetActive(false);
        tileMap.SetActive(true);
        touchCamera.enabled = true;
    }

    public void Thrash() // this function is setting construction operation to demolish
    {
        if (GetComponent<DragAndDrop>().building != null)
            GetComponent<DragAndDrop>().building.GetComponent<BuildingInfo>().BuildingUnselect();

        if (gameLogic.constructionOperation == ConstructionOperation.Build || gameLogic.constructionOperation == ConstructionOperation.None)
        {
            GetComponent<BuildingList>().ShowPicked("Button", 0);
            gameLogic.constructionOperation = ConstructionOperation.Demolish;
            GetComponent<BuildingList>().currentBuilding.i = -1;
            tileMap.SetActive(false);
            touchCamera.enabled = false;
        }
        else
        {
            GetComponent<BuildingList>().ShowPicked("", -1);
            gameLogic.constructionOperation = ConstructionOperation.None;
            touchCamera.enabled = true;
        }
    }

    public void YesDelete() //function that we want to delete selected buildings
    {
        deleteBuildingPanel.SetActive(false);//deactive panel
        touchCamera.enabled = true;//enable camera script
        GetComponent<BuildingList>().ShowPicked("", -1);

        undoSystem.action.Add("Demolish");//add action to undo system
        DemolishList demolishList = new DemolishList();//create demolish list

        //add all building that was deleted to demolish list 
        foreach (Transform item in buildingParent)
        {
            if (item.GetComponent<BuildingInfo>().isSelected && item.GetComponent<BuildingInfo>().typeOfBuilding != TypeOfBuilding.startBuilding)
            {
                demolishList.buildings.Add(item.gameObject);
                buildingList.BuildingsCount(int.Parse(item.name), 1);

                item.GetComponent<BuildingInfo>().TriggerExit();
                item.GetComponent<BuildingInfo>().isSelected = false;
                item.gameObject.SetActive(false);
            }
        }
        undoSystem.demolishList.Add(demolishList); //add demolish list to undo system list

        StartCoroutine(wait());
    }

    public void NoDelete()
    {
        deleteBuildingPanel.SetActive(false);
        touchCamera.enabled = true;
        GetComponent<BuildingList>().ShowPicked("", -1);

        foreach (Transform item in buildingParent)
        {
            if (item.GetComponent<BuildingInfo>().isSelected && item.GetComponent<BuildingInfo>().typeOfBuilding != TypeOfBuilding.startBuilding)
            {
                item.GetComponent<BuildingInfo>().TriggerExit();
                item.GetComponent<BuildingInfo>().isSelected = false;
            }
        }
        StartCoroutine(wait());
    }

    public void ShowInputsOutputs()//this function is for showing what item we need to put in which input of building
    {
        if (isInputsOutputsShowed)
        {
            showInputsOutputsIcon.sprite = showInputsOutputsIcons[0];
            foreach (Transform building in buildingParent)
            {
                if (building.tag != "Building/Pipe" || building.tag != "Building/Conveyor" || building.tag != "Building/PipeBridge" || building.tag != "Building/ConveyorPipeBridge" || building.tag != "Building/ConveyorBridge")
                    building.GetComponent<BuildingsUI>().buildingsItemsParent.SetActive(false);
            }
        }
        else
        {
            showInputsOutputsIcon.sprite = showInputsOutputsIcons[1];
            foreach (Transform building in buildingParent)
            {
                if (building.tag != "Building/Pipe" || building.tag != "Building/Conveyor" || building.tag != "Building/PipeBridge" || building.tag != "Building/ConveyorPipeBridge" || building.tag != "Building/ConveyorBridge")
                    building.GetComponent<BuildingsUI>().buildingsItemsParent.SetActive(true);
            }
        }
        isInputsOutputsShowed = !isInputsOutputsShowed;
    }

    public void RotateCameraLeft()//smooth rotate camera to left by 90 degree
    {
        if (!rotating)
        {
            StartCoroutine(RotateCamera(new Vector3(0, 90, 0), 0.5f));
            ChangeBuildingsCanvasItemsRotation(new Vector3(0, 90, 0), 0.5f);
        }
    }

    public void RotateCameraRight()
    {
        if (!rotating)
        {
            StartCoroutine(RotateCamera(new Vector3(0, -90, 0), 0.5f));
            ChangeBuildingsCanvasItemsRotation(new Vector3(0, -90, 0), 0.5f);
        }
    }//smooth rotate camera to right by 90 degree

    public void Play() //set play mode
    {
        if (GetComponent<DragAndDrop>().building != null)
            GetComponent<DragAndDrop>().building.GetComponent<BuildingInfo>().BuildingUnselect(); //unselect selected building

        Time.timeScale = 1;
        if (!gameLogic.isPlaying) //if are are not in play mode
        {
            bool canPlay = true; //we can change mode to play 

            //check if any building is coliding, if any building is coliding then we need to fix it
            for (int i = 0; i < buildingParent.childCount; i++)
            {
                if (buildingParent.GetChild(i).GetComponent<BuildingInfo>().typeOfBuilding == TypeOfBuilding.miningDrill)
                    buildingParent.GetChild(i).GetComponent<MiningDrill>().FindDeposit();

                if (buildingParent.GetChild(i).GetComponent<BuildingInfo>().typeOfBuilding == TypeOfBuilding.fluidRig)
                    buildingParent.GetChild(i).GetChild(2).GetChild(2).GetComponentInChildren<FluidRig>().FindDeposit();

                if (buildingParent.GetChild(i).GetComponent<BuildingInfo>().buildingStatus == BuildingStatus.colliding)
                    canPlay = false;
            }

            if (canPlay) //if is everything ok 
            {            
                buildingListPanel.SetTrigger("ListOut"); //we hide UI panel of buildings
                gameLogic.isPlaying = true; //set play mode to true
                gameLogic.constructionOperation = ConstructionOperation.None;//set construction operation to none
                GetComponent<BuildingList>().ShowPicked("", -1);//we deselect picked building in UI
                GetComponent<BuildingList>().currentBuilding.i = -1;
                tileMap.SetActive(false);//disable grid
                touchCamera.enabled = true;//enable camera script

                //change Play UI button to Build UI Button
                playButton.SetActive(false);
                stopButton.SetActive(true);
                //we cant use undo button in play mode
                undoButton.interactable = false;
            }
            else//if something is wrng it will show panel with warning
            {
                gameLogic.somethingWrongPanel.SetActive(true);
            }
        }      
    }

    public void Stop()
    {
        if (gameLogic.isPlaying)
        {
            buildingListPanel.SetTrigger("ListIn"); //show building panel
            gameLogic.isPlaying = false; //set play mode to false (set to building mode)
            Time.timeScale = 1;

            //change Build UI Button to Build UI Button
            playButton.SetActive(true);
            stopButton.SetActive(false);
            //we can use undo button
            undoButton.interactable = true;

            //set every building to default state
            foreach (Transform building in buildingParent)
            {
                if (building.CompareTag("Building/Assembler"))
                    building.GetComponent<Assembler>().SetDefaults();
                else if (building.CompareTag("Building/Refinery"))
                    building.GetComponent<Refinery>().SetDefaults();
                else if (building.CompareTag("Building/Mixer"))
                    building.GetComponent<Solidifier>().SetDefaults();
                else if (building.CompareTag("Building/Furnace"))
                    building.GetComponent<Furnace>().SetDefaults();
                else if (building.CompareTag("Building/OreCrusher"))
                    building.GetComponent<OreCrusher>().SetDefaults();
                else if (building.CompareTag("Building/Pipe"))
                    building.GetComponent<Pipe>().SetDefaults();
                else if (building.CompareTag("Building/MiningDrill"))
                    building.GetComponent<MiningDrill>().SetDefaults();
                else if (building.CompareTag("Building/OilRig"))
                    building.GetChild(2).GetChild(2).GetComponentInChildren<FluidRig>().SetDefaults();
            }
            //set every deposit to default state
            foreach(Transform deposit in depositParent)
            {
                if (deposit.CompareTag("Deposit/Ore"))
                    deposit.GetComponent<OreDeposit>().SetDefaults();
                else if (deposit.CompareTag("Deposit/Fluid"))
                    deposit.GetComponent<FluidDeposit>().SetDefaults();
            }
            //set pipe bridges to default state
            GameObject[] obj = GameObject.FindGameObjectsWithTag("PipeBridge");
            for (int i = 0; i < obj.Length; i++)
            {
                obj[i].GetComponent<PipeBridge>().SetDefaults();
            }           
            //destroy every item that was created
            for (int i = 0; i < itemParent.childCount; i++)
            {
                Destroy(itemParent.GetChild(i).gameObject);
            }
        }
    } //stop play mode , set build mode

    public void FastForward()
    {
        if (gameLogic.isPlaying)
            Time.timeScale = 2;
    }

    public void Help()
    {
        pause();
        gameLogic.helpPanel.SetActive(true);
    }

    public void CloseHelpPanel()
    {
        Time.timeScale = 1;
        gameLogic.helpPanel.SetActive(false);
        touchCamera.enabled = true;
    }

    public void pause()
    {
        gameLogic.constructionOperation = ConstructionOperation.None;
        GetComponent<BuildingList>().ShowPicked("", -1);
        GetComponent<BuildingList>().currentBuilding.i = -1;
        tileMap.SetActive(false);
        touchCamera.enabled = false;
        Time.timeScale = 0;
        AudioListener.pause = true;
    }

    public void Undo()
    {
        GetComponent<UndoSystem>().Undo();
    }

    public void Ok()
    {
        gameLogic.somethingWrongPanel.SetActive(false);
    }

    public void CreateFreePlayGame()
    {
        GetComponent<LevelSetupFreePlay>().Setup();
    }

    //this function is to checking if in input field is number between min and max value for taht inputfield
    public void MinMaxInputField(string minMax)
    {
        int result;
        int number;
        string[] splitMinMax = minMax.Split(','); //we have min and max number stored in string , because we cant use two integers for input field(On Edit End)

        if (!int.TryParse(EventSystem.current.currentSelectedGameObject.GetComponent<InputField>().text, out result))//we check if in input field is number
        {
            //if not then number will be 0
            number = 1;
        }
        else
        {
            //if yes then we will use that number
            number = int.Parse(EventSystem.current.currentSelectedGameObject.GetComponent<InputField>().text);
        }

        number = Mathf.Clamp(number, int.Parse(splitMinMax[0]), int.Parse(splitMinMax[1]));

        EventSystem.current.currentSelectedGameObject.GetComponent<InputField>().text = number.ToString();
    }

    private IEnumerator wait()
    {
        yield return new WaitForSeconds(0.3f);
        gameLogic.constructionOperation = ConstructionOperation.None;
    }
        
    //function to smooth rotate camera by 90 degree
    private IEnumerator RotateCamera(Vector3 angles, float duration)
    {
        rotating = true;
        Quaternion startRotation = rotateCamera.rotation;
        Quaternion endRotation = Quaternion.Euler(angles) * startRotation;
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            rotateCamera.rotation = Quaternion.Lerp(startRotation, endRotation, t / duration);
            yield return null;
        }
        rotateCamera.rotation = endRotation;
        rotating = false;
    }

    private void ChangeBuildingsCanvasItemsRotation(Vector3 angles, float duration)
    {
        foreach (Transform building in buildingParent)
        {
            if (building.gameObject.activeSelf)           
                building.GetComponent<BuildingsUI>().ChangeCanvasItemsRotation(angles, duration);
        }
    }
}
