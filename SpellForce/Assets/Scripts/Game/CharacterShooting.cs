using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterShooting : MonoBehaviour
{
    public SpellList[] spellList;

    public float[] projectilePattern;
    public Transform[] shotPosProjectile;

    private CharacterStats characterStats;

    [HideInInspector]
    public PickedSpell pickedSpell;

    private void Start()
    {
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

                    Instantiate(spellList[pickedSpell.spellListId].spell[pickedSpell.spellId].prefab, hit.point, Quaternion.identity);
                    characterStats.TakeMana(spellList[pickedSpell.spellListId].spell[pickedSpell.spellId].manaType, spellList[pickedSpell.spellListId].spell[pickedSpell.spellId].manaAmount);
                }
            }
        }
    }


    public void PressShooting(int spellListId, int spellId)
    {
        Type type = spellList[spellListId].spell[spellId].type;

        if (type == Type.Projectile)
            Projectile(spellListId, spellId, (int)spellList[spellListId].spell[spellId].level);
        else if (type == Type.Wave)
            Wave(spellListId, spellId, (int)spellList[spellListId].spell[spellId].level);
    }

    public void MoveShooting(int spellListId, int spellId)
    {
        pickedSpell.spell = Instantiate(spellList[spellListId].spell[spellId].projection, Vector3.zero, Quaternion.identity);
        pickedSpell.spellListId = spellListId;
        pickedSpell.spellId = spellId;
    }

    public void HoldShootingPointerDown(int spellListId, int spellId)
    {
        IEnumerator coroutine = Laser(spellListId, spellId, 0);
        StartCoroutine(coroutine);
	   //onParticleSystem
    }
    
    public void HoldShootingPointerUp(int spellListId, int spellId)
    {
        IEnumerator coroutine = Laser(spellListId, spellId, 0);
        StopCoroutine(coroutine);
		//offParticleSystem
    }



    private void Projectile(int spellListId, int spellId, int spellLevel)
    {
        int projectileAmount = (int)Mathf.Ceil(spellLevel * 3.5f + 1);
        for (int i = 0; i < projectileAmount; i++)
        {
            GameObject spell = Instantiate(spellList[spellListId].spell[spellId].prefab, shotPosProjectile[i].position, Quaternion.identity);

            //spell.GetComponent<SpellStats>().damage = spellLists[spellListId].spells[spellId].damage;
            //spell.GetComponent<Rigidbody>().AddForce((shotPos.transform.forward + shotPos.transform.right * projectilePattern[i]) * spellLists[spellListId].spells[spellId].shootForce);
        }
        characterStats.TakeMana(spellList[spellListId].spell[spellId].manaType, spellList[spellListId].spell[spellId].manaAmount);
    }

    private void Wave(int spellListId, int spellId, int spellLevel)
    {
	
    }

    IEnumerator Laser(int spellListId, int spellId, int spellLevel)
    {
            yield return new WaitForSeconds(1.0f);
	        characterStats.TakeMana(spellList[spellListId].spell[spellId].manaType, spellList[spellListId].spell[spellId].manaAmount);
	    
	    //find object in collider then damage them
    }
}
