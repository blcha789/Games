using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellsUI : MonoBehaviour
{
    public GameObject spellPrefab;
    public GameObject menuSpellPrefab;

    public Transform mainParent;
    public GameObject infoPanel;
    public Transform[] spellParent;

    SpellDatabase spellDatabase;

    void Start()
    {

        spellDatabase = GameObject.Find("Database").GetComponent<SpellDatabase>();

        Vector3 mainCenter = new Vector3(0, 0, 0);
        for (int i = 0; i < spellDatabase.menuSpells.Length; i++)
        {
            Vector3 mainPos = FirstCircle(mainCenter, 150f, i);

            GameObject menuSpell = Instantiate(menuSpellPrefab, mainPos, Quaternion.identity);
            menuSpell.transform.SetParent(mainParent.transform, false);

            menuSpell.name = spellDatabase.menuSpells[i].name;
            menuSpell.transform.GetChild(0).GetComponent<Image>().sprite = spellDatabase.menuSpells[i].menuSpellImage;

            menuSpell.GetComponent<SpellButton>().menuCount = i;
            menuSpell.GetComponent<SpellButton>().spellParent = spellParent;


            Vector3 center = menuSpell.transform.localPosition;
            for (int j = 0; j < spellDatabase.menuSpells[i].spells.Length; j++)
            {
                Vector3 pos = SecondCircle(mainCenter, 320f, i, j);

                GameObject spell = Instantiate(spellPrefab, pos, Quaternion.identity);
                spell.transform.SetParent(spellParent[i].transform, false);

                spell.name = spellDatabase.menuSpells[i].spells[j].name;
                spell.transform.GetChild(0).GetComponent<Image>().sprite = spellDatabase.menuSpells[i].spells[j].image;

                spell.GetComponent<SpellButton>().infoPanel = infoPanel;
                spell.GetComponent<SpellButton>().count = j;
                spell.GetComponent<SpellButton>().menuCount = i;
            }
        }

        for (int i = 0; i < spellParent.Length; i++)
        {
            spellParent[i].gameObject.SetActive(false);
        }
    }

    Vector3 FirstCircle(Vector3 center, float radius, int i)
    {
        float ang = (360 / spellDatabase.menuSpells.Length) * i;
        Vector3 pos;
        pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.y = center.y + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        pos.z = 0;
        return pos;
    }

    Vector3 SecondCircle(Vector3 center, float radius, int i, int j)
    {
        float ang = (20 * j) + ((360 / spellDatabase.menuSpells.Length) * i); // ... + odkial zacina
        Vector3 pos;
        pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.y = center.y + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        pos.z = 0;
        return pos;
    }

}
