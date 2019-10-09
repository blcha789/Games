using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "SceneSettings/Spawner", order = 1)]
public class SpellList : ScriptableObject
{
    public Spell[] spell;
}

//inspector/editor

[System.Serializable]
public class Spell
{
    public string name;
    public string castType;
    public string type;
    
    public Sprite image;
    public GameObject prefab;
    public GameObject projection;

    public int level;
    public string effect;
    public string manaType;
    public float manaAmount;
    public float dmg;
    public float dps;
    public int required;
    public int price;
    
    public string description;
}
