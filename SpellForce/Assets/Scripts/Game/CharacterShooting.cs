using System.Collections;
using System.Collections.Generic;
using UnityEngine;

<<<<<<< HEAD
=======
public enum typeOfSpell { projectile, wave, laser}

>>>>>>> 20e02be97fb7f80c786405da7da5ce7f2122e581
public class CharacterShooting : MonoBehaviour
{
    public float[] projectilePattern;
    public Transform[] shotPosProjectile;

    private List<SpellListGame> spellLists = new List<SpellListGame>();
    private CharacterStats characterStats;

<<<<<<< HEAD
    [HideInInspector]
    public PickedSpell pickedSpell;

    private void Start()
    {
        spellLists = GameObject.FindGameObjectWithTag("GameManager").GetComponent<SpellSetup>().spellLists;
        characterStats = GetComponent<CharacterStats>();
    }

    private void Update()
    {
        if (pickedSpell.spell != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //cast ray from mouse position
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity)) //, mask.value   raycast for ingoring some layers
            {
                if (Input.GetMouseButton(0))
                {
                    pickedSpell.spell.transform.position = hit.point;
                }

                if (Input.GetMouseButtonUp(0))
                {
                    Destroy(pickedSpell.spell);

                    Instantiate(spellLists[pickedSpell.spellListId].spells[pickedSpell.spellId].prefab, hit.point, Quaternion.identity);
                    //--mana
                }
            }
        }
    }


    public void PressShooting(int spellListId, int spellId)
    {
        string type = spellLists[spellListId].spells[spellId].type;

        if (type == "Projectile")
            Projectile(spellListId, spellId, spellLists[spellListId].spells[spellId].level);
        else if (type == "Wave")
            Wave(spellListId, spellId, spellLists[spellListId].spells[spellId].level);
        else if (type == "Laser")
            Laser(spellListId, spellId, spellLists[spellListId].spells[spellId].level);
    }

    public void MoveShooting(int spellListId, int spellId)
    {
        pickedSpell.spell = Instantiate(spellLists[spellListId].spells[spellId].projection, Vector3.zero, Quaternion.identity);
        pickedSpell.spellListId = spellListId;
        pickedSpell.spellId = spellId;
    }

    public void HoldShooting(int spellListId, int spellId)
    {
        Debug.Log("Holdshoot: " + spellListId + ";" + spellId);
    }



    private void Projectile(int spellListId, int spellId, int spellLevel)
    {
        int projectileAmount = spellLevel * 2 + 1;
        for (int i = 0; i < projectileAmount; i++)
        {
            GameObject spell = Instantiate(spellLists[spellListId].spells[spellId].prefab, shotPosProjectile[i].position, Quaternion.identity);

            //spell.GetComponent<SpellStats>().damage = spellLists[spellListId].spells[spellId].damage;
            //spell.GetComponent<Rigidbody>().AddForce((shotPos.transform.forward + shotPos.transform.right * projectilePattern[i]) * spellLists[spellListId].spells[spellId].shootForce);
        }
        //characterStats.TakeMana(spellList[spellListId].spell[spellId].mana, spellList[spellListId].spell[spellId].manaAmount);
    }

    private void Wave(int spellListId, int spellId, int spellLevel)
    {

    }

    private void Laser(int spellListId, int spellId, int spellLevel)
    {

        //--mana per 1  by holdng button
    }
=======
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
   	ShootProjectile(spellListId, spellId, spellList[spellListId].spell[spellId].spellLevel);
  else if(spellList[spellListId].spell[spellId].typeOfspell == TypeOfSpell.Wave)
	ShootWave(spellListId, spellId, spellList[spellListId].spell[spellId].spellLevel); 
  else if(spellList[spellListId].spell[spellId].typeOfspell == TypeOfSpell.Laser)
	ShootLaser(spellListId, spellId, spellList[spellListId].spell[spellId].spellLevel); 
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
>>>>>>> 20e02be97fb7f80c786405da7da5ce7f2122e581
}
