using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum StatusMode { Play, Build, Waiting};
public enum BuildMode { Placing, Removing, None}
public enum WaveState { Spawning, Waiting, Complete}
public enum TypeOfFire { Normal, Shotgun}

public class EnumAndClass : MonoBehaviour
{
}

[System.Serializable]
public class BuildingList
{
    public string name;

    public Sprite image;
    public GameObject prefab;
    public int amount;
    public int cost;
    public float size;
    public Text amountTextBuildList;
    public Text amountTextShopList;
    public GameObject buildingListObject;
}

[System.Serializable]
public class AmmoList
{
    public string name;
    public int pickAmmoAmount;
    public float fireRate;
    public float fireForce;
    public float damage;
    public Color color;
    public TypeOfFire typeOfFire;
}
