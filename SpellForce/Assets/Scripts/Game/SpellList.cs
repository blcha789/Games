using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "SceneSettings/Spawner", order = 1)]
public class SpellList : ScriptableObject
{
    public Spell[] spell;
}

//editor
[CustomEditor (typeof(SpellList))]
public class SpellListEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SpellList mp = (SpellList)target;
        
        
    }
}

enum CastType = {Press, Move, Hold};
enum Type = {Projectile, Wave, Laser};
enum ManaType = {Fire, Water, Ice, Air, Earth, Dark}

[System.Serializable]
public class Spell
{
    public string name;
    public CastType castType;
    public Type type;
    public ManaType manaType;
    
    public Sprite image;
    public GameObject prefab;
    public GameObject projection;

    public int level;
    public string effect;
    public float manaAmount;
    public float dmg;
    public float dps;
    public int required;
    public int price;
    
    public string description;
}
