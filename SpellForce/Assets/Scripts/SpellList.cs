using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Setting/Spells/SpellList", order = 1)]
public class SpellList : ScriptableObject
{
    public Spell[] spell;
}

public enum CastType {Press, Move, Hold }
public enum Type {Projectile, Wall, Laser, Wave }
public enum ManaType {Fire, Water, Air, Ice, Earth, Dark }
public enum Level { Level0, Level1, Level2}

[System.Serializable]
public class Spell
{
    public string name;
    public CastType castType;
    public Type type;
    public ManaType manaType;
    public Level level;

    public Sprite image;
    public GameObject prefab;
    public GameObject projection;

    public int required;
    public string effect;
    public float manaAmount;
    public float dmg;
    public float dps;
    public int price;
    public string description;
}
