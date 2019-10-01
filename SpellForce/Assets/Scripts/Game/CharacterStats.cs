using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [Header("Start Stats")]
    public float starthealth = 100f;
    public float startMana = 100f;	
    public float startHealthRegen = 1f;
    public float startManaRegen = 1f;
	
    [Header("Modifiers")]
    public float healthModifier = 10;
    public float manaModifier = 10;
    public float healthRegenModifier = 0.2f;
    public float manaRegenModifier = 0.2f; 

    private float currentManaRegen;
    private float currentHealthRegen;

    private float currentHealth;
    private float currentFireMana;
    private float currentWaterMana;
    private float currentColdMana;
    private float currentAirMana;
    private float currentDarkMana;
    private float currentEarthMana;

    private float maxHealth;
    private float maxFireMana;
    private float maxWaterMana;
    private float maxColdMana;
    private float maxAirMana;
    private float maxDarkMana;
    private float maxEarthMana;  
    
    private bool isDead = false;

    private void Start()
    {
        SetStats();
    }

    private void SetStats()
    {
        maxHealth = startHealth + PlayerPrefs.GetInt("HealthModifier") * healthModifier;
	currentHealth = maxHealth;
        
        currentHealthRegen = startHealthRegen + PlayerPrefs.GetInt("HealthRegenModifier") * healthRegenModifier;
	currentManaRegen = startManaRegen + PlayerPrefs.GetInt("ManaRegenModifier") * manaRegenModifier;
     
        maxFireMana = startMana + (PlayerPrefs.GetInt("FireManaModifier") * manaModifier);
        maxWaterMana = startMana + (PlayerPrefs.GetInt("WaterManaModifier") * manaModifier);
        maxColdMana = startMana + (PlayerPrefs.GetInt("ColdManaModifier") * manaModifier);
        maxAirMana = startMana + (PlayerPrefs.GetInt("AirManaModifier") * manaModifier);
        maxDarkMana = startMana + (PlayerPrefs.GetInt("DarkManaModifier") * manaModifier);
        maxEarthMana = startMana + (PlayerPrefs.GetInt("EarthManaModifier") * manaModifier);
        
        currentFireMana = maxFireMana;
        currentWaterMana = maxWaterMana;
        currentColdMana = maxColdMana;
        currentAirMana = maxAirMana;
        currentDarkMana = maxDarkMana;
        currentEarthMana = maxEarthMana;
    }

    private void Update()
	{
	 Regeneration();
	}


	private void Regeneration()
	{
	  if(currentHealth < maxHealth)
	  {
	   currentHealth += currentHealthRegen * Time.deltaTime;
	   //UI
	  }

	  if(currentMana < maxMana)
	  {
	   currentMana += currentManaRegen * Time.delatTime;
 	   //UI
	  }	
	}

	public void TakeDamage(float damage)
	{
	  if(isDead)
	    return;

	  currentHealth -= damage;
	  //UI

	  if (currentHealth <= 0)
          {
	   //GameOver
	  }
	}

	public void TakeMana(float mana) 
	{
	 currentMana -= mana;
	 //UI
	}
}
