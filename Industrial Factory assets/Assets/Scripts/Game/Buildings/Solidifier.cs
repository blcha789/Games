using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Solidifier : MonoBehaviour
{

    [Header("Main")]
    public float craftingTime;//Time to craft item

    [Header("Inputs")]//Script checking what fluid is on inputs
    public CheckInputFluid inputFluid1;
    public CheckInputFluid inputFluid2;
    public CheckInputItem inputItem1;
    public CheckInputItem inputItem2;

    [Header("Outputs")]
    public GameObject outputItemPrefab;
    public Transform outputSpawnPos;

    [Header("Storage")]
    public float fluid1;
    public float fluid2;
    public float item1;
    public float item2;

    private float needFluid1 = 1;
    private float needFluid2 = 1;
    private float needItem1 = 1;
    private float needItem2 = 1;

    private bool isFluid1;
    private bool isFluid2;
    private bool isItem1;
    private bool isItem2;

    private float setCraftingTime;
    private GameLogic gameLogic;
    private Transform itemParent;
    private BuildingsUI buildingsUI;
    private AudioSource audioSource;

    private void Start()
    {
        gameLogic = GameObject.FindGameObjectWithTag("Hierarchy/GameLogic").GetComponent<GameLogic>();
        itemParent = GameObject.FindGameObjectWithTag("Hierarchy/Items").transform;
        buildingsUI = GetComponent<BuildingsUI>();
        audioSource = GetComponent<AudioSource>();
    }

    //This function is called when is picked crafting recipe
    public void SetParameters(SolidifierRecipe mixerRecipe)
    {
        craftingTime = mixerRecipe.craftingTime;
        setCraftingTime = mixerRecipe.craftingTime;

        isFluid1 = mixerRecipe.isFluid1;
        isFluid2 = mixerRecipe.isFluid2;
        isItem1 = mixerRecipe.isItem1;
        isItem2 = mixerRecipe.isItem2;
     
        if(isFluid1)
        {
            inputFluid1.fluidName = mixerRecipe.inputFluids[0].name;
            inputFluid1.fluidMax = mixerRecipe.inputFluids[0].amount;
            needFluid1 = mixerRecipe.inputFluids[0].amount;
        }
        if(isFluid2)
        {
            inputFluid2.fluidName = mixerRecipe.inputFluids[1].name;
            inputFluid2.fluidMax = mixerRecipe.inputFluids[1].amount;
            needFluid2 = mixerRecipe.inputFluids[1].amount;
        }
        if(isItem1)
        {
            inputItem1.itemName = mixerRecipe.inputItems[0].prefab.name;
            needItem1 = mixerRecipe.inputItems[0].amount;
        }
        if(isItem2)
        {
            inputItem2.itemName = mixerRecipe.inputItems[1].prefab.name;
            needItem2 = mixerRecipe.inputItems[1].amount;
        }

            outputItemPrefab = mixerRecipe.outputItems[0].prefab;
        
        for (int i = 0; i < buildingsUI.buildingsItems.Length; i++)
        {
            buildingsUI.buildingsItems[i].gameObject.SetActive(false);
        }

        if (isFluid1)
        {
            buildingsUI.buildingsItems[0].gameObject.SetActive(true);
            Color c = mixerRecipe.inputFluids[0].color;
            buildingsUI.buildingsItems[0].GetChild(0).GetChild(0).GetComponent<Image>().color = new Color(c.r, c.g, c.b, 255);
        }

        if (isFluid2)
        {
            buildingsUI.buildingsItems[1].gameObject.SetActive(true);
            Color c = mixerRecipe.inputFluids[1].color;
            buildingsUI.buildingsItems[1].GetChild(0).GetChild(0).GetComponent<Image>().color = new Color(c.r, c.g, c.b, 255);
        }

        if (isItem1)
        {
            buildingsUI.buildingsItems[2].gameObject.SetActive(true);
            buildingsUI.buildingsItems[2].GetChild(0).GetChild(0).GetComponent<Image>().sprite = mixerRecipe.inputItems[0].image;
        }

        if (isItem2)
        {
            buildingsUI.buildingsItems[3].gameObject.SetActive(true);
            buildingsUI.buildingsItems[3].GetChild(0).GetChild(0).GetComponent<Image>().sprite = mixerRecipe.inputItems[0].image;
        }

            buildingsUI.buildingsItems[4].gameObject.SetActive(true);
            buildingsUI.buildingsItems[4].GetChild(0).GetChild(0).GetComponent<Image>().sprite = mixerRecipe.outputItems[0].image;      
    }

    //This function set recipe and building to default state
    public void SetDefaults()
    {
        fluid1 = 0;
        fluid2 = 0;

        inputFluid1.fluidAmount = 0;
        inputFluid2.fluidAmount = 0;

        craftingTime = setCraftingTime;
        audioSource.Stop();
    }

    private void Update()
    {
        if (outputItemPrefab != null)
        {
            if (gameLogic.isPlaying)//if is in play mode 
            {
                if (fluid1 >= needFluid1 || !isFluid1) //if is enougth fluid to craft or if we dont need fluid to craft
                {
                    if (fluid2 >= needFluid2 || !isFluid2)//if is enougth fluid to craft or if we dont need fluid to craft
                    {
                        if (item1 >= needItem1 || !isItem1)//if is enougth items to craft or if we dont need item to craft
                        {
                            if (item2 >= needItem2 || !isItem2)//if is enougth items to craft or if we dont need item to craft
                            {
                                if (craftingTime > 0)
                                {
                                    if (!audioSource.isPlaying)
                                        audioSource.Play();

                                    craftingTime -= Time.deltaTime;
                                }
                                else
                                {
                                    Instantiate(outputItemPrefab, outputSpawnPos.position, outputItemPrefab.transform.rotation, itemParent);

                                    craftingTime = setCraftingTime;

                                    fluid1 -= needFluid1;
                                    fluid2 -= needFluid2;

                                    item1 -= needItem1;
                                    item2 -= needItem2;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
