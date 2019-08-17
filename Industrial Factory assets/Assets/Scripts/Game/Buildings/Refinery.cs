using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Refinery : MonoBehaviour
{

    [Header("Main")]
    public float craftingTime;//Time to craft fluid

    [Header("Inputs")]//Scripts checking what item is on inputs
    public CheckInputFluid inputFluid1;
    public CheckInputFluid inputFluid2;
    public CheckInputFluid inputFluid3;

    [Header("Outputs")]//scripts that containes output fluid
    public OutputFluid outputFluid1;
    public OutputFluid outputFluid2;
    public OutputFluid outputFluid3;

    [Header("Storage")]//How many fluids are in storage
    public float fluid1;
    public float fluid2;
    public float fluid3;

    //how many fluids we need to start crafting
    private float needFluid1 = 1;
    private float needFluid2 = 1;
    private float needFluid3 = 1;

    //how many output fluids are in storage
    private float outputFluid1Amount;
    private float outputFluid2Amount;
    private float outputFluid3Amount;

    //which fluid we need to craft
    private bool isFluid2;
    private bool isFluid3;

    private float setCraftingTime;

    private GameLogic gameLogic;
    private BuildingsUI buildingsUI;
    private AudioSource audioSource;

    private void Start()
    {
        gameLogic = GameObject.FindGameObjectWithTag("Hierarchy/GameLogic").GetComponent<GameLogic>();
        buildingsUI = GetComponent<BuildingsUI>();
        audioSource = GetComponent<AudioSource>();
    }

    //This function is called when is picked crafting recipe
    public void SetParameters(RefineryRecipes refineryRecipe)
    {
        craftingTime = refineryRecipe.craftingTime;
        setCraftingTime = refineryRecipe.craftingTime;

        isFluid2 = refineryRecipe.isFluid2Input;
        isFluid3 = refineryRecipe.isFluid3Input;

        inputFluid1.fluidName = refineryRecipe.inputFluids[0].name;
        inputFluid1.fluidMax = refineryRecipe.inputFluids[0].amount;
        needFluid1 = refineryRecipe.inputFluids[0].amount;

        if (isFluid2)
        {
            inputFluid2.fluidName = refineryRecipe.inputFluids[1].name;
            inputFluid2.fluidMax = refineryRecipe.inputFluids[1].amount;
            needFluid2 = refineryRecipe.inputFluids[1].amount;
        }
        if (isFluid3)
        {
            inputFluid3.fluidName = refineryRecipe.inputFluids[2].name;
            inputFluid3.fluidMax = refineryRecipe.inputFluids[2].amount;
            needFluid3 = refineryRecipe.inputFluids[2].amount;
        }

        outputFluid1.fluidName = refineryRecipe.outputFluids[0].name;
        outputFluid1.fluidColor = refineryRecipe.outputFluids[0].color;
        outputFluid1Amount = refineryRecipe.outputFluids[0].amount;

        if (refineryRecipe.isFluid2Output)
        {
            outputFluid2.fluidName = refineryRecipe.outputFluids[1].name;
            outputFluid2.fluidColor = refineryRecipe.outputFluids[1].color;
            outputFluid2Amount = refineryRecipe.outputFluids[1].amount;
        }
        if (refineryRecipe.isFluid3Output)
        {
            outputFluid3.fluidName = refineryRecipe.outputFluids[2].name;
            outputFluid3.fluidColor = refineryRecipe.outputFluids[2].color;
            outputFluid3Amount = refineryRecipe.outputFluids[2].amount;
        }

        //Inputs Outputs Item Show
        for (int i = 0; i < buildingsUI.buildingsItems.Length; i++)
        {
            buildingsUI.buildingsItems[i].gameObject.SetActive(false);
        }

        //input1
        buildingsUI.buildingsItems[0].gameObject.SetActive(true);
        Color c = refineryRecipe.inputFluids[0].color;
        buildingsUI.buildingsItems[0].GetChild(0).GetChild(0).GetComponent<Image>().color = new Color(c.r, c.g, c.b, 255);
        //input2
        if (isFluid2)
        {
            buildingsUI.buildingsItems[1].gameObject.SetActive(true);
            c =  refineryRecipe.inputFluids[1].color;
            buildingsUI.buildingsItems[1].GetChild(0).GetChild(0).GetComponent<Image>().color = new Color(c.r, c.g, c.b, 255);
        }
        //input3
        if (isFluid3)
        {
            buildingsUI.buildingsItems[2].gameObject.SetActive(true);
            c = refineryRecipe.inputFluids[2].color;
            buildingsUI.buildingsItems[2].GetChild(0).GetChild(0).GetComponent<Image>().color = new Color(c.r, c.g, c.b, 255);
        }

        //output1
        buildingsUI.buildingsItems[3].gameObject.SetActive(true);
        c = refineryRecipe.outputFluids[0].color;
        buildingsUI.buildingsItems[3].GetChild(0).GetChild(0).GetComponent<Image>().color = new Color(c.r, c.g, c.b, 255);
        //output2
        if (refineryRecipe.isFluid2Output)
        {
            buildingsUI.buildingsItems[4].gameObject.SetActive(true);
            c = refineryRecipe.outputFluids[1].color;
            buildingsUI.buildingsItems[4].GetChild(0).GetChild(0).GetComponent<Image>().color = new Color(c.r, c.g, c.b, 255);
        }
        //ouput3
        if (refineryRecipe.isFluid3Output)
        {
            buildingsUI.buildingsItems[5].gameObject.SetActive(true);
            c = refineryRecipe.outputFluids[2].color;
            buildingsUI.buildingsItems[5].GetChild(0).GetChild(0).GetComponent<Image>().color = new Color(c.r, c.g, c.b, 255);
        }
    }

    //This function set recipe and building to default state
    public void SetDefaults()
    {
        fluid1 = 0;
        fluid2 = 0;
        fluid3 = 0;

        outputFluid1.fluidAmount = 0;
        outputFluid2.fluidAmount = 0;
        outputFluid3.fluidAmount = 0;

        inputFluid1.fluidAmount = 0;
        inputFluid2.fluidAmount = 0;
        inputFluid3.fluidAmount = 0;

        craftingTime = setCraftingTime;
        audioSource.Stop();
    }

    private void Update()
    {
        if (gameLogic.isPlaying) //if is in play mode
        {
            if (fluid1 >= needFluid1) //if is stored enougth fluid to craft
            {
                if (fluid2 >= needFluid2 || !isFluid2) //if is stored enougth fluid to craft or if we dont need fluid to craft
                {
                    if (fluid3 >= needFluid3 || !isFluid3)//if is stored enougth fluid to craft or if we dont need fluid to craft
                    {
                        if (craftingTime > 0)//if crafting time is greater than 0, decrease crafting time by time
                        {
                            if (!audioSource.isPlaying)//if sound of building is off then play sound
                                audioSource.Play();

                            craftingTime -= Time.deltaTime;
                        }
                        else //if is less than 0 spawn crafted fluid, decrease fluid in storage and set crafting time
                        {
                            audioSource.Stop();
                            outputFluid1.fluidAmount += outputFluid1Amount;
                            outputFluid2.fluidAmount += outputFluid2Amount;
                            outputFluid3.fluidAmount += outputFluid3Amount;

                            craftingTime = setCraftingTime;

                            fluid1 -= needFluid1;
                            fluid2 -= needFluid2;
                            fluid3 -= needFluid3;
                        }
                    }
                }
            }
        }
    }
}
