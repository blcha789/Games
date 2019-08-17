using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.IO;

public class AssemblyList : MonoBehaviour {


    //text file with recipes 
    public TextAsset assemblerRecipeFile;
    public TextAsset refineryRecipeFile;
    public TextAsset solidifierRecipeFile;
    public TextAsset extruderRecipeFile;

    //what recipes do you want to show when picking recipe
    public int[] chooseAssemblerRecipes;//if empty == all recipes
    public int[] chooseRefineryRecipes;
    public int[] chooseSolidifierRecipes;
    public int[] chooseExtruderRecipes;

    [Header("AssemblyList")]
    public GameObject assemblyListPrefab;
    public GameObject assemblyRecipePrefab;
    public GameObject plusImagePrefab;
    public Transform contentParent;

    //Recipes lists
    private List<AssemblerRecipes> assemblerRecipes = new List<AssemblerRecipes>();
    private List<RefineryRecipes> refineryRecipes = new List<RefineryRecipes>();
    private List<SolidifierRecipe> solidifierRecipes = new List<SolidifierRecipe>();
    private List<ExtruderRecipe> extruderRecipes = new List<ExtruderRecipe>();

    private int countInputsFluids, countInputsItems, countOutputsFluids;

    private void Start()
    {

        //on start load all recipes
        LoadAssemblerDatabase();
        LoadRefineryDatabase();
        LoadSolidifierDatabase();
        LoadExtruderDatabase();
    }

    private void LoadAssemblerDatabase()
    {
        string[] lines = assemblerRecipeFile.text.Split('\n'); // split text to lines

        for (int i = 1; i < lines.Length; i++)
        {
            string[] line = lines[i].Split('\t');//split line

            int length;
            if (chooseAssemblerRecipes.Length == 0)//if we dont choose any recipe
                length = 1;
            else
                length = chooseAssemblerRecipes.Length;

            for (int j = 0; j < length; j++)
            {
                if (chooseAssemblerRecipes.Length == 0 || (int.Parse(line[0]) == chooseAssemblerRecipes[j] && chooseAssemblerRecipes.Length > 0)) // if choosen recipe length is 0 
                {
                    AssemblerRecipes assemblerRecipeOnLoad = new AssemblerRecipes(); //create assembler recipe
                    assemblerRecipeOnLoad.name = line[1]; //add name
                    assemblerRecipeOnLoad.craftingTime = int.Parse(line[2]); //ad crafting time

                    countInputsItems = 1; //how many inputs it have

                    if (line[6] != "None") //if line[6]( name of item in input 2) is not None then is item 2 
                    {
                        assemblerRecipeOnLoad.isItem2 = true;
                        countInputsItems++;
                    }

                    assemblerRecipeOnLoad.inputItems = new RecipeSetupItem[countInputsItems];//set amount of inputs 
                    for (int k = 0; k < countInputsItems; k++)
                    {
                        RecipeSetupItem recipeSetupItemOnLoad = new RecipeSetupItem();//create recipe setup

                        recipeSetupItemOnLoad.name = line[k * 2 + 3].Replace("_", " "); //add name of item on input
                        recipeSetupItemOnLoad.image = Resources.Load("Images/Items/" + line[k * 2 + 3], typeof(Sprite)) as Sprite; //load image
                        recipeSetupItemOnLoad.prefab = Resources.Load<GameObject>("Prefabs/Items/" + line[k * 2 + 3]); //load prefab
                        recipeSetupItemOnLoad.amount = int.Parse(line[k * 2 + 4]); //add amount of items need to craft

                        assemblerRecipeOnLoad.inputItems[k] = recipeSetupItemOnLoad;
                    }

                    assemblerRecipeOnLoad.outputItem = new RecipeSetupItem[1];
                    for (int k = 0; k < 1; k++)
                    {
                        RecipeSetupItem recipeSetupItemOnLoad = new RecipeSetupItem();

                        recipeSetupItemOnLoad.name = line[7].Replace("_", " ");//add name of item on output
                        recipeSetupItemOnLoad.image = Resources.Load("Images/Items/" + line[7], typeof(Sprite)) as Sprite; // load image
                        recipeSetupItemOnLoad.prefab = Resources.Load<GameObject>("Prefabs/Items/" + line[7]); // load prefab
                        recipeSetupItemOnLoad.amount = 1;//set amount that will spawn

                        assemblerRecipeOnLoad.outputItem[k] = recipeSetupItemOnLoad;
                    }

                    assemblerRecipes.Add(assemblerRecipeOnLoad);
                    break;
                }
            }
        }
    }

