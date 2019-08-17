using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;

public class BuildingList : MonoBehaviour
{

    [Header("Main parameters")]
    public Transform canvasParent;
    public Transform buildingsParent;
    public GameObject buildingListPrefab;
    public GameObject tileMap;
    public TouchCamera touchCamera;
    public LayerMask mask;//ground
    public GameObject arrowPrefab;
    public ChooseBuilding[] chooseBuildings;//if empty == all buildings
    public TextAsset buildingTextAsset;

    [Header("Other info")]
    public CurrentBuilding currentBuilding;

    private List<GameObject> tempBuildings = new List<GameObject>();
    private List<GameObject> tempBuildingsStaff = new List<GameObject>();
    private List<GameObject> buildingList = new List<GameObject>();

    public List<Building> buildings = new List<Building>();
    public List<Image> buttonList = new List<Image>();

    private GameLogic gameLogic;
    private LevelSetup levelSetup;
    private DragAndDrop dragAndDrop;
    private UndoSystem undoSystem;

    private TypeOfSize buildingTypeOfSize;
    private int buildingSizeX, buildingSizeZ;
    private Vector3 pos, lastPos, currentPos, currentLastPos, size;
    private float countZ, countX;
    private bool startedBuilding = false;

    private void Start()
    {
        gameLogic = GetComponent<GameLogic>();
        levelSetup = GetComponent<LevelSetup>();
        dragAndDrop = GetComponent<DragAndDrop>();
        undoSystem = GetComponent<UndoSystem>();

        currentBuilding.i = -1;

        if (!levelSetup.sandbox)//if is not sandbox scene then load setup game 
        {
            GameSetup();
        }
    }

    public void GameSetup()
    {
        size = new Vector3(gameLogic.GetComponent<LevelSetup>().sizeX, 0, gameLogic.GetComponent<LevelSetup>().sizeZ);//get size of map from LevelSetup script

        LoadBuildings();
        CreateUIBuildings();
    }

