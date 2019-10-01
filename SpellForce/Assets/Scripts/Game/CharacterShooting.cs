using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum typeOfSpell { projectile, wave, laser}

public class CharacterShooting : MonoBehaviour
{

    public float[] projectilePattern; 
    private List<SpellList> spellList =  new List<SpellList>();
    private CharacterStats characterStats;
 
 private void Start()
 {
  spellList = GameObject.FindObjectWithTag("GameManager").getComponent<SpellList>().spellList;
  characterStats = getComponent<CharacterStats>(); 
 }

 
 public void Shoot(int spellListId, int spellId)
 {
  if(spellList[spellListId].spell[spellId].typeOfspell == TypeOfSpell.Projectile)  
   	ShootProjectile(spellListId, spellId, spellList[spellListId].spell[spellId].spellLevel) 
  else if(spellList[spellListId].spell[spellId].typeOfspell == TypeOfSpell.Wave)
	ShootWave(spellListId, spellId, spellList[spellListId].spell[spellId].spellLevel) 
  else if(spellList[spellListId].spell[spellId].typeOfspell == TypeOfSpell.Laser)
	ShootLaser(spellListId, spellId, spellList[spellListId].spell[spellId].spellLevel) 
 }

 private void ShootProjectile(int spellListId, int spellId, int spellLevel)
 {
	int projectileAmount = spellLevel * 2 + 1;
  	for (int i = 0; i < projectileAmount; i++)
        {
          GameObject spell = Instantiate(spellList[spellListId].spell[spellId].spellPrefab, shotPos.position, Quaternion.identity);

          spell.GetComponent<SpellStats>().damage = spellList[spellListId].spell[spellId].damage;
          spell.GetComponent<Rigidbody>().AddForce((shotPos.transform.forward + shotPos.transform.right * projectilePattern[i]) * spellList[spellListId].spell[spellId].shootForce);
        }
	characterStats.TakeMana(spellList[spellListId].spell[spellId].mana, spellList[spellListId].spell[spellId].manaAmount);
 }

 private void ShootWave(int spellListId, int spellId, int spellLevel)
 {
  
  
  characterStats.TakeMana(spellList[spellListId].spell[spellId].mana, spellList[spellListId].spell[spellId].manaAmount);
 }

 private void ShootLaser(int spellListId, int spellId, int spellLevel)
 {
   
    //--mana per 1  by holdng button
 }
}
