using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class DragAndDrop : MonoBehaviour
{

    [Header("MainInfo")]
    public LayerMask groundMask;//layer mask of ground

    public GameObject tileMap;//grid 
    public TouchCamera touchCamera;//script for camera to move and zoom
    public GameObject buildingsParent;//parent of buildings

    [Header("BuildingInfo")]
    public Transform building;//picked building
    public TypeOfSize typeOfSize;//type of size of picked building
    public int buildingSizeX, buildingSizeZ; //size x , z of picked building

    private GameLogic gameLogic;
    private Vector3 mapSize;

    private void Start()
    {
        gameLogic = GetComponent<GameLogic>();
        mapSize = new Vector3(GetComponent<LevelSetup>().sizeX, 0, GetComponent<LevelSetup>().sizeZ);//get size of map from level setup
    }

    private void Update()
    {
        if (!gameLogic.isPlaying)//if we are in build mode
        {
            if (gameLogic.constructionOperation == ConstructionOperation.None) //if construction operation is none
            {
                if (Application.platform == RuntimePlatform.WindowsEditor)//if we are in editor use mouse clicks
                    MouseClicks();
                else if (Application.platform == RuntimePlatform.Android)//if on android then touches
                {
                    if (Input.touchCount > 0)
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
        }
    }

    //mouse click for testing in editor 
    private void MouseClicks()
    {
        if (Input.GetMouseButtonDown(0))//on left mouse button click
        {
            if (!EventSystem.current.IsPointerOverGameObject())//if there is not any UI between mouse and building 
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//cast ray from mouse position
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    if (hit.transform.tag.Contains("Building") && hit.collider.tag != "UI" && hit.collider != null) //if we hit building
                    {
                        tileMap.SetActive(true);//enable grid
                        touchCamera.enabled = false;//disable camera script

                        if (!hit.collider.GetComponent<BuildingInfo>().startBuilding)//if is not start building
                        {
                            if (building != null) //if we have picked building
                            {
                                if (building.gameObject != hit.collider.gameObject)//if we another building
                                {
                                    building.GetComponent<BuildingInfo>().BuildingUnselect(); //the unselect previous building

                                    building = hit.collider.transform;//change picked building
                                    // get size of picked building
                                    typeOfSize = hit.collider.GetComponent<BuildingInfo>().buildingSize.typeOfSize;
                                    buildingSizeX = Mathf.FloorToInt(hit.collider.GetComponent<BuildingInfo>().buildingSize.x / 2);
                                    buildingSizeZ = Mathf.FloorToInt(hit.collider.GetComponent<BuildingInfo>().buildingSize.z / 2);

                                    //select picked building , show size, UI of building
                                    building.GetComponent<BuildingInfo>().BuildingSelect();
                                }
                            }
                            else//if picked building is empty 
                            {
                                building = hit.collider.transform;//get picked building
                                typeOfSize = hit.collider.GetComponent<BuildingInfo>().buildingSize.typeOfSize;

                                buildingSizeX = Mathf.FloorToInt(building.gameObject.GetComponent<BuildingInfo>().buildingSize.x / 2);
                                buildingSizeZ = Mathf.FloorToInt(building.gameObject.GetComponent<BuildingInfo>().buildingSize.z / 2);

                                building.GetComponent<BuildingInfo>().BuildingSelect();
                            }
                        }
                    }
                    //if we hit ground 
                    else if ((hit.collider.CompareTag("Ground") || hit.collider.CompareTag("Hierarchy/TileMap")) && hit.collider.tag != "UI" && hit.collider.tag != "Building" && hit.collider != null)
                    {
                        tileMap.SetActive(false);//disable grid
                        touchCamera.enabled = true;//enable camera script

                        if (building != null) //if we have picked building
                        {
                            //unselect picked building
                            building.GetComponent<BuildingInfo>().BuildingUnselect();

                            building = null;//we dont have picked building any more
                        }
                    }
                }
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (building != null)//if picked building is not empty
            {
                if (!EventSystem.current.IsPointerOverGameObject())//if we dont hit UI
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//cast ray from mouse position
                RaycastHit hit;

                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundMask.value)) //raycast for only ground layer
                    {
                        if (building.GetComponent<BuildingInfo>().isSelected)//if picked building is selected to move
                        {
                            //get position of mouse
                            Vector3 pos = new Vector3(Mathf.Round(hit.point.x / gameLogic.GetComponent<LevelSetup>().tileSize), 1, Mathf.Round(hit.point.z / gameLogic.GetComponent<LevelSetup>().tileSize));

                            if (hit.collider != null && hit.collider.CompareTag("Hierarchy/TileMap")) //if wi hit grid with tag "Hierarchy/TileMap"
                            {
                                if (typeOfSize == TypeOfSize.uneven) //if building size is 1x1, 3x3, 5x5
                                {

                                    //check position of building if is not out of grid
                                    if (pos.x < buildingSizeX)
                                        pos.x = buildingSizeX;
                                    else if (pos.x > mapSize.x - buildingSizeX - 1)
                                        pos.x = mapSize.x - buildingSizeX - 1;

                                    if (pos.z < buildingSizeZ)
                                        pos.z = buildingSizeZ;
                                    else if (pos.z > mapSize.z - buildingSizeZ - 1)
                                        pos.z = mapSize.z - buildingSizeZ - 1;

                                    //then move it
                                    building.position = new Vector3(pos.x, pos.y, pos.z);
                                }
                                else if (typeOfSize == TypeOfSize.paired)//if building is 2x2, 4x4
                                {                                
                                    if (pos.x < buildingSizeX)
                                        pos.x = buildingSizeX - 1;
                                    else if (pos.x > mapSize.x - buildingSizeX - 1)
                                        pos.x = mapSize.x - buildingSizeX - 1;

                                    if (pos.z < buildingSizeZ)
                                        pos.z = buildingSizeZ - 1;
                                    else if (pos.z > mapSize.z - buildingSizeZ - 1)
                                        pos.z = mapSize.z - buildingSizeZ - 1;

                                    building.position = new Vector3(pos.x + 0.5f, pos.y, pos.z + 0.5f);
                                }
                                else //if building size is various
                                {
                                    /* //if(rotation) // parna strana smer x
                                     if (hit.collider.transform.position.x >= buildingSize.x && hit.collider.transform.position.x <= size.x - buildingSize.x)
                                         transform.position = new Vector3(hit.collider.transform.position.x, hit.collider.transform.position.y + 1, transform.position.z);
                                     if (hit.collider.transform.position.z >= buildingSize.y && hit.collider.transform.position.z <= size.y - buildingSize.y)
                                         transform.position = new Vector3(transform.position.x, hit.collider.transform.position.y + 1, hit.collider.transform.position.z);
                                     //else //parna strana y*/
                                }
                            }
                        }
                    }
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (building != null) //if we have still picked building
            {
                if (building.CompareTag("Building/Pipe")) //if picked building is pipe
                    building.GetComponent<Pipe>().CheckSides();//then call on pipe to check in she is connected to another pipe
            }
        }
    }

    //touches for android
    private void TouchPhaseBegan()
    {
        if (!IsPointerOverGameObject(Input.GetTouch(0).fingerId))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.transform.tag.Contains("Building") && hit.collider.tag != "UI" && hit.collider != null)
                {
                    tileMap.SetActive(true);//zapne grid
                    touchCamera.enabled = false;

                    if (!hit.collider.GetComponent<BuildingInfo>().startBuilding)
                    {
                        if (building != null)
                        {
                            if (building.gameObject != hit.collider.gameObject)
                            {
                                building.GetComponent<BuildingInfo>().BuildingUnselect();

                                building = hit.collider.transform;
                                typeOfSize = hit.collider.GetComponent<BuildingInfo>().buildingSize.typeOfSize;

                                buildingSizeX = Mathf.FloorToInt(hit.collider.GetComponent<BuildingInfo>().buildingSize.x / 2);
                                buildingSizeZ = Mathf.FloorToInt(hit.collider.GetComponent<BuildingInfo>().buildingSize.z / 2);

                                building.GetComponent<BuildingInfo>().BuildingSelect();
                            }
                        }
                        else
                        {
                            building = hit.collider.transform;
                            typeOfSize = hit.collider.GetComponent<BuildingInfo>().buildingSize.typeOfSize;

                            buildingSizeX = Mathf.FloorToInt(building.gameObject.GetComponent<BuildingInfo>().buildingSize.x / 2);
                            buildingSizeZ = Mathf.FloorToInt(building.gameObject.GetComponent<BuildingInfo>().buildingSize.z / 2);

                            building.GetComponent<BuildingInfo>().BuildingSelect();
                        }
                    }
                }
                else if ((hit.collider.CompareTag("Ground") || hit.collider.CompareTag("Hierarchy/TileMap")) && hit.collider.tag != "UI" && hit.collider.tag != "Building" && hit.collider != null)
                {
                    tileMap.SetActive(false);//vypne grid
                    touchCamera.enabled = true;

                    if (building != null)
                    {
                        building.GetComponent<BuildingInfo>().BuildingUnselect();

                        building = null;
                    }
                }
            }
        }
    }

    private void TouchPhaseMoved()
    {
        if (!IsPointerOverGameObject(Input.GetTouch(0).fingerId))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundMask.value))
            {
                if (building.GetComponent<BuildingInfo>().isSelected)
                {
                    Vector3 pos = new Vector3(Mathf.Round(hit.point.x / gameLogic.GetComponent<LevelSetup>().tileSize), 1, Mathf.Round(hit.point.z / gameLogic.GetComponent<LevelSetup>().tileSize));

                    if (hit.collider != null && hit.collider.CompareTag("Hierarchy/TileMap"))
                    {
                        if (typeOfSize == TypeOfSize.uneven)
                        {
                            if (pos.x < buildingSizeX)
                                pos.x = buildingSizeX;
                            else if (pos.x > mapSize.x - buildingSizeX - 1)
                                pos.x = mapSize.x - buildingSizeX - 1;

                            if (pos.z < buildingSizeZ)
                                pos.z = buildingSizeZ;
                            else if (pos.z > mapSize.z - buildingSizeZ - 1)
                                pos.z = mapSize.z - buildingSizeZ - 1;

                            building.position = new Vector3(pos.x, pos.y, pos.z);
                        }
                        else if (typeOfSize == TypeOfSize.paired)
                        {
                            //kontrola a oprava pozicie
                            if (pos.x < buildingSizeX)
                                pos.x = buildingSizeX - 1;
                            else if (pos.x > mapSize.x - buildingSizeX - 1)
                                pos.x = mapSize.x - buildingSizeX - 1;

                            if (pos.z < buildingSizeZ)
                                pos.z = buildingSizeZ - 1;
                            else if (pos.z > mapSize.z - buildingSizeZ - 1)
                                pos.z = mapSize.z - buildingSizeZ - 1;

                            building.position = new Vector3(pos.x + 0.5f, pos.y, pos.z + 0.5f);
                        }
                        else
                        {
                            /* //if(rotation) // parna strana smer x
                             if (hit.collider.transform.position.x >= buildingSize.x && hit.collider.transform.position.x <= size.x - buildingSize.x)
                                 transform.position = new Vector3(hit.collider.transform.position.x, hit.collider.transform.position.y + 1, transform.position.z);
                             if (hit.collider.transform.position.z >= buildingSize.y && hit.collider.transform.position.z <= size.y - buildingSize.y)
                                 transform.position = new Vector3(transform.position.x, hit.collider.transform.position.y + 1, hit.collider.transform.position.z);
                             //else //parna strana y*/
                        }
                    }
                }
            }
        }
    }

    private void TouchPhaseEnded()
    {
        if (building != null)
        {
            if (building.CompareTag("Building/Pipe"))
                building.GetComponent<Pipe>().CheckSides();
        }
    }

    private bool IsPointerOverGameObject(int fingerId)
    {
        EventSystem eventSystem = EventSystem.current;
        return (eventSystem.IsPointerOverGameObject(fingerId)
            && eventSystem.currentSelectedGameObject != null);
    }
}