    private void LoadRefineryDatabase()
    {
        string[] lines = refineryRecipeFile.text.Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {
            string[] line = lines[i].Split('\t');

            int length;
            if (chooseRefineryRecipes.Length == 0)
                length = 1;
            else
                length = chooseRefineryRecipes.Length;

            for (int j = 0; j < length; j++)
            {
                if (chooseRefineryRecipes.Length == 0 || (int.Parse(line[0]) == chooseRefineryRecipes[j] && chooseRefineryRecipes.Length > 0))
                {
                    RefineryRecipes refineryRecipeOnLoad = new RefineryRecipes();

                    refineryRecipeOnLoad.name = line[1]; //add name of fluid
                    refineryRecipeOnLoad.craftingTime = int.Parse(line[2]); // add crafting time
                    refineryRecipeOnLoad.fluidImage = Resources.Load("Images/Items/" + line[3], typeof(Sprite)) as Sprite; //load fluid image

                    countInputsFluids = 1;
                    countOutputsFluids = 1;

                    if (line[7] != "None")
                    {
                        refineryRecipeOnLoad.isFluid2Input = true;
                        countInputsFluids++;
                    }
                    if (line[10] != "None")
                    {
                        refineryRecipeOnLoad.isFluid3Input = true;
                        countInputsFluids++;
                    }
                    if (line[16] != "None")
                    {
                        refineryRecipeOnLoad.isFluid2Output = true;
                        countOutputsFluids++;
                    }
                    if (line[19] != "None")
                    {
                        refineryRecipeOnLoad.isFluid3Output = true;
                        countOutputsFluids++;
                    }

                    refineryRecipeOnLoad.inputFluids = new RecipeSetupFluid[countInputsFluids];
                    for (int k = 0; k < countInputsFluids; k++)
                    {
                        RecipeSetupFluid recipeSetup = new RecipeSetupFluid();
                        string[] colors = line[k * 3 + 5].Split(','); // split color parameters

                        recipeSetup.name = line[k * 3 + 4]; //set fluid name on input
                        recipeSetup.color = new Color(float.Parse(colors[0]) / 255f, float.Parse(colors[1]) / 255f, float.Parse(colors[2]) / 255f); //set color of fluid on input
                        recipeSetup.amount = int.Parse(line[k * 3 + 6]);

                        refineryRecipeOnLoad.inputFluids[k] = recipeSetup;
                    }

                    refineryRecipeOnLoad.outputFluids = new RecipeSetupFluid[countOutputsFluids];
                    for (int k = 0; k < countOutputsFluids; k++)
                    {
                        RecipeSetupFluid recipeSetup = new RecipeSetupFluid();
                        string[] colors = line[k * 3 + 14].Split(',');// split color parameters

                        recipeSetup.name = line[k * 3 + 13];//set fluid name on output
                        recipeSetup.color = new Color(float.Parse(colors[0]) / 255f, float.Parse(colors[1]) / 255f, float.Parse(colors[2]) / 255f);//set color of fluid on input
                        recipeSetup.amount = int.Parse(line[k * 3 + 15]);

                        refineryRecipeOnLoad.outputFluids[k] = recipeSetup; 
                    }

                    refineryRecipes.Add(refineryRecipeOnLoad);
                    break;
                }
            }
        }
    }

