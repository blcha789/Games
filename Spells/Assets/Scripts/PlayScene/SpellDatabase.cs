using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellDatabase : MonoBehaviour {

    public MenuSpells[] menuSpells;
}

[System.Serializable]
public class MenuSpells
{
    public string name;
    public Sprite menuSpellImage;
    public Spells[] spells;
}

[System.Serializable]
public class Spells
{
    [Header("Main")]
    public string name;
    public Sprite image;

    public SpellCategory spellCategory;
  
    public int damageHit;
    public float cooldown;
    public float mana;
    public GameObject prefab;

    public string Description;

    [Header("SpellType")]
    public SpellType spellType;
    public float spellTypeDamage;
    public float spellTypeBonusDamage;
    public float spellTypeDuration;

    [Header("SpellEffect")]
    public SpellEffect spellEffect;
    public float spellEffectModifier;
    public float spellEffectDuration;
}

public enum SpellCategory {Ball, Wall, Laser }


