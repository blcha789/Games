using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{

    public float startMana = 100;
    public float manaModifier = 10;

    [Header("Mana")]
    public float fireMana;
    public float waterMana;
    public float coldMana;
    public float airMana;
    public float darkMana;
    public float earthMana;

    void Start()
    {
        SetMana();
    }

    private void SetMana()
    {
        fireMana = startMana + (PlayerPrefs.GetInt("FireManaModifier") * manaModifier);
        waterMana = startMana + (PlayerPrefs.GetInt("WaterManaModifier") * manaModifier);
        coldMana = startMana + (PlayerPrefs.GetInt("ColdManaModifier") * manaModifier);
        airMana = startMana + (PlayerPrefs.GetInt("AirManaModifier") * manaModifier);
        darkMana = startMana + (PlayerPrefs.GetInt("DarkManaModifier") * manaModifier);
        earthMana = startMana + (PlayerPrefs.GetInt("EarthManaModifier") * manaModifier);
    }

    void Update()
    {
        
    }
}
