using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Assembler : MonoBehaviour
{

    [Header("Main")]
    public float craftingTime; //Time to craft item
    public GameObject outputItemPrefab; //Crafted item
    public Transform spawnPos; //Position where crafted item will spawn

    [Header("Inputs")]
    public CheckInputItem input1; //Script checking what item is on input 1
    public CheckInputItem input2; //Script checking what item is on input 2

    [Header("Storage")]
    public int item1; //How many item is stored on input 1
    public int item2; //How many item is stored on input 2

    private int needItem1 = 1; //How many items is needed to start crafting process
    private int needItem2 = 1;

    private bool isItem2 =  false; //parameter if we need item 2 to craft

    private float setCraftingTime; //Stored crafting time

    GameLogic gameLogic;
    Transform itemParent;
    BuildingsUI buildingsUI;
    AudioSource audioSource;

    private void Start()
    {
        //get scripts
        gameLogic = GameObject.FindGameObjectWithTag("Hierarchy/GameLogic").GetComponent<GameLogic>();
        itemParent = GameObject.FindGameObjectWithTag("Hierarchy/Items").transform;
        buildingsUI = GetComponent<BuildingsUI>();
        audioSource = GetComponent<AudioSource>();
    }

    //This function is called when is picked crafting recipe
    public void SetParameters(AssemblerRecipes assemblerRecipe)
    {
        craftingTime = assemblerRecipe.craftingTime;
        setCraftingTime = assemblerRecipe.craftingTime;

        isItem2 = assemblerRecipe.isItem2;

        input1.itemName = assemblerRecipe.inputItems[0].prefab.name;
        needItem1 = assemblerRecipe.inputItems[0].amount;

        if (isItem2)
        {
            input2.itemName = assemblerRecipe.inputItems[1].prefab.name;
            needItem2 = assemblerRecipe.inputItems[1].amount;
        }

        outputItemPrefab = assemblerRecipe.outputItem[0].prefab;

        //Inputs Outputs Item Show
        for (int i = 0; i < buildingsUI.buildingsItems.Length; i++)
        {
            buildingsUI.buildingsItems[i].gameObject.SetActive(false);
        }

        //input1
        buildingsUI.buildingsItems[0].gameObject.SetActive(true);
        buildingsUI.buildingsItems[0].GetChild(0).GetChild(0).GetComponent<Image>().sprite = assemblerRecipe.inputItems[0].image;
        //input2
        if (isItem2)
        {
            buildingsUI.buildingsItems[1].gameObject.SetActive(true);
            buildingsUI.buildingsItems[1].GetChild(0).GetChild(0).GetComponent<Image>().sprite = assemblerRecipe.inputItems[1].image;
        }
        //output1
        buildingsUI.buildingsItems[2].gameObject.SetActive(true);
        buildingsUI.buildingsItems[2].GetChild(0).GetChild(0).GetComponent<Image>().sprite = assemblerRecipe.outputItem[0].image;

        SetDefaults();
    }

    //This function set recipe and building to default state
    public void SetDefaults()
    {
        item1 = 0;
        item2 = 0;
        craftingTime = setCraftingTime;
        audioSource.Stop();
    }

    private void Update()
    {
        if (outputItemPrefab != null) //if crafted item is not empty
        {
            if (gameLogic.isPlaying) //if is in play mode 
            {
                if (item1 >= needItem1) //if is in storage enought items 1 for crafting
                {
                    if (item2 >= needItem2 || !isItem2) //if is in storage enought items 2 or we dont need item 2
                    {
                        if (!audioSource.isPlaying) //if sound of building is off then play sound
                            audioSource.Play(); 

                        if (craftingTime > 0) //if crafting time is greater than 0, decrease crafting time by time
                        {
                            craftingTime -= Time.deltaTime;
                        }
                        else // if is less than 0 spawn crafted item, decrease items in storage and set crafting time
                        {
                            Instantiate(outputItemPrefab, spawnPos.position, outputItemPrefab.transform.rotation, itemParent);
                            craftingTime = setCraftingTime; 

                            item1 -= needItem1;
                            item2 -= needItem2;
                        }
                    }
                }
            }
        }
    }
}