    private void LoadSolidifierDatabase()
    {
        string[] lines = solidifierRecipeFile.text.Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {
            string[] line = lines[i].Split('\t');

            int length;
            if (chooseSolidifierRecipes.Length == 0)
                length = 1;
            else
                length = chooseSolidifierRecipes.Length;

            for (int j = 0; j < length; j++)
            {
                if (chooseSolidifierRecipes.Length == 0 || (int.Parse(line[0]) == chooseSolidifierRecipes[j] && chooseSolidifierRecipes.Length > 0))
                {
                    SolidifierRecipe solidifierRecipeOnLoad = new SolidifierRecipe();

                    solidifierRecipeOnLoad.name = line[1];
                    solidifierRecipeOnLoad.craftingTime = int.Parse(line[2]);
                    solidifierRecipeOnLoad.fluidImage = Resources.Load("Images/Items/" + line[3], typeof(Sprite)) as Sprite;

                    countInputsItems = 0;
                    countInputsFluids = 0;
                    countOutputsFluids = 0;

                    if (line[4] != "None")
                    {
                        solidifierRecipeOnLoad.isItem1 = true;
                        countInputsItems++;
                    }
                    if (line[6] != "None")
                    {
                        solidifierRecipeOnLoad.isItem2 = true;
                        countInputsItems++;
                    }
                    if (line[8] != "None")
                    {
                        solidifierRecipeOnLoad.isFluid1 = true;
                        countInputsFluids++;
                    }
                    if (line[11] != "None")
                    {
                        solidifierRecipeOnLoad.isFluid2 = true;
                        countInputsFluids++;
                    }

                    solidifierRecipeOnLoad.inputItems = new RecipeSetupItem[countInputsItems];
                    for (int k = 0; k < countInputsItems; k++)
                    {
                        RecipeSetupItem recipeSetupItemOnLoad = new RecipeSetupItem();

                        recipeSetupItemOnLoad.name = line[k * 2 + 4].Replace("_", " ");
                        recipeSetupItemOnLoad.image = Resources.Load("Images/Items/" + line[k * 2 + 4], typeof(Sprite)) as Sprite;
                        recipeSetupItemOnLoad.prefab = Resources.Load<GameObject>("Prefabs/Items/" + line[k * 2 + 4]);
                        recipeSetupItemOnLoad.amount = int.Parse(line[k * 2 + 5]);

                        solidifierRecipeOnLoad.inputItems[k] = recipeSetupItemOnLoad;
                    }

                    solidifierRecipeOnLoad.inputFluids = new RecipeSetupFluid[countInputsFluids];
                    for (int k = 0; k < countInputsFluids; k++)
                    {
                        RecipeSetupFluid recipeSetupFluidOnLoad = new RecipeSetupFluid();
                        string[] colors = line[k * 3 + 9].Split(',');

                        recipeSetupFluidOnLoad.name = line[k * 3 + 8];
                        recipeSetupFluidOnLoad.color = new Color(float.Parse(colors[0]) / 255f, float.Parse(colors[1]) / 255f, float.Parse(colors[2]) / 255f);
                        recipeSetupFluidOnLoad.amount = int.Parse(line[k * 3 + 10]);

                        solidifierRecipeOnLoad.inputFluids[k] = recipeSetupFluidOnLoad;
                    }

                    solidifierRecipeOnLoad.outputItems = new RecipeSetupItem[1];
                    if (line[14] != "None")
                    {
                        RecipeSetupItem recipeSetupOutputOnLoad = new RecipeSetupItem();

                        recipeSetupOutputOnLoad.name = line[14].Replace("_", " ");
                        recipeSetupOutputOnLoad.image = Resources.Load("Images/Items/" + line[14], typeof(Sprite)) as Sprite;
                        recipeSetupOutputOnLoad.prefab = Resources.Load<GameObject>("Prefabs/Items/" + line[14]);
                        recipeSetupOutputOnLoad.amount = 1;

                        solidifierRecipeOnLoad.outputItems[0] = recipeSetupOutputOnLoad;
                    }

                    solidifierRecipes.Add(solidifierRecipeOnLoad);
                    break;
                }
            }
        }
    }