    private void Update()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor) //if is playing in editor then use mouse clicks
            MouseClicks();
        else if (Application.platform == RuntimePlatform.Android) //if is on android then use touch
        {
            {
                Touch touch = Input.GetTouch(0);

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        TouchPhaseBegan();
                        break;

                    case TouchPhase.Moved:
                        TouchPhaseMoved();
                        break;

                    case TouchPhase.Ended:
                        TouchPhaseEnded();
                        break;
                }
            }
        }
    }


    //load buildings from text file
    private void LoadBuildings()
    {
        string[] lines = buildingTextAsset.text.Split('\n');//split lines

        for (int i = 1; i < lines.Length; i++)
        {
            string[] line = lines[i].Split('\t');//split line 

            for (int j = 0; j < chooseBuildings.Length; j++)
            {
                if (int.Parse(line[0]) == chooseBuildings[j].id)
                {
                    Building buildingOnLoad = new Building(); //create building
                    BuildingSize buildingSizeOnLoad = new BuildingSize(); //create building size

                    buildingOnLoad.name = line[2];//add name of building
                    buildingOnLoad.tag = line[3];//tag building

                    buildingSizeOnLoad.typeOfSize = (TypeOfSize)int.Parse(line[4]); //type of size (1x1, 2x2,3x3, 5x5)
                    buildingSizeOnLoad.x = int.Parse(line[5]);//size x
                    buildingSizeOnLoad.z = int.Parse(line[6]);//size z

                    buildingOnLoad.buildingSize = buildingSizeOnLoad;//add building size to building

                    buildingOnLoad.image = Resources.Load("Images/Buildings/" + line[7], typeof(Sprite)) as Sprite; //load building image
                    buildingOnLoad.prefab = Resources.Load<GameObject>("Prefabs/Buildings/" + line[7]);//load building prefab
                    buildingOnLoad.amount = chooseBuildings[j].amount;//how many buidlings we can use in game
                    buildingOnLoad.Description = line[8];//description of building

                    buildings.Add(buildingOnLoad);//add building to list of buildings
                }
            }
        }
    }

    //add buildings we load to UI
    private void CreateUIBuildings()
    {
        for (int i = 0; i < buildings.Count; i++)
        {
            GameObject b = Instantiate(buildingListPrefab, canvasParent); //create building button that contains name, image, counter of building
            b.name = buildings[i].tag + ";" + i.ToString();//change building name to building tag (machine, comnveyor) with id 
            b.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = buildings[i].image;
            b.transform.GetChild(0).GetChild(2).GetComponent<Text>().text = buildings[i].name;
            b.GetComponent<Button>().onClick.AddListener(() => PickBuilding());//add click listener for picking building to build

            buildings[i].amountText = b.GetComponentInChildren<Text>();//add building counter text to variable text to increase or decrease when building or demolishing building
            buildings[i].amountText.text = buildings[i].amount.ToString();

            buildingList.Add(b);
        }
    }

    //this function is called when picking building
    public void PickBuilding()
    {
        string[] name = EventSystem.current.currentSelectedGameObject.name.Split(';');//get name of clicked object 
        int i = int.Parse(name[1]);//get id of building

        if (currentBuilding.i != i)
        {
            ShowPicked("Building", i);//call highlight building button that we picked

            if (name[0].Equals("Machine")) //if is machine
            {
                currentBuilding.i = i;
                currentBuilding.buildingType = "Machine";
            }
            else //if is conveyor or pipe
            {
                currentBuilding.i = i;
                currentBuilding.buildingType = "ConveyorPipe";
            }

            tileMap.SetActive(true);//active tile map for placing building on grid
            gameLogic.constructionOperation = ConstructionOperation.Build;//set construction operation to build
            touchCamera.enabled = false;//off camera that is used for moving 
        }
        else 
        {
            tileMap.SetActive(false); //deactive tile map
            gameLogic.constructionOperation = ConstructionOperation.None; //construction operation none
            touchCamera.enabled = true;//anable camera for moving on map
            ShowPicked("", -1);//we dont want to highlight picked buildng anymore
            currentBuilding.i = -1;//current building is none
        }

        if (dragAndDrop.building != null)
            dragAndDrop.building.GetComponent<BuildingInfo>().BuildingUnselect();//un select building that we had builded
    }

    public void ShowPicked(string type, int j)
    {
        //on every building in UI disable highlinght image
        for (int i = 0; i < buildingList.Count; i++)
        {
            buildingList[i].GetComponent<Image>().enabled = false;
        }

        for (int i = 0; i < buttonList.Count; i++)
        {
            buttonList[i].enabled = false;
        }

        //if we picked building then j is greater than 0
        if (j >= 0)
        {
            if (type == "Building") //if is building then enable highlight image
            {
                buildingList[j].GetComponent<Image>().enabled = true;
            }
            else //or is it conveor or pipe
            {
                buttonList[j].enabled = true;
            }
        }
    }

    //mouse click to test game in editor 
    private void MouseClicks()
    {
        if (Input.GetMouseButtonDown(0)) //if we pressed left mouse button 
        {
            if (gameLogic.constructionOperation == ConstructionOperation.Build) //if we are in building mode
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //cast ray from mouse position
                RaycastHit hit;

                if (!EventSystem.current.IsPointerOverGameObject())//call function for check if there is not between mouse and grid
                {
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask.value)) //raycast for ingoring some layers
                    {
                        if (hit.transform.CompareTag("Hierarchy/TileMap"))//if we hit tile map with tag "Hierarchy/TileMap"
                        {
                            pos = new Vector3(Mathf.Round(hit.point.x / levelSetup.tileSize), 1, Mathf.Round(hit.point.z / levelSetup.tileSize));//get current position

                            //add building size to drag and drop script
                            dragAndDrop.typeOfSize = buildings[currentBuilding.i].buildingSize.typeOfSize;//get picked building type of size
                            dragAndDrop.buildingSizeX = Mathf.FloorToInt(buildings[currentBuilding.i].buildingSize.x / 2);//get picked buildíng x size
                            dragAndDrop.buildingSizeZ = Mathf.FloorToInt(buildings[currentBuilding.i].buildingSize.z / 2);//get picked buildíng z size

                            //add building size to this script
                            buildingTypeOfSize = buildings[currentBuilding.i].buildingSize.typeOfSize;
                            buildingSizeX = Mathf.FloorToInt(buildings[currentBuilding.i].buildingSize.x / 2);
                            buildingSizeZ = Mathf.FloorToInt(buildings[currentBuilding.i].buildingSize.z / 2);

                            startedBuilding = true;//we started building 

                            if (currentBuilding.buildingType.Equals("ConveyorPipe"))//if picked building is conveyor or pipe
                            {

                                //we check if building is not out of grid if is then we change position where will be placed
                                if (pos.x < 0)
                                    pos.x = 0;
                                else if (pos.x > size.x - 1)
                                    pos.x = size.x - 1;
                                if (pos.z < 0)
                                    pos.z = 0;
                                else if (pos.z > size.z - 1)
                                    pos.z = size.z - 1;

                                GameObject b = Instantiate(buildings[currentBuilding.i].prefab, new Vector3(pos.x, pos.y, pos.z), Quaternion.identity);
                                b.transform.name = currentBuilding.i.ToString();
                                tempBuildings.Add(b);
                            }
                            else if (currentBuilding.buildingType.Equals("Machine")) //if picked building is machine
                            {
                                if (buildings[currentBuilding.i].buildingSize.typeOfSize == TypeOfSize.paired) //if building is 2x2 or 4x4
                                {
                                    //we check if building is not out of grid if is then we change position where will be placed
                                    if (pos.x < buildingSizeX)
                                        pos.x = buildingSizeX - 1;
                                    else if (pos.x > size.x - buildingSizeX - 1)
                                        pos.x = size.x - buildingSizeX - 1;

                                    if (pos.z < buildingSizeZ)
                                        pos.z = buildingSizeZ - 1;
                                    else if (pos.z > size.z - buildingSizeZ - 1)
                                        pos.z = size.z - buildingSizeZ - 1;


                                    //when position is ok then we create building 
                                    GameObject b = Instantiate(buildings[currentBuilding.i].prefab, new Vector3(pos.x + .5f, pos.y, pos.z + .5f), Quaternion.identity);
                                    tempBuildings.Add(b);

                                    dragAndDrop.building = b.transform;
                                }
                                else if (buildings[currentBuilding.i].buildingSize.typeOfSize == TypeOfSize.uneven)//if building is 1x1, 3x3 , 5x5
                                {
                                    if (pos.x < buildingSizeX)
                                        pos.x = buildingSizeX;
                                    else if (pos.x > size.x - buildingSizeX - 1)
                                        pos.x = size.x - buildingSizeX - 1;

                                    if (pos.z < buildingSizeZ)
                                        pos.z = buildingSizeZ;
                                    else if (pos.z > size.z - buildingSizeZ - 1)
                                        pos.z = size.z - buildingSizeZ - 1;

                                    GameObject b = Instantiate(buildings[currentBuilding.i].prefab, new Vector3(pos.x, pos.y, pos.z), Quaternion.identity);
                                    tempBuildings.Add(b);

                                    dragAndDrop.building = b.transform;
                                }
                                else
                                {

                                }
                            }
                        }
                    }
                }
            }
        }

        if (Input.GetMouseButton(0)) //if we still have pressed left mouse button we can move building
        {
            countZ = 0;
            countX = 0;

            if (gameLogic.constructionOperation == ConstructionOperation.Build)// if is build mode
            {
                if (startedBuilding)//if we started building
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//cast ray from mouse position
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask.value))//raycast for ignoring some layers
                    {
                        if (hit.transform.CompareTag("Hierarchy/TileMap") && hit.collider != null) //if we hit tile map
                        {
                            currentPos = new Vector3(Mathf.Round(hit.point.x / levelSetup.tileSize), 1, Mathf.Round(hit.point.z / levelSetup.tileSize));//current position of mouse

                            if (currentBuilding.buildingType.Equals("ConveyorPipe"))//if pciked building is conveyor or pipe
                            {
                                //here we create line of conveyor and pipes
                                if (currentPos.x >= 0 && currentPos.z >= 0 && currentPos.x < size.x && currentPos.z < size.z)
                                {
                                    if (currentPos == currentLastPos)
                                        return;
                                    else
                                    {
                                        currentLastPos = currentPos;

                                        for (int i = 0; i < buildingsParent.childCount; i++)
                                        {
                                            buildingsParent.GetChild(i).GetComponent<BuildingInfo>().TriggerExit();
                                        }

                                        for (int i = 0; i < tempBuildingsStaff.Count; i++)
                                        {
                                            Destroy(tempBuildingsStaff[i]);
                                        }
                                        tempBuildingsStaff.Clear();

                                        for (int i = 0; i < tempBuildings.Count; i++)
                                        {
                                            Destroy(tempBuildings[i]);
                                        }
                                        tempBuildings.Clear();

                                        Vector3 length = pos - currentPos;

                                        if (length.z > 0)
                                        {
                                            GameObject startArrow = Instantiate(arrowPrefab, new Vector3(pos.x, 1f, pos.z), Quaternion.Euler(0, 0, 0));
                                            tempBuildingsStaff.Add(startArrow);

                                            if (length.x == 0)
                                            {
                                                GameObject endArrow = Instantiate(arrowPrefab, new Vector3(currentPos.x, 1f, currentPos.z), Quaternion.Euler(0, 0, 0));
                                                tempBuildingsStaff.Add(endArrow);
                                            }
                                        }
                                        else if (length.z < 0)
                                        {
                                            GameObject startArrow = Instantiate(arrowPrefab, new Vector3(pos.x, 1f, pos.z), Quaternion.Euler(0, 180, 0));
                                            tempBuildingsStaff.Add(startArrow);

                                            if (length.x == 0)
                                            {
                                                GameObject endArrow = Instantiate(arrowPrefab, new Vector3(currentPos.x, 1f, currentPos.z), Quaternion.Euler(0, 180, 0));
                                                tempBuildingsStaff.Add(endArrow);
                                            }
                                        }

                                        if (length.x < 0)
                                        {
                                            GameObject endArrow = Instantiate(arrowPrefab, new Vector3(currentPos.x, 1f, currentPos.z), Quaternion.Euler(0, -90, 0));
                                            tempBuildingsStaff.Add(endArrow);

                                            if (length.z == 0)
                                            {
                                                GameObject startArrow = Instantiate(arrowPrefab, new Vector3(pos.x, 1f, pos.z), Quaternion.Euler(0, -90, 0));
                                                tempBuildingsStaff.Add(startArrow);
                                            }
                                        }
                                        else if (length.x > 0)
                                        {
                                            GameObject endArrow = Instantiate(arrowPrefab, new Vector3(currentPos.x, 1f, currentPos.z), Quaternion.Euler(0, 90, 0));
                                            tempBuildingsStaff.Add(endArrow);

                                            if (length.z == 0)
                                            {
                                                GameObject startArrow = Instantiate(arrowPrefab, new Vector3(pos.x, 1f, pos.z), Quaternion.Euler(0, 90, 0));
                                                tempBuildingsStaff.Add(startArrow);
                                            }
                                        }

                                        //--------
                                        if (Mathf.Abs(length.x) == 0)
                                        {
                                            for (int j = 0; j < Mathf.Abs(length.z) + 1; j++)
                                            {
                                                if (length.z > 0) //+
                                                {
                                                    GameObject b = Instantiate(buildings[currentBuilding.i].prefab, new Vector3(pos.x + countX, 1, pos.z + countZ), Quaternion.identity);
                                                    b.transform.name = currentBuilding.i.ToString();
                                                    b.transform.GetChild(2).rotation = Quaternion.Euler(0, 90, 0);
                                                    tempBuildings.Add(b);
                                                    countZ -= 1;
                                                }
                                                else if (length.z < 0) //-
                                                {
                                                    GameObject b = Instantiate(buildings[currentBuilding.i].prefab, new Vector3(pos.x + countX, 1, pos.z + countZ), Quaternion.identity);
                                                    b.transform.name = currentBuilding.i.ToString();
                                                    b.transform.GetChild(2).rotation = Quaternion.Euler(0, -90, 0);
                                                    tempBuildings.Add(b);
                                                    countZ += 1;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            for (int j = 0; j < Mathf.Abs(length.z); j++)
                                            {
                                                if (length.z > 0) //+
                                                {
                                                    GameObject b = Instantiate(buildings[currentBuilding.i].prefab, new Vector3(pos.x + countX, 1, pos.z + countZ), Quaternion.identity);
                                                    b.transform.name = currentBuilding.i.ToString();
                                                    b.transform.GetChild(2).rotation = Quaternion.Euler(0, 90, 0);
                                                    tempBuildings.Add(b);
                                                    countZ -= 1;
                                                }
                                                else if (length.z < 0) //-
                                                {
                                                    GameObject b = Instantiate(buildings[currentBuilding.i].prefab, new Vector3(pos.x + countX, 1, pos.z + countZ), Quaternion.identity);
                                                    b.transform.name = currentBuilding.i.ToString();
                                                    b.transform.GetChild(2).rotation = Quaternion.Euler(0, -90, 0);
                                                    tempBuildings.Add(b);
                                                    countZ += 1;
                                                }
                                            }

                                            for (int i = 0; i < Mathf.Abs(length.x) + 1; i++)
                                            {
                                                if (length.x < 0) //+
                                                {
                                                    GameObject b = Instantiate(buildings[currentBuilding.i].prefab, new Vector3(pos.x + countX, 1, pos.z + countZ), Quaternion.identity);
                                                    b.transform.name = currentBuilding.i.ToString();
                                                    b.transform.GetChild(2).rotation = Quaternion.Euler(0, 0, 0);
                                                    tempBuildings.Add(b);
                                                    countX += 1;
                                                }
                                                else if (length.x > 0) //-
                                                {
                                                    GameObject b = Instantiate(buildings[currentBuilding.i].prefab, new Vector3(pos.x + countX, 1, pos.z + countZ), Quaternion.identity);
                                                    b.transform.name = currentBuilding.i.ToString();
                                                    b.transform.GetChild(2).rotation = Quaternion.Euler(0, 180, 0);
                                                    tempBuildings.Add(b);
                                                    countX -= 1;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else if (currentBuilding.buildingType.Equals("Machine")) //if picked building is machine
                            {
                                if (buildingTypeOfSize == TypeOfSize.uneven)//building size is 1x1,3x3, 5x5
                                {
                                    //we check position of building if is not out of grid
                                    if (currentPos.x < buildingSizeX)
                                        currentPos.x = buildingSizeX;
                                    else if (currentPos.x > size.x - buildingSizeX - 1)
                                        currentPos.x = size.x - buildingSizeX - 1;

                                    if (currentPos.z < buildingSizeZ)
                                        currentPos.z = buildingSizeZ;
                                    else if (currentPos.z > size.z - buildingSizeZ - 1)
                                        currentPos.z = size.z - buildingSizeZ - 1;

                                    //then we move it
                                    tempBuildings[0].transform.position = new Vector3(currentPos.x, currentPos.y, currentPos.z);
                                }
                                else if (buildingTypeOfSize == TypeOfSize.paired)//building size is 2x2, 4x4
                                {
                                    if (currentPos.x < buildingSizeX)
                                        currentPos.x = buildingSizeX - 1;
                                    else if (currentPos.x > size.x - buildingSizeX - 1)
                                        currentPos.x = size.x - buildingSizeX - 1;

                                    if (currentPos.z < buildingSizeZ)
                                        currentPos.z = buildingSizeZ - 1;
                                    else if (currentPos.z > size.z - buildingSizeZ - 1)
                                        currentPos.z = size.z - buildingSizeZ - 1;

                                    tempBuildings[0].transform.position = new Vector3(currentPos.x + 0.5f, currentPos.y, currentPos.z + 0.5f);
                                }
                                else
                                {

                                }
                            }
                        }
                    }
                }
            }
        }

        if (Input.GetMouseButtonUp(0))//end of building 
        {
            if (gameLogic.constructionOperation == ConstructionOperation.Build)//if is still build mode
            {
                if (currentBuilding.buildingType.Equals("ConveyorPipe"))//if picked building is conveyor
                {
                    if (tempBuildings.Count > 0) //if is more than 0 conveyors or pipes
                    {
                        Undo();//call function to add all conveyors or pipes to undo system

                        currentBuilding.buildingType = "";//we dont have picked any building anymore
                        currentBuilding.i = -1;
                        gameLogic.constructionOperation = ConstructionOperation.None;//construction operation is none
                        ShowPicked("", -1);//we dont have picked any building any more
                        tileMap.SetActive(false);//deactive tile map(grid)
                    }
                }
                else if (currentBuilding.buildingType.Equals("Machine"))//if picked building is machine
                {             
                    for (int i = 0; i < tempBuildings.Count; i++)
                    {
                        if (buildings[currentBuilding.i].amount >= 1)//chceck if we have enougth buildings to place
                        {
                            Undo();//call function to add building to undo system

                            tempBuildings[i].transform.name = currentBuilding.i.ToString();//change name of building to id of building
                            tempBuildings[i].transform.parent = buildingsParent;//move building gameobject to building parent
                            BuildingsCount(currentBuilding.i, -1);//decrease building counter

                            currentBuilding.buildingType = "";
                            currentBuilding.i = -1;
                            gameLogic.constructionOperation = ConstructionOperation.None;
                            ShowPicked("", -1);
                        }
                        else
                        {
                            Destroy(tempBuildings[i]);
                        }
                    }
                    tempBuildings.Clear();//clear list of temporary buildings
                }
            }
            startedBuilding = false;//end of building
        }
    }

    //touches for android
    private void TouchPhaseBegan()
    {
        if (gameLogic.constructionOperation == ConstructionOperation.Build)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit hit;

            if (!IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask.value))
                {
                    if (hit.transform.CompareTag("Hierarchy/TileMap"))
                    {
                        pos = new Vector3(Mathf.Round(hit.point.x / levelSetup.tileSize), 1, Mathf.Round(hit.point.z / levelSetup.tileSize));

                        dragAndDrop.typeOfSize = buildings[currentBuilding.i].buildingSize.typeOfSize;
                        dragAndDrop.buildingSizeX = Mathf.FloorToInt(buildings[currentBuilding.i].buildingSize.x / 2);
                        dragAndDrop.buildingSizeZ = Mathf.FloorToInt(buildings[currentBuilding.i].buildingSize.z / 2);

                        buildingTypeOfSize = buildings[currentBuilding.i].buildingSize.typeOfSize;
                        buildingSizeX = Mathf.FloorToInt(buildings[currentBuilding.i].buildingSize.x / 2);
                        buildingSizeZ = Mathf.FloorToInt(buildings[currentBuilding.i].buildingSize.z / 2);

                        startedBuilding = true;

                        if (currentBuilding.buildingType.Equals("ConveyorPipe"))
                        {
                            if (pos.x < 0)
                                pos.x = 0;
                            else if (pos.x > size.x - 1)
                                pos.x = size.x - 1;
                            if (pos.z < 0)
                                pos.z = 0;
                            else if (pos.z > size.z - 1)
                                pos.z = size.z - 1;

                            GameObject b = Instantiate(buildings[currentBuilding.i].prefab, new Vector3(pos.x, pos.y, pos.z), Quaternion.identity);
                            b.transform.name = currentBuilding.i.ToString();
                            tempBuildings.Add(b);
                        }
                        else if (currentBuilding.buildingType.Equals("Machine"))
                        {
                            if (buildings[currentBuilding.i].buildingSize.typeOfSize == TypeOfSize.paired)//párny
                            {
                                //kontrola a oprava pozicie
                                if (pos.x < buildingSizeX)
                                    pos.x = buildingSizeX - 1;
                                else if (pos.x > size.x - buildingSizeX - 1)
                                    pos.x = size.x - buildingSizeX - 1;

                                if (pos.z < buildingSizeZ)
                                    pos.z = buildingSizeZ - 1;
                                else if (pos.z > size.z - buildingSizeZ - 1)
                                    pos.z = size.z - buildingSizeZ - 1;

                                GameObject b = Instantiate(buildings[currentBuilding.i].prefab, new Vector3(pos.x + .5f, pos.y, pos.z + .5f), Quaternion.identity);
                                tempBuildings.Add(b);

                                dragAndDrop.building = b.transform;
                            }
                            else if (buildings[currentBuilding.i].buildingSize.typeOfSize == TypeOfSize.uneven)//nepárny
                            {
                                if (pos.x < buildingSizeX)
                                    pos.x = buildingSizeX;
                                else if (pos.x > size.x - buildingSizeX - 1)
                                    pos.x = size.x - buildingSizeX - 1;

                                if (pos.z < buildingSizeZ)
                                    pos.z = buildingSizeZ;
                                else if (pos.z > size.z - buildingSizeZ - 1)
                                    pos.z = size.z - buildingSizeZ - 1;

                                GameObject b = Instantiate(buildings[currentBuilding.i].prefab, new Vector3(pos.x, pos.y, pos.z), Quaternion.identity);
                                tempBuildings.Add(b);

                                dragAndDrop.building = b.transform;
                            }
                            else // ostatné
                            {

                            }
                        }
                    }
                }
            }
        }
    }

    private void TouchPhaseMoved()
    {
        countZ = 0;
        countX = 0;

        if (gameLogic.constructionOperation == ConstructionOperation.Build)
        {
            if (startedBuilding)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask.value))
                {
                    if (hit.transform.CompareTag("Hierarchy/TileMap") && hit.collider != null)
                    {
                        currentPos = new Vector3(Mathf.Round(hit.point.x / levelSetup.tileSize), 1, Mathf.Round(hit.point.z / levelSetup.tileSize));

                        if (currentBuilding.buildingType.Equals("ConveyorPipe"))
                        {
                            if (currentPos.x >= 0 && currentPos.z >= 0 && currentPos.x < size.x && currentPos.z < size.z)
                            {
                                if (currentPos == currentLastPos)
                                    return;
                                else
                                {
                                    currentLastPos = currentPos;

                                    for (int i = 0; i < buildingsParent.childCount; i++)
                                    {
                                        buildingsParent.GetChild(i).GetComponent<BuildingInfo>().TriggerExit();
                                    }

                                    for (int i = 0; i < tempBuildingsStaff.Count; i++)
                                    {
                                        Destroy(tempBuildingsStaff[i]);
                                    }
                                    tempBuildingsStaff.Clear();

                                    for (int i = 0; i < tempBuildings.Count; i++)
                                    {
                                        Destroy(tempBuildings[i]);
                                    }

                                    tempBuildings.Clear();

                                    Vector3 length = pos - currentPos;

                                    if (length.z > 0)
                                    {
                                        GameObject startArrow = Instantiate(arrowPrefab, new Vector3(pos.x, 1f, pos.z), Quaternion.Euler(0, 0, 0));
                                        tempBuildingsStaff.Add(startArrow);

                                        if (length.x == 0)
                                        {
                                            GameObject endArrow = Instantiate(arrowPrefab, new Vector3(currentPos.x, 1f, currentPos.z), Quaternion.Euler(0, 0, 0));
                                            tempBuildingsStaff.Add(endArrow);
                                        }
                                    }
                                    else if (length.z < 0)
                                    {
                                        GameObject startArrow = Instantiate(arrowPrefab, new Vector3(pos.x, 1f, pos.z), Quaternion.Euler(0, 180, 0));
                                        tempBuildingsStaff.Add(startArrow);

                                        if (length.x == 0)
                                        {
                                            GameObject endArrow = Instantiate(arrowPrefab, new Vector3(currentPos.x, 1f, currentPos.z), Quaternion.Euler(0, 180, 0));
                                            tempBuildingsStaff.Add(endArrow);
                                        }
                                    }

                                    if (length.x < 0)
                                    {
                                        GameObject endArrow = Instantiate(arrowPrefab, new Vector3(currentPos.x, 1f, currentPos.z), Quaternion.Euler(0, -90, 0));
                                        tempBuildingsStaff.Add(endArrow);

                                        if (length.z == 0)
                                        {
                                            GameObject startArrow = Instantiate(arrowPrefab, new Vector3(pos.x, 1f, pos.z), Quaternion.Euler(0, -90, 0));
                                            tempBuildingsStaff.Add(startArrow);
                                        }
                                    }
                                    else if (length.x > 0)
                                    {
                                        GameObject endArrow = Instantiate(arrowPrefab, new Vector3(currentPos.x, 1f, currentPos.z), Quaternion.Euler(0, 90, 0));
                                        tempBuildingsStaff.Add(endArrow);

                                        if (length.z == 0)
                                        {
                                            GameObject startArrow = Instantiate(arrowPrefab, new Vector3(pos.x, 1f, pos.z), Quaternion.Euler(0, 90, 0));
                                            tempBuildingsStaff.Add(startArrow);
                                        }
                                    }


                                    if (Mathf.Abs(length.x) == 0)
                                    {
                                        for (int j = 0; j < Mathf.Abs(length.z) + 1; j++)
                                        {
                                            if (length.z > 0) //+
                                            {
                                                GameObject b = Instantiate(buildings[currentBuilding.i].prefab, new Vector3(pos.x + countX, 1, pos.z + countZ), Quaternion.identity);
                                                b.transform.name = currentBuilding.i.ToString();
                                                b.transform.GetChild(2).rotation = Quaternion.Euler(0, 90, 0);
                                                tempBuildings.Add(b);
                                                countZ -= 1;
                                            }
                                            else if (length.z < 0) //-
                                            {
                                                GameObject b = Instantiate(buildings[currentBuilding.i].prefab, new Vector3(pos.x + countX, 1, pos.z + countZ), Quaternion.identity);
                                                b.transform.name = currentBuilding.i.ToString();
                                                b.transform.GetChild(2).rotation = Quaternion.Euler(0, -90, 0);
                                                tempBuildings.Add(b);
                                                countZ += 1;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        for (int j = 0; j < Mathf.Abs(length.z); j++)
                                        {
                                            if (length.z > 0) //+
                                            {
                                                GameObject b = Instantiate(buildings[currentBuilding.i].prefab, new Vector3(pos.x + countX, 1, pos.z + countZ), Quaternion.identity);
                                                b.transform.name = currentBuilding.i.ToString();
                                                b.transform.GetChild(2).rotation = Quaternion.Euler(0, 90, 0);
                                                tempBuildings.Add(b);
                                                countZ -= 1;
                                            }
                                            else if (length.z < 0) //-
                                            {
                                                GameObject b = Instantiate(buildings[currentBuilding.i].prefab, new Vector3(pos.x + countX, 1, pos.z + countZ), Quaternion.identity);
                                                b.transform.name = currentBuilding.i.ToString();
                                                b.transform.GetChild(2).rotation = Quaternion.Euler(0, -90, 0);
                                                tempBuildings.Add(b);
                                                tempBuildings.Add(b);
                                                countZ += 1;
                                            }
                                        }

                                        for (int i = 0; i < Mathf.Abs(length.x) + 1; i++)
                                        {
                                            if (length.x < 0) //+
                                            {
                                                GameObject b = Instantiate(buildings[currentBuilding.i].prefab, new Vector3(pos.x + countX, 1, pos.z + countZ), Quaternion.identity);
                                                b.transform.name = currentBuilding.i.ToString();
                                                b.transform.GetChild(2).rotation = Quaternion.Euler(0, 0, 0);
                                                tempBuildings.Add(b);
                                                countX += 1;
                                            }
                                            else if (length.x > 0) //-
                                            {
                                                GameObject b = Instantiate(buildings[currentBuilding.i].prefab, new Vector3(pos.x + countX, 1, pos.z + countZ), Quaternion.identity);
                                                b.transform.name = currentBuilding.i.ToString();
                                                b.transform.GetChild(2).rotation = Quaternion.Euler(0, 180, 0);
                                                tempBuildings.Add(b);
                                                countX -= 1;
                                            }
                                        }
                                    }
                                }
                            }
                            else if (currentBuilding.buildingType.Equals("Machine"))
                            {
                                if (buildingTypeOfSize == TypeOfSize.uneven)
                                {
                                    if (currentPos.x < buildingSizeX)
                                        currentPos.x = buildingSizeX;
                                    else if (currentPos.x > size.x - buildingSizeX - 1)
                                        currentPos.x = size.x - buildingSizeX - 1;

                                    if (currentPos.z < buildingSizeZ)
                                        currentPos.z = buildingSizeZ;
                                    else if (currentPos.z > size.z - buildingSizeZ - 1)
                                        currentPos.z = size.z - buildingSizeZ - 1;

                                    tempBuildings[0].transform.position = new Vector3(currentPos.x, currentPos.y, currentPos.z);
                                }
                                else if (buildingTypeOfSize == TypeOfSize.paired)
                                {
                                    if (currentPos.x < buildingSizeX)
                                        currentPos.x = buildingSizeX - 1;
                                    else if (currentPos.x > size.x - buildingSizeX - 1)
                                        currentPos.x = size.x - buildingSizeX - 1;

                                    if (currentPos.z < buildingSizeZ)
                                        currentPos.z = buildingSizeZ - 1;
                                    else if (currentPos.z > size.z - buildingSizeZ - 1)
                                        currentPos.z = size.z - buildingSizeZ - 1;

                                    tempBuildings[0].transform.position = new Vector3(currentPos.x + 0.5f, currentPos.y, currentPos.z + 0.5f);
                                }
                                else
                                {

                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private void TouchPhaseEnded()
    {
        if (gameLogic.constructionOperation == ConstructionOperation.Build)
        {
            if (currentBuilding.buildingType.Equals("ConveyorPipe"))
            {
                if (tempBuildings.Count > 0)
                {
                    Undo();

                    currentBuilding.buildingType = "";
                    currentBuilding.i = -1;
                    gameLogic.constructionOperation = ConstructionOperation.None;
                    ShowPicked("", -1);
                }
            }
            else if (currentBuilding.buildingType.Equals("Machine"))
            {
                for (int i = 0; i < tempBuildings.Count; i++)
                {
                    if (buildings[currentBuilding.i].amount >= 1)
                    {
                        Undo();

                        tempBuildings[i].transform.name = currentBuilding.i.ToString();
                        tempBuildings[i].transform.parent = buildingsParent;
                        BuildingsCount(currentBuilding.i, -1);

                        currentBuilding.buildingType = "";
                        currentBuilding.i = -1;
                        gameLogic.constructionOperation = ConstructionOperation.None;
                        ShowPicked("", -1);
                    }
                    else
                    {
                        Destroy(tempBuildings[i]);
                    }
                }
                tempBuildings.Clear();
            }
        }
        startedBuilding = false;
    }

    //check if pointer(touch) is not over any gameobject like UI
    private bool IsPointerOverGameObject(int fingerId)
    {
        EventSystem eventSystem = EventSystem.current;
        return (eventSystem.IsPointerOverGameObject(fingerId)
            && eventSystem.currentSelectedGameObject != null);
    }

    public void BuildingsCount(int i, int b)
    {
        buildings[i].amount += b;
        buildings[i].amountText.text = buildings[i].amount.ToString();
    }

    private void Undo()
    {
        undoSystem.action.Add("Build");//action was build 

        BuildList buildList = new BuildList(); //create build list of all buildnigs that was builded with that action
        buildList.iBuilding = currentBuilding.i;
        buildList.buildings = tempBuildings.ToArray();

        if (currentBuilding.buildingType.Equals("ConveyorPipe"))
            buildList.iBuildingCorountine = StartCoroutine("Wait"); //call corountit to place conveyors or building in row

        undoSystem.buildList.Add(buildList);
    }

    private IEnumerator Wait()
    {
        List<GameObject> temporalBuildings = new List<GameObject>(tempBuildings);//move all temp buildings to temporary building
        int curBuilding = currentBuilding.i; //get current picked building
        tempBuildings.Clear();//clear list or temp buildings

        for (int i = 0; i < tempBuildingsStaff.Count; i++)//destroy arrows that show direction of conveyors
        {
            Destroy(tempBuildingsStaff[i]);
        }
        tempBuildingsStaff.Clear();

        for (int i = 0; i < temporalBuildings.Count; i++)//deactive all conveyors and pipes
        {
            temporalBuildings[i].SetActive(false);
        }

        foreach (Transform building in buildingsParent)//call to all buildings that they are not colliding 
        {
            building.GetComponent<BuildingInfo>().TriggerExit();
        }

        for (int i = 0; i < temporalBuildings.Count; i++)//for each conveyror/pipe 
        {
            temporalBuildings[i].SetActive(true);//set active
            temporalBuildings[i].GetComponent<BuildingInfo>().BuildingUnselect();//unselect conveyor

            if (temporalBuildings[i].GetComponent<BuildingInfo>().buildingStatus == BuildingStatus.colliding)//if is conveyor coliding then dont place it (destroy it)
                Destroy(temporalBuildings[i]);
            else//if is not coliding
            {
                if (buildings[curBuilding].amount >= 1)//then if we have enought conveyors or pipes
                {
                    temporalBuildings[i].transform.parent = buildingsParent;//change parent of conveyor
                    BuildingsCount(curBuilding, -1);//decrease conveyors 
                }
                else//if we dont have enought conveyors then dont place it
                {
                    Destroy(temporalBuildings[i]);
                }
                yield return new WaitForSeconds(0.2f);//wait then place another conveyor
            }
        }
    }
}

