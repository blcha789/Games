using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BuildingStatus { ok, colliding }
public enum TypeOfBuilding { solidifier, assembler, refinery, furnace, splitter, conveyor, pipe, pipeOut, pipeBridge, startBuilding, recycler, oreCrusher, bridge, miningDrill, fluidRig, extruder, buyer, electricPole, powerPlant}
public enum TypeOfSize { uneven, paired, various } // neparny, parny, rozny
public enum Rotation { Up = 0, Down = 180, Left = 270, Right = 90 }

public class ClassAndEnums : MonoBehaviour
{

}

[System.Serializable]
public class Deposits
{
    public string Name;
    public GameObject prefab;
    public Vector3 position;
    public int depositSize;
}

[System.Serializable]
public class BuildingSize
{
    public TypeOfSize typeOfSize;
    public int x;
    public int z;
}

[System.Serializable]
public class Building
{
    public string name;
    public string tag;
    public BuildingSize buildingSize;
    public Sprite image;
    public GameObject prefab;
    public int amount;
    public Text amountText;
    public string Description;
}

[System.Serializable]
public class ChooseBuilding
{
    public int id;
    public int amount;
}

[System.Serializable]
public class CurrentBuilding
{
    public int i;
    public string buildingType;
}

//input output classes
[System.Serializable]
public class ConveyorOutputSetup
{
    public Vector3 position = new Vector3(0, 1, 0);
    public Rotation rotation;
    public GameObject item;
    public Sprite image;
}

[System.Serializable]
public class PipeOutputSetup
{
    public Vector3 position;
    public Rotation rotation;
    public string fluidName;
    public Color fluidColor;
}

[System.Serializable]
public class BuyerSetup
{
    public Vector3 position = new Vector3(0, 1, 0);
    public Rotation rotation;

    public GameObject item;
    public string fluidName;

    public Sprite itemImage;
    public Color fluidColor;

    public Buyer buyer;

    public int itemAmount;
    public int fluidAmount;
}

[System.Serializable]
public class PowerPlantSetup
{
    public Vector3 position = new Vector3(0, 1, 0);
    public Rotation rotation;
}

//recipe classes
[System.Serializable]
public class AssemblerRecipes
{
    [Header("Main")]
    public string name;
    public float craftingTime;

    [Header("isItem")]
    public bool isItem2;

    [Header("Input")]
    public RecipeSetupItem[] inputItems;

    [Header("Output")]
     public RecipeSetupItem[] outputItem;
}

[System.Serializable]
public class RefineryRecipes
{
    [Header("Main")]
    public string name;
    public float craftingTime;
    public Sprite fluidImage;

    [Header("IsFluid")]
    public bool isFluid2Input;
    public bool isFluid3Input;
    public bool isFluid2Output;
    public bool isFluid3Output;

    [Header("Input")]
    public RecipeSetupFluid[] inputFluids;

    [Header("Output")]
    public RecipeSetupFluid[] outputFluids;
}

[System.Serializable]
public class SolidifierRecipe
{
    [Header("Main")]
    public string name;
    public float craftingTime;
    public Sprite fluidImage;

    [Header("IsFluid/Item")]
    public bool isFluid1;
    public bool isFluid2;
    public bool isItem1;
    public bool isItem2;

    [Header("Input")]
    public RecipeSetupFluid[] inputFluids;
    public RecipeSetupItem[] inputItems;

    [Header("Output")]
    public RecipeSetupItem[] outputItems;

}

[System.Serializable]
public class ExtruderRecipe
{
    [Header("Main")]
    public string name;
    public float craftingTime;

    [Header("Input")]
    public RecipeSetupItem[] inputItems;

    [Header("Output")]
    public RecipeSetupItem[] outputItem;
}

[System.Serializable]
public class RecipeSetupFluid
{
    public string name;
    public Color color;
    public float amount;
}

[System.Serializable]
public class RecipeSetupItem
{
    public string name;
    public Sprite image;
    public GameObject prefab;
    public int amount;
}

//undo system classes
[System.Serializable]
public class BuildList
{
    public int iBuilding;
    public Coroutine iBuildingCorountine;
    public GameObject[] buildings;
}

[System.Serializable]
public class DemolishList
{
    public List<GameObject> buildings = new List<GameObject>();
}

[System.Serializable]
public class StorageList
{
    public string name;
    public Sprite image;
    public float amount;
    public float amountNeed;
    public Color fluidColor;
}