    private void LoadExtruderDatabase()
    {
        string[] lines = extruderRecipeFile.text.Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {
            string[] line = lines[i].Split('\t');

            int length;
            if (chooseExtruderRecipes.Length == 0)
                length = 1;
            else
                length = chooseExtruderRecipes.Length;

            for (int j = 0; j < length; j++)
            {
                if (chooseExtruderRecipes.Length == 0 || (int.Parse(line[0]) == chooseExtruderRecipes[j] && chooseExtruderRecipes.Length > 0))
                {
                    ExtruderRecipe extruderRecipeOnLoad = new ExtruderRecipe();
                    extruderRecipeOnLoad.name = line[1];
                    extruderRecipeOnLoad.craftingTime = int.Parse(line[2]);

                    countInputsItems = 1;

                    extruderRecipeOnLoad.inputItems = new RecipeSetupItem[countInputsItems];//nastavime pocet inputov
                    for (int k = 0; k < countInputsItems; k++)
                    {
                        RecipeSetupItem recipeSetupItemOnLoad = new RecipeSetupItem();

                        recipeSetupItemOnLoad.name = line[3].Replace("_", " ");
                        recipeSetupItemOnLoad.image = Resources.Load("Images/Items/" + line[3], typeof(Sprite)) as Sprite;
                        recipeSetupItemOnLoad.prefab = Resources.Load<GameObject>("Prefabs/Items/" + line[3]);
                        recipeSetupItemOnLoad.amount = int.Parse(line[4]);

                        extruderRecipeOnLoad.inputItems[k] = recipeSetupItemOnLoad;
                    }

                    extruderRecipeOnLoad.outputItem = new RecipeSetupItem[1];
                    for (int k = 0; k < 1; k++)
                    {
                        RecipeSetupItem recipeSetupItemOnLoad = new RecipeSetupItem();

                        recipeSetupItemOnLoad.name = line[5].Replace("_", " ");
                        recipeSetupItemOnLoad.image = Resources.Load("Images/Items/" + line[5], typeof(Sprite)) as Sprite;
                        recipeSetupItemOnLoad.prefab = Resources.Load<GameObject>("Prefabs/Items/" + line[5]);
                        recipeSetupItemOnLoad.amount = 1;

                        extruderRecipeOnLoad.outputItem[k] = recipeSetupItemOnLoad;
                    }

                    extruderRecipes.Add(extruderRecipeOnLoad);
                    break;
                }
            }
        }
    }


    private void ClearAssemblyList()
    {
       //destroy all loaded recipes in UI
        foreach (Transform item in contentParent)
        {
            Destroy(item.gameObject);
        }
    }


    //this functions are called to load recipes to UI

    public void AssemblerList()
    {
        ClearAssemblyList(); //call clear UI

        for (int i = 0; i < assemblerRecipes.Count; i++)
        {
            GameObject a = Instantiate(assemblyListPrefab, contentParent); //create Ui object that contains Crafting time, image of outputs and inputs ,...
            a.GetComponent<Button>().onClick.AddListener(() => PickAssemblyTask(TypeOfBuilding.assembler)); //add click listener to UI Object that conatins Button component

            a.name = i.ToString();//change name to id of recipe
            a.transform.GetChild(0).GetComponent<Text>().text = assemblerRecipes[i].craftingTime.ToString("F1") + "s";//set crafting time to text


            //set images to outputs 
            GameObject imageOut = Instantiate(assemblyRecipePrefab, a.transform.GetChild(1));
            imageOut.transform.GetChild(0).GetComponent<Image>().sprite = assemblerRecipes[i].outputItem[0].image;
            imageOut.transform.GetChild(0).GetComponentInChildren<Text>().text = assemblerRecipes[i].outputItem[0].amount.ToString();
            imageOut.transform.GetChild(1).GetComponent<Text>().text = assemblerRecipes[i].outputItem[0].name;


            //set images on inputs and place between Plus Image
            for (int j = 0; j < assemblerRecipes[i].inputItems.Length; j++)//inputs
            {
                if (j > 0)
                    Instantiate(plusImagePrefab, a.transform.GetChild(2));

                GameObject imageIn = Instantiate(assemblyRecipePrefab, a.transform.GetChild(2));
                imageIn.transform.GetChild(0).GetComponent<Image>().sprite = assemblerRecipes[i].inputItems[j].image;
                imageIn.transform.GetChild(0).GetComponentInChildren<Text>().text = assemblerRecipes[i].inputItems[j].amount.ToString();
                imageIn.transform.GetChild(1).GetComponent<Text>().text = assemblerRecipes[i].inputItems[j].name;
            }
        }
    }

    public void RefineryList()
    {
        ClearAssemblyList();

        for (int i = 0; i < refineryRecipes.Count; i++)
        {
            GameObject a = Instantiate(assemblyListPrefab, contentParent);
            a.GetComponent<Button>().onClick.AddListener(() => PickAssemblyTask(TypeOfBuilding.refinery));

            a.name = i.ToString();
            a.transform.GetChild(0).GetComponent<Text>().text = refineryRecipes[i].craftingTime.ToString("F1") + "s";//crafting time

            for (int j = 0; j < refineryRecipes[i].outputFluids.Length; j++)//outputs
            {
                GameObject imageOut = Instantiate(assemblyRecipePrefab, a.transform.GetChild(1));
                imageOut.transform.GetChild(0).GetComponent<Image>().sprite = refineryRecipes[i].fluidImage;
                imageOut.transform.GetChild(0).GetComponentInChildren<Text>().text = refineryRecipes[i].outputFluids[j].amount.ToString();
                Color color = refineryRecipes[i].outputFluids[j].color;
                imageOut.transform.GetChild(0).GetComponent<Image>().color = new Color(color.r, color.g, color.b, 255);
                imageOut.transform.GetChild(1).GetComponent<Text>().text = refineryRecipes[i].outputFluids[j].name;
            }

            for (int j = 0; j < refineryRecipes[i].inputFluids.Length; j++)//inputs
            {
                if (j > 0)
                    Instantiate(plusImagePrefab, a.transform.GetChild(2));

                GameObject imageIn = Instantiate(assemblyRecipePrefab, a.transform.GetChild(2));
                imageIn.transform.GetChild(0).GetComponent<Image>().sprite = refineryRecipes[i].fluidImage;
                imageIn.transform.GetChild(0).GetComponentInChildren<Text>().text = refineryRecipes[i].inputFluids[j].amount.ToString();
                Color color = refineryRecipes[i].inputFluids[j].color;
                imageIn.transform.GetChild(0).GetComponent<Image>().color = new Color(color.r, color.g, color.b, 255);
                imageIn.transform.GetChild(1).GetComponent<Text>().text = refineryRecipes[i].inputFluids[j].name;
            }
        }
    }

    public void SolidifierList()
    {
        ClearAssemblyList();

        for (int i = 0; i < solidifierRecipes.Count; i++)
        {
            GameObject a = Instantiate(assemblyListPrefab, contentParent);
            a.GetComponent<Button>().onClick.AddListener(() => PickAssemblyTask(TypeOfBuilding.solidifier));

            a.name = i.ToString();
            a.transform.GetChild(0).GetComponent<Text>().text = solidifierRecipes[i].craftingTime.ToString("F1") + "s";//crafting time

            for (int j = 0; j < solidifierRecipes[i].inputFluids.Length; j++)//inputFluid
            {
                if (j > 0)
                    Instantiate(plusImagePrefab, a.transform.GetChild(2));

                GameObject imageOut = Instantiate(assemblyRecipePrefab, a.transform.GetChild(2));
                imageOut.transform.GetChild(0).GetComponent<Image>().sprite = solidifierRecipes[i].fluidImage;
                imageOut.transform.GetChild(0).GetComponentInChildren<Text>().text = solidifierRecipes[i].inputFluids[j].amount.ToString();

                Color color = solidifierRecipes[i].inputFluids[j].color;
                imageOut.transform.GetChild(0).GetComponent<Image>().color = new Color(color.r, color.g, color.b , 255);
                imageOut.transform.GetChild(1).GetComponent<Text>().text = solidifierRecipes[i].inputFluids[j].name;
            }

            for (int j = 0; j < solidifierRecipes[i].inputItems.Length; j++)//inputItem
            {
                if (j > 0 || solidifierRecipes[i].isFluid1)
                    Instantiate(plusImagePrefab, a.transform.GetChild(2));

                GameObject imageIn = Instantiate(assemblyRecipePrefab, a.transform.GetChild(2));
                imageIn.transform.GetChild(0).GetComponent<Image>().sprite = solidifierRecipes[i].inputItems[j].image;
                imageIn.transform.GetChild(0).GetComponentInChildren<Text>().text = solidifierRecipes[i].inputItems[j].amount.ToString();
                imageIn.transform.GetChild(1).GetComponent<Text>().text = solidifierRecipes[i].inputItems[j].name;
            }

            for (int j = 0; j < solidifierRecipes[i].outputItems.Length; j++)//outputItem
            {
                GameObject imageOut = Instantiate(assemblyRecipePrefab, a.transform.GetChild(1));
                imageOut.transform.GetChild(0).GetComponent<Image>().sprite = solidifierRecipes[i].outputItems[j].image;
                imageOut.transform.GetChild(0).GetComponentInChildren<Text>().text = solidifierRecipes[i].outputItems[j].amount.ToString();
                imageOut.transform.GetChild(1).GetComponent<Text>().text = solidifierRecipes[i].outputItems[j].name;
            }
        }
    }

