using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SpellButton : MonoBehaviour, IPointerClickHandler
{
    public bool isSpellButton = false;

    public int menuCount;
    public int count;

    public Transform[] spellParent;

    public GameObject infoPanel;
    SpellDatabase spellDatabase;

    void Start()
    {
        spellDatabase = GameObject.Find("Database").GetComponent<SpellDatabase>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isSpellButton)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                PlayerPrefs.SetInt("Mouse0MenuCount", menuCount);
                PlayerPrefs.SetInt("Mouse0Spell", count);
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                PlayerPrefs.SetInt("Mouse1MenuCount", menuCount);
                PlayerPrefs.SetInt("Mouse1Spell", count);
            }
        }
    }

    public void Click()
    {
        for (int i = 0; i < spellParent.Length; i++)
        {
            if (i == menuCount)
                spellParent[i].gameObject.SetActive(true);
            else
                spellParent[i].gameObject.SetActive(false);
        }
    }

    public void Hover()
    {
        if (isSpellButton)
        {
            infoPanel.SetActive(true);
            infoPanel.transform.GetChild(0).GetComponent<Text>().text = spellDatabase.menuSpells[menuCount].spells[count].name;
            infoPanel.transform.GetChild(1).GetComponent<Image>().sprite = spellDatabase.menuSpells[menuCount].spells[count].image;

            //mainSpellInfo
            if(spellDatabase.menuSpells[menuCount].spells[count].spellCategory == SpellCategory.Ball)
            {
                infoPanel.transform.GetChild(2).GetChild(0).GetChild(1).GetComponent<Text>().text = spellDatabase.menuSpells[menuCount].spells[count].damageHit.ToString();
                infoPanel.transform.GetChild(2).GetChild(1).GetChild(1).GetComponent<Text>().text = spellDatabase.menuSpells[menuCount].spells[count].cooldown.ToString();
                infoPanel.transform.GetChild(2).GetChild(2).GetChild(1).GetComponent<Text>().text = spellDatabase.menuSpells[menuCount].spells[count].mana.ToString();
            }
            else if (spellDatabase.menuSpells[menuCount].spells[count].spellCategory == SpellCategory.Wall)
            {
                infoPanel.transform.GetChild(2).GetChild(0).gameObject.SetActive(false);
                infoPanel.transform.GetChild(2).GetChild(1).GetChild(1).GetComponent<Text>().text = spellDatabase.menuSpells[menuCount].spells[count].cooldown.ToString();
                infoPanel.transform.GetChild(2).GetChild(2).GetChild(1).GetComponent<Text>().text = spellDatabase.menuSpells[menuCount].spells[count].mana.ToString();
            }
            else
            {
                infoPanel.transform.GetChild(2).GetChild(0).GetChild(1).GetComponent<Text>().text = spellDatabase.menuSpells[menuCount].spells[count].damageHit.ToString();
                infoPanel.transform.GetChild(2).GetChild(1).GetChild(1).GetComponent<Text>().text = spellDatabase.menuSpells[menuCount].spells[count].cooldown.ToString()+ "s";
                infoPanel.transform.GetChild(2).GetChild(2).GetChild(1).GetComponent<Text>().text = spellDatabase.menuSpells[menuCount].spells[count].mana.ToString();
            }

            //spellTypeInfo
            if(spellDatabase.menuSpells[menuCount].spells[count].spellType == SpellType.None)
            {
                infoPanel.transform.GetChild(3).GetChild(0).GetChild(1).GetComponent<Text>().text = "None";
                infoPanel.transform.GetChild(3).GetChild(1).gameObject.SetActive(false);
                infoPanel.transform.GetChild(3).GetChild(2).gameObject.SetActive(false);
                infoPanel.transform.GetChild(3).GetChild(3).gameObject.SetActive(false);
            }
            else
            {
                infoPanel.transform.GetChild(3).GetChild(0).GetChild(1).GetComponent<Text>().text = spellDatabase.menuSpells[menuCount].spells[count].spellType.ToString();
                infoPanel.transform.GetChild(3).GetChild(1).GetChild(1).GetComponent<Text>().text = spellDatabase.menuSpells[menuCount].spells[count].spellTypeDamage.ToString();
                infoPanel.transform.GetChild(3).GetChild(2).GetChild(1).GetComponent<Text>().text = spellDatabase.menuSpells[menuCount].spells[count].spellTypeBonusDamage.ToString();
                infoPanel.transform.GetChild(3).GetChild(3).GetChild(1).GetComponent<Text>().text = spellDatabase.menuSpells[menuCount].spells[count].spellTypeDuration.ToString()+ "s";
            }

            //spellEffectInfo
            if (spellDatabase.menuSpells[menuCount].spells[count].spellEffect == SpellEffect.None)
            {
                infoPanel.transform.GetChild(4).GetChild(0).GetChild(1).GetComponent<Text>().text = "None";
                infoPanel.transform.GetChild(4).GetChild(1).gameObject.SetActive(false);
                infoPanel.transform.GetChild(4).GetChild(2).gameObject.SetActive(false);
            }
            else
            {
                infoPanel.transform.GetChild(4).GetChild(0).GetChild(1).GetComponent<Text>().text = spellDatabase.menuSpells[menuCount].spells[count].spellEffect.ToString();
                infoPanel.transform.GetChild(4).GetChild(1).GetChild(1).GetComponent<Text>().text = spellDatabase.menuSpells[menuCount].spells[count].spellEffectModifier.ToString();
                infoPanel.transform.GetChild(4).GetChild(2).GetChild(1).GetComponent<Text>().text = spellDatabase.menuSpells[menuCount].spells[count].spellEffectDuration.ToString() + "s";
            }

            //description
            infoPanel.transform.GetChild(5).GetComponent<Text>().text = spellDatabase.menuSpells[menuCount].spells[count].Description;
        }
    }

    public void HoverExit()
    {
        if (isSpellButton)
        {
            infoPanel.SetActive(false);
            infoPanel.transform.GetChild(0).GetComponent<Text>().text = "";
            infoPanel.transform.GetChild(1).GetComponent<Image>().sprite = spellDatabase.menuSpells[menuCount].spells[count].image;

            for (int i = 2; i < 5; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    infoPanel.transform.GetChild(i).GetChild(j).gameObject.SetActive(true);
                }
            }
        }
    }
}
