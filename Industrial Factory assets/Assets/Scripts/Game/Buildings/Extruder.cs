using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Extruder : MonoBehaviour
{

    [Header("Main")]
    public float craftingTime;//Time to craft item
    public GameObject outputItemPrefab;//Crafted item
    public Transform spawnPos; //Position where crafted item will spawn

    public Transform press; //Object that will move 
    public float animationSpeed; //How fast will object move

    [Header("Inputs")]
    public CheckInputItem input1; //Script checking what item is on input

    [Header("Storage")]
    public int item1;//How many item is stored on input

    private int needItem1 = 1;//How many items is needed to start crafting process

    private float setCraftingTime;//Stored crafting time

    private GameLogic gameLogic;
    private Transform itemParent;
    private BuildingsUI buildingsUI;
    private AudioSource audioSource;
    private BuildingPower buildingPower;

    private bool up = false;

    private void Start()
    {
        //get scripts
        gameLogic = GameObject.FindGameObjectWithTag("Hierarchy/GameLogic").GetComponent<GameLogic>();
        itemParent = GameObject.FindGameObjectWithTag("Hierarchy/Items").transform;
        buildingsUI = GetComponent<BuildingsUI>();
        audioSource = GetComponent<AudioSource>();
        buildingPower = GetComponent<BuildingPower>();
    }

    //This function is called when is picked crafting recipe
    public void SetParameters(ExtruderRecipe extrudeRecipe)
    {
        craftingTime = extrudeRecipe.craftingTime;
        setCraftingTime = extrudeRecipe.craftingTime;


        input1.itemName = extrudeRecipe.inputItems[0].prefab.name;
        needItem1 = extrudeRecipe.inputItems[0].amount;


        outputItemPrefab = extrudeRecipe.outputItem[0].prefab;

        //Inputs Outputs Item Show
        for (int i = 0; i < buildingsUI.buildingsItems.Length; i++)
        {
            buildingsUI.buildingsItems[i].gameObject.SetActive(false);
        }

        //input1
        buildingsUI.buildingsItems[0].gameObject.SetActive(true);
        buildingsUI.buildingsItems[0].GetChild(0).GetChild(0).GetComponent<Image>().sprite = extrudeRecipe.inputItems[0].image;

        //output1
        buildingsUI.buildingsItems[1].gameObject.SetActive(true);
        buildingsUI.buildingsItems[1].GetChild(0).GetChild(0).GetComponent<Image>().sprite = extrudeRecipe.outputItem[0].image;

        SetDefaults();
    }

    //This function set recipe and building to default state
    public void SetDefaults()
    {
        item1 = 0;
        craftingTime = setCraftingTime;
        audioSource.Stop();
        buildingPower.SetDefaults();
    }

    private void Update()
    {
        if (gameLogic.isPlaying)//if is in play mode 
        {
            if (!gameLogic.isPowerInLevel || buildingPower.capacity >= 1)
            {
                if (item1 >= needItem1) //if is in storage enought items 1 for crafting
                {
                    if (!audioSource.isPlaying)//if sound of building is off then play sound
                        audioSource.Play();

                    Move();

                    if (craftingTime > 0)//if crafting time is greater than 0, decrease crafting time by time
                    {
                        craftingTime -= Time.deltaTime;
                    }
                    else// if is less than 0 spawn crafted item, decrease items in storage and set crafting time
                    {
                        Instantiate(outputItemPrefab, spawnPos.position, outputItemPrefab.transform.rotation, itemParent);
                        craftingTime = setCraftingTime;

                        item1 -= needItem1;
                        buildingPower.capacity -= 0.1f;
                    }
                }
            }
        }
    }

    //This function is moving Object(Press)
    private void Move()
    {
        if (up)
            press.position = new Vector3(press.position.x, press.position.y + animationSpeed * Time.deltaTime, press.position.z);
        else
            press.position = new Vector3(press.position.x, press.position.y - animationSpeed * Time.deltaTime, press.position.z);

        if (press.localPosition.y >= 0.6f)
            up = false;
        if (press.localPosition.y <= 0.3f)
            up = true;
    }
}

