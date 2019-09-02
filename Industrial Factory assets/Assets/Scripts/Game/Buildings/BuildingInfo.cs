using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class BuildingInfo : MonoBehaviour
{
    [Header("Building Info")]
    public bool isSelected = false; //if building is selected
    public bool startBuilding = false; //if is starting building 
    public BuildingStatus buildingStatus; //if building is ok or ic coliding with other building
    public TypeOfBuilding typeOfBuilding; // assembler, conveyor, refinery ...
    public BuildingSize buildingSize; //size of building (1x1, 2x2, 3x3 , 5x5)

    [Header("Building effect")]
    public float placeBuildingEffectTime;//duration of effect
    public Vector3 placeBuildingEffectSize;//size of effect
    public GameObject placeBuildingEffect;//effect when building is placed

    [Header("Building Objects")]
    public GameObject[] showObjects;//UI, Size, ...
    public Renderer[] objectRenderer; //renderer of model of building

    private List<Color>colors = new List<Color>();
    private GameLogic gameLogic;
    private Transform deposits;
    private Animator anim;

    private void Start()
    {
        gameLogic = GameObject.FindGameObjectWithTag("Hierarchy/GameLogic").GetComponent<GameLogic>();
        deposits = GameObject.FindGameObjectWithTag("Hierarchy/Deposits").transform;

        anim = GetComponent<Animator>();


        //set all building models to default color
        for (int j = 0; j < objectRenderer.Length; j++)
        {
            for (int i = 0; i < objectRenderer[j].materials.Length; i++)
            {
                colors.Add(objectRenderer[j].materials[i].color);
            }
        }

        if (typeOfBuilding != TypeOfBuilding.conveyor && typeOfBuilding != TypeOfBuilding.pipe)//if is type of building is assembler or another building except conveyor or pipe call building select
            BuildingSelect();
    }

    private void Update()
    {
        if (!startBuilding)
        {
            if (gameLogic.constructionOperation == ConstructionOperation.Demolish)//if is in demolishing
            {
                if (Input.GetMouseButton(0))
                {
                    Vector3 camPos = Camera.main.WorldToScreenPoint(transform.position);
                    camPos.y = Demolish.InvertMouseY(camPos.y);
                    isSelected = Demolish.selection.Contains(camPos);
                }

                if (isSelected)// if building is selected in demolish mode then will have green color
                    for (int j = 0; j < objectRenderer.Length; j++)
                    {
                        for (int i = 0; i < objectRenderer[j].materials.Length; i++)
                        {
                            objectRenderer[j].materials[i].color = Color.green;
                        }
                    }
                else//if is not any more selected then color is default
                {
                    int k = 0;
                    for (int j = 0; j < objectRenderer.Length; j++)
                    {
                        for (int i = 0; i < objectRenderer[j].materials.Length; i++)
                        {
                            objectRenderer[j].materials[i].color = colors[k];
                            k += 1;
                        }
                    }
                }
            }
        }
    }

    private void OnTriggerStay(Collider col)
    {
        if (buildingStatus == BuildingStatus.ok) //if is not coliding with any building
        {
            if (col.tag.Contains("Building"))//trigered building with tag Building
            {
                //change color to red when is coliding and set that building is coliding
                for (int j = 0; j < objectRenderer.Length; j++)
                {
                    for (int i = 0; i < objectRenderer[j].materials.Length; i++)
                    {
                        objectRenderer[j].materials[i].color = Color.red;
                    }
                }
                buildingStatus = BuildingStatus.colliding;
            }
        }
    }

    //when building is not any more coliding
    public void TriggerExit()
    {
        buildingStatus = BuildingStatus.ok; //set status tu ok

        int k = 0;
        //change color to default
        for (int j = 0; j < objectRenderer.Length; j++)
        {
            for (int i = 0; i < objectRenderer[j].materials.Length; i++)
            {
                objectRenderer[j].materials[i].color = colors[k];
                k += 1;
            }
        }

        //if building is not any more on fluid deposit
        if (deposits != null)
        {
            for (int i = 0; i < deposits.childCount; i++)
            {
                if (deposits.GetChild(i).tag == "Deposit/Fluid")//if deposit have tag Deposit/Fluid
                {
                    deposits.GetChild(i).GetComponent<FluidDeposit>().TriggerExit(); //call in deposit that building is not any more on it
                }
            }
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.tag.Contains("Building"))
        {
            TriggerExit();
        }
    }

    public void BuildingSelect()
    {
        if (!startBuilding)//if is not starting building
        {
            for (int i = 0; i < showObjects.Length; i++) //show all object when building is selectet (UI, Size,...)
            {
                showObjects[i].SetActive(true);
            }
            GetComponent<BuildingsUI>().ChangeCanvasItemsRotationToCamera(); // call function to rotate building UI To camera         
        }
    }

    public void BuildingUnselect()
    {
        isSelected = false;

        for (int i = 0; i < showObjects.Length; i++)//deactive all object that was showed when building was selected
        {
            showObjects[i].SetActive(false);
        }

        //If building animation is any of this states then set trigger Down
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("BuildingUp") || anim.GetAnimatorTransitionInfo(0).IsName("BuildingDown -> BuildingUp") || anim.GetCurrentAnimatorStateInfo(0).IsName("Main"))
        {
            anim.SetTrigger("Down");
            Invoke("BuildingUnselectEffect", placeBuildingEffectTime);
        }
    }

    //this function is called when building is placed on ground, it will crate smoke on when building touched ground 
    public void BuildingUnselectEffect()
    {
        GameObject smoke = Instantiate(placeBuildingEffect, transform.position + new Vector3(0, -0.5f, 0), Quaternion.identity);

        smoke.transform.localScale = placeBuildingEffectSize;

        Destroy(smoke, 1f);

        if (typeOfBuilding == TypeOfBuilding.conveyor)
        {
            GetComponent<Conveyor>().CheckSidesOnMove();
        }
        else if (typeOfBuilding == TypeOfBuilding.electricPole)
        {
            Invoke("FindPoles", 0.25f);
        }
    }

    private void FindPoles()
    {
        GetComponent<ElectricPole>().FindPoles();
    }

    public void WaitToSelect()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("BuildingDown"))
            anim.SetTrigger("Up");

        isSelected = true;

        if (typeOfBuilding == TypeOfBuilding.conveyor)
        {
            GetComponent<Conveyor>().CheckSidesOnMove();
        }
        else if(typeOfBuilding == TypeOfBuilding.electricPole)
        {
            GetComponent<ElectricPole>().ClearCables();
            GetComponent<ElectricPole>().ClearPowerPlantCable();
        }
    }
}