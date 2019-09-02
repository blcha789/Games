using System.Collections;
using UnityEngine;

public class BuildingButtons : MonoBehaviour {

    private GameLogic gameLogic;
    private BuildingList buildingList;
    private AssemblyList assemblyList;
    private DragAndDrop dragAndDrop;
    private UndoSystem undoSystem;

    public Transform rotateObject;

    private Transform buildingsParent;
    private GameObject tileMap;
    private BuildingInfo buildingInfo;
    private Storage storage;

    private bool rotating = false;

    private void Start()
    {
        GameObject GL = GameObject.FindGameObjectWithTag("Hierarchy/GameLogic");

        gameLogic = GL.GetComponent<GameLogic>();
        buildingList = GL.GetComponent<BuildingList>();
        assemblyList = GL.GetComponent<AssemblyList>();
        dragAndDrop = GL.GetComponent<DragAndDrop>();
        undoSystem = GL.GetComponent<UndoSystem>();
        storage = GL.GetComponent<Storage>();

        buildingInfo = GetComponent<BuildingInfo>();

        buildingsParent = GameObject.FindGameObjectWithTag("Hierarchy/Buildings").transform;
        tileMap = GameObject.FindGameObjectWithTag("Hierarchy/GameLogic").GetComponent<BuildingList>().tileMap;
    }

    public void RotateLeft() // rotating building to left
    {
        if (!rotating)
        {
            StartCoroutine(Rotate(new Vector3(0, 90, 0), 0.5f));//call corountine to rotate building smoothly 
        }
    }

    public void RotateRight() // rotating building to right
    {
        if (!rotating)
        {
           StartCoroutine(Rotate(new Vector3(0, -90, 0), 0.5f));//call corountine to rotate building smoothly 
        }
    }

    private IEnumerator Rotate(Vector3 angles, float duration)
    {
        GetComponent<BuildingsUI>().ChangeOnlyItemsRotation(new Vector3(0, 0, 0), duration); //call function to rotate building UI

        rotating = true;
        Quaternion startRotation = rotateObject.transform.rotation; //get current rotation of building
        Quaternion endRotation = Quaternion.Euler(angles) * startRotation; // set end roation of building

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            rotateObject.transform.rotation = Quaternion.Lerp(startRotation, endRotation, t / duration);//lerp between start end end rotation of building
            yield return null;
        }
        rotateObject.transform.rotation = endRotation;
        rotating = false;

        if (buildingInfo.typeOfBuilding == TypeOfBuilding.conveyor)
            GetComponent<Conveyor>().CheckSidesOnDestroyRotate();
    }

    public void Delete()//deleteting building
    {
        int j = int.Parse(name); //get name of building

        dragAndDrop.building = null;

        //set action (demolist ) to undo system and set building that was demolished
        undoSystem.action.Add("Demolish"); 
        DemolishList demolishList = new DemolishList();
        demolishList.buildings.Add(this.gameObject);
        undoSystem.demolishList.Add(demolishList);

        buildingList.BuildingsCount(j, 1);   

        //call in every building that this building was destroyed 
        for (int i = 0; i < buildingsParent.childCount; i++)
        {
            buildingsParent.GetChild(i).GetComponent<BuildingInfo>().TriggerExit();
        }

        tileMap.SetActive(false);//off tile map for placing buildings

        if (buildingInfo.typeOfBuilding == TypeOfBuilding.electricPole)
        {
            GetComponent<ElectricPole>().ClearCables();
            GetComponent<ElectricPole>().ClearPowerPlantCable();
        }

        this.gameObject.SetActive(false);
    }

    public void Options()//this function is called when building can have recipes
    {
        gameLogic.assemblyPanel.SetActive(true);
        tileMap.SetActive(false);
        gameLogic.pickedBuilding = this.gameObject;

        if (buildingInfo.typeOfBuilding == TypeOfBuilding.assembler)
            assemblyList.AssemblerList();
        else if (buildingInfo.typeOfBuilding == TypeOfBuilding.refinery)
            assemblyList.RefineryList();
        else if (buildingInfo.typeOfBuilding == TypeOfBuilding.solidifier)
            assemblyList.SolidifierList();
        else if (buildingInfo.typeOfBuilding == TypeOfBuilding.extruder)
            assemblyList.ExtruderList();
            
    }

    public void Storage()
    {
        if (buildingInfo.typeOfBuilding == TypeOfBuilding.assembler)
        {
            StorageList[] storageListAssembler = GetComponent<Assembler>().CreateAssemblerStorageList();
            storage.AssemblerStorage(storageListAssembler);
        }
        else if (buildingInfo.typeOfBuilding == TypeOfBuilding.refinery)
        {
            StorageList[] storageListRefinery = GetComponent<Refinery>().CreateRefineryStorageList();
            storage.RefineryStorage(storageListRefinery);
        }
        else if (buildingInfo.typeOfBuilding == TypeOfBuilding.solidifier)
        {
            //StorageList[] storageListSolidifier = GetComponent<Assembler>().CreateAssemblerStorageList();
           // storage.RefineryStorage(storageListSolidifier);
        }
        else if (buildingInfo.typeOfBuilding == TypeOfBuilding.powerPlant)
        {
           // StorageList[] storageListPowerPlant = GetComponent<Assembler>().CreateAssemblerStorageList();
           // storage.PowerPlantStorage(storageListPowerPlant);
        }
        else if (buildingInfo.typeOfBuilding == TypeOfBuilding.buyer)
        {
          //  StorageList[] storageListBuyer = GetComponent<Assembler>().CreateAssemblerStorageList();
          //  storage.BuyerStorage(storageListBuyer);
        }
    }

    public void Move() //this function is called when we want to move building
    {
        buildingInfo.WaitToSelect();
    }
}
