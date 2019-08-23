using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BuildAndDemolish : MonoBehaviour
{
    public TextAsset buildingsTextFile;

    public List<BuildingList> buildingList = new List<BuildingList>();
    public Image demolishButton; 
    public LayerMask groundLayerMask;
    public LayerMask demolishLayerMask;
    public LayerMask buildingLayerMask;

    [Header("Canvas")]
    public GameObject buildingListPrefab;
    public Transform canvasBuildingParent;

    private GameLogic gameLogic;

    private int pickedBuilding = -1;

    private BuildMode buildMode = BuildMode.Placing;

    void Start()
    {
        gameLogic = GetComponent<GameLogic>();

        LoadBuildings();
        LoadUI();
    }

    void Update()
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
                }
            }
        }
    }

    private void MouseClicks()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (buildMode == BuildMode.Placing)
            {
                if (pickedBuilding != -1)
                {
                    if (buildingList[pickedBuilding].amount > 0)
                    {
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //cast ray from mouse position
                        RaycastHit hit;

                        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayerMask.value)) //raycast for ingoring some layers
                        {
                            if (hit.transform.CompareTag("Grid"))
                            {
                                Vector3 pos = new Vector3(Mathf.Round(hit.point.x), 0, Mathf.Round(hit.point.z));//get current position

                                Collider[] hitColliders = Physics.OverlapSphere(pos, buildingList[pickedBuilding].size, buildingLayerMask.value);

                                if (hitColliders.Length > 0)
                                    Debug.Log("hit");
                                else
                                {
                                    GameObject b = Instantiate(buildingList[pickedBuilding].prefab, pos, Quaternion.identity);
                                    b.transform.name = pickedBuilding.ToString();

                                    --buildingList[pickedBuilding].amount;
                                    buildingList[pickedBuilding].amountTextBuildList.text = buildingList[pickedBuilding].amount.ToString();
                                    buildingList[pickedBuilding].amountTextShopList.text = buildingList[pickedBuilding].amount.ToString();
                                    PlayerPrefs.SetInt(buildingList[pickedBuilding].name, buildingList[pickedBuilding].amount);
                                }
                            }
                        }
                    }
                }
            }
            else if (buildMode == BuildMode.Removing)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //cast ray from mouse position
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, demolishLayerMask.value)) //raycast for ingoring some layers
                {
                    if (hit.transform.tag.Contains("Building"))
                    {
                        int id = int.Parse(hit.collider.name);
                        ++buildingList[id].amount;
                        buildingList[id].amountTextBuildList.text = buildingList[id].amount.ToString();
                        buildingList[id].amountTextShopList.text = buildingList[id].amount.ToString();
                        PlayerPrefs.SetInt(buildingList[id].name, buildingList[id].amount);

                        Destroy(hit.collider.gameObject);
                    }
                }
            }
        }
    }

    private void TouchPhaseBegan()
    {
        if (buildMode == BuildMode.Placing)
        {
            if (pickedBuilding != -1)
            {
                if (buildingList[pickedBuilding].amount > 0)
                {
                    if (!IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                    {
                        Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position); //cast ray from mouse position
                        RaycastHit hit;

                        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayerMask.value)) //raycast for ingoring some layers
                        {
                            if (hit.transform.CompareTag("Grid"))
                            {
                                Vector3 pos = new Vector3(Mathf.Round(hit.point.x), 0, Mathf.Round(hit.point.z));//get current position

                                Collider[] hitColliders = Physics.OverlapSphere(pos, buildingList[pickedBuilding].size, buildingLayerMask.value);

                                if (hitColliders.Length > 0)
                                    Debug.Log("hit");
                                else
                                {
                                    GameObject b = Instantiate(buildingList[pickedBuilding].prefab, pos, Quaternion.identity);
                                    b.transform.name = pickedBuilding.ToString();

                                    --buildingList[pickedBuilding].amount;
                                    buildingList[pickedBuilding].amountTextBuildList.text = buildingList[pickedBuilding].amount.ToString();
                                    buildingList[pickedBuilding].amountTextShopList.text = buildingList[pickedBuilding].amount.ToString();
                                    PlayerPrefs.SetInt(buildingList[pickedBuilding].name, buildingList[pickedBuilding].amount);
                                }
                            }
                        }
                    }
                }
            }
        }
        else if (buildMode == BuildMode.Removing)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position); //cast ray from mouse position
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, demolishLayerMask.value)) //raycast for ingoring some layers
            {
                if (hit.transform.tag.Contains("Building"))
                {
                    int id = int.Parse(hit.collider.name);
                    ++buildingList[id].amount;
                    buildingList[id].amountTextBuildList.text = buildingList[id].amount.ToString();
                    buildingList[id].amountTextShopList.text = buildingList[id].amount.ToString();
                    PlayerPrefs.SetInt(buildingList[id].name, buildingList[id].amount);

                    Destroy(hit.collider.gameObject);
                }
            }
        }
    }

    private void LoadBuildings()
    {
        string[] lines = buildingsTextFile.text.Split('\n');//split to lines

        for (int i = 1; i < lines.Length; i++)
        {
            string[] line = lines[i].Split('\t');//split line 

            BuildingList building = new BuildingList();

            building.name = line[0];
            building.image = Resources.Load("Images/Buildings/" + line[1], typeof(Sprite)) as Sprite;
            building.prefab = Resources.Load<GameObject>("Prefabs/Buildings/" + line[1]);
            building.amount = PlayerPrefs.GetInt(line[0]);
            building.cost = int.Parse(line[2]);
            building.size = float.Parse(line[3], System.Globalization.CultureInfo.InvariantCulture.NumberFormat);

            buildingList.Add(building);
        }
    }

    private void LoadUI()
    {
        for (int i = 0; i < buildingList.Count; i++)
        {
            GameObject b = Instantiate(buildingListPrefab, canvasBuildingParent); //create building button that contains name, image, counter of building

            b.name = i.ToString();//change building name to building tag (machine, comnveyor) with id 
            b.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = buildingList[i].image;
            b.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = buildingList[i].name;
            b.GetComponent<Button>().onClick.AddListener(() => PickBuilding());//add click listener for picking building to build

            buildingList[i].amountTextBuildList = b.transform.GetChild(0).GetChild(2).GetComponent<Text>();//add building counter text to variable text to increase or decrease when building or demolishing building
            buildingList[i].amountTextBuildList.text = buildingList[i].amount.ToString();
            buildingList[i].buildingListObject = b;
        }
    }

    public void PickBuilding()
    {
        if (gameLogic.statusMode == StatusMode.Build)
        {
            int id = int.Parse(EventSystem.current.currentSelectedGameObject.name);//get name of clicked object 

            if (pickedBuilding != id)
            {
                ShowPicked("Building", id);//call highlight building button that we picked
                pickedBuilding = id;

                SetBuildMode(BuildMode.Placing);
                // touchCamera.enabled = false;//off camera that is used for moving
            }
            else
            {
                SetBuildMode(BuildMode.None);
                //touchCamera.enabled = true;//anable camera for moving on map
                ShowPicked("", -2);//we dont want to highlight picked buildng anymore
                pickedBuilding = -1;//current building is none
            }
        }
    }

    public void PickDemolish()
    {
        if (gameLogic.statusMode == StatusMode.Build)
        {
            if (buildMode != BuildMode.Removing)
            {
                SetBuildMode(BuildMode.Removing);
                ShowPicked("", -1);
            }
            else
            {
                SetBuildMode(BuildMode.None);
                ShowPicked("", -2);
            }
        }
        pickedBuilding = -1;//current building is none
    }

    public void SetBuildMode(BuildMode bMode)
    {
        buildMode = bMode;
    }

    public void ShowPicked(string type, int j)
    {
        demolishButton.color = Color.white;

        //on every building in UI disable highlinght image
        for (int i = 0; i < buildingList.Count; i++)
        {
            buildingList[i].buildingListObject.GetComponent<Image>().enabled = false;
        }

        //if we picked building then j is greater than 0
        if (j >= 0)
        {
            buildingList[j].buildingListObject.GetComponent<Image>().enabled = true;
        }
        else if (j == -1)
        {
            //demolish button
            demolishButton.color = new Color(0f / 255f, 157 / 255f, 255f / 255f);
        }
    }

    private bool IsPointerOverGameObject(int fingerId)
    {
        EventSystem eventSystem = EventSystem.current;
        return (eventSystem.IsPointerOverGameObject(fingerId)
            && eventSystem.currentSelectedGameObject != null);
    }
}
