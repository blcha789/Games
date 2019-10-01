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

    [Header("Mana")]
    public Mana[] mana;
    
    private float currentHealthRegen;
    private float currentHealth;
    private float maxHealth;
    
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
        
	for(i = 0; i < mana.Length,i++)
	{
	mana[i].maxMana = startMana + (PlayerPrefs.GetInt("ManaModifier" + i) * manaModifier);
	mana[i].currentMana = mana[i].maxMana;
	mana[i].manaRegen = startManaRegen + (PlayerPrefs.GetInt("ManaRegenModifier" + i) * manaModifier);
	}	
        
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

	public void TakeMana(TypeOfMana mana, float manaAmount) 
	{
	 for(i = 0; i < mana.Length,i++)
	 {
	  if(mana[i].name == mana)
	  {
	   mana[i].current-= manaAmount;
	   //UI
	  }
	 }
	}
}

[System.Seriazable]
public class Mana
{
	public TypeOfMana name;
	public float maxMana;
	public float currentMana;
	public float manaRegen;
}