    public void ExtruderList()
    {
        ClearAssemblyList();

        for (int i = 0; i < extruderRecipes.Count; i++)
        {
            GameObject a = Instantiate(assemblyListPrefab, contentParent);
            a.GetComponent<Button>().onClick.AddListener(() => PickAssemblyTask(TypeOfBuilding.extruder));

            a.name = i.ToString();
            a.transform.GetChild(0).GetComponent<Text>().text = extruderRecipes[i].craftingTime.ToString("F1") + "s";//crafting time

            GameObject imageOut = Instantiate(assemblyRecipePrefab, a.transform.GetChild(1));//output
            imageOut.transform.GetChild(0).GetComponent<Image>().sprite = extruderRecipes[i].outputItem[0].image;
            imageOut.transform.GetChild(0).GetComponentInChildren<Text>().text = extruderRecipes[i].outputItem[0].amount.ToString();
            imageOut.transform.GetChild(1).GetComponent<Text>().text = extruderRecipes[i].outputItem[0].name;

            GameObject imageIn = Instantiate(assemblyRecipePrefab, a.transform.GetChild(2));
            imageIn.transform.GetChild(0).GetComponent<Image>().sprite = extruderRecipes[i].inputItems[0].image;
            imageIn.transform.GetChild(0).GetComponentInChildren<Text>().text = extruderRecipes[i].inputItems[0].amount.ToString();
            imageIn.transform.GetChild(1).GetComponent<Text>().text = extruderRecipes[i].inputItems[0].name;

        }
    }


    //this function is called when we pick recipe 
    //it will check for what building it is and set parameters of recipes to building
    private void PickAssemblyTask(TypeOfBuilding typeOfBuilding)
    {
        GameLogic gameLogic = GetComponent<GameLogic>();

        gameLogic.assemblyPanel.SetActive(false);

        int i = int.Parse(EventSystem.current.currentSelectedGameObject.name);

        if (typeOfBuilding == TypeOfBuilding.assembler)
        {
            gameLogic.pickedBuilding.GetComponent<Assembler>().SetParameters(assemblerRecipes[i]);
        }
        else if(typeOfBuilding == TypeOfBuilding.refinery)
        {
            gameLogic.pickedBuilding.GetComponent<Refinery>().SetParameters(refineryRecipes[i]);
        }
        else if(typeOfBuilding == TypeOfBuilding.solidifier)
        {
            gameLogic.pickedBuilding.GetComponent<Solidifier>().SetParameters(solidifierRecipes[i]);
        }
        else if(typeOfBuilding == TypeOfBuilding.extruder)
        {
            gameLogic.pickedBuilding.GetComponent<Extruder>().SetParameters(extruderRecipes[i]);
        }
    }
}
