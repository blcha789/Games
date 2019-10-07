using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum spellElement{fire, water, cold, air, dark, earth };

public class CollectionsButtons : MonoBehaviour
{
    public TextAsset[] spellTxt;
    public Transform[] spellsButtons;
    public Transform[] spellSlots;

    [Header("Prefabs")]
    public GameObject unlockedSpellPrefab;
    public GameObject draggableUISpell;

    [Header("Main Parent Panels")]
    public Transform collectionPanelParent;
    public Transform spellsPanelsParent;
    public Transform offensiveSpellsParent;
    public Transform defensiveSpellsParent;
    public Transform unlockedSpellsParent;
    public Transform canvas;

    [Header("Other Panels")]
    public Transform spellDescriptionPanel;
    public GameObject notEnoughtGemsPanel;

    private List<SpellListMenu> spellLists = new List<SpellListMenu>();
    private DragSystem dragSystem;

    [HideInInspector]
    public SpellList spellList;

    void Start()
    {
        dragSystem = GetComponent<DragSystem>();
        LoadDatabase();
    }

    private void LoadDatabase()
    {
        for (int i = 0; i < spellTxt.Length; i++)
        {
            string[] lines = spellTxt[i].text.Split('\n');

            for (int j = 1; j < lines.Length; j++)
            {
                string[] line = lines[j].Split('\t');
                SpellMenu spell = new SpellMenu();

                spell.name = line[1];
                spell.image = Resources.Load("Images/Spells/" + line[1], typeof(Sprite)) as Sprite;
                 /*spell.type = line[2];
                 spell.effect = line[3];
                 spell.manaType = line[4];
                 /*  spell.mana = int.Parse(line[5]);
                   spell.dmg = float.Parse(line[6]);
                   spell.dps = float.Parse(line[7]);
                   spell.required = int.Parse(line[8]);
                   spell.price = int.Parse(line[9]);
                   spell.description = line[10];*/

                spellList.spells.Add(spell);
            }
            spellLists.Add(spellList);
        }
    }

    private void LoadUnlockedSpells()
    {
        foreach (Transform item in unlockedSpellsParent)
        {
            Destroy(item.gameObject);
        }

        for (int i = 0; i < spellLists.Count; i++)
        {
            for (int j = 0; j < spellLists[i].spells.Count; j++)
            {
                if (PlayerPrefs.GetInt("SpellList" + i + "/" + j) == 0)//ma byt 1
                {
                    GameObject s = Instantiate(unlockedSpellPrefab, unlockedSpellsParent);

                    s.name = i + ";" + j;
                    s.GetComponent<Image>().sprite = spellLists[i].spells[j].image;

                    EventTrigger trigger = s.GetComponent<EventTrigger>();
                    EventTrigger.Entry entry = new EventTrigger.Entry();
                    entry.eventID = EventTriggerType.PointerDown;
                    entry.callback.AddListener((data) => CreateDraggableSpell());
                    trigger.triggers.Add(entry);
                }            
            }
        }

        for (int k = 0; k < 9; k++)
        {
            string[] id = PlayerPrefs.GetString("SpellSlot" + k).Split(';');

            if (id[0] != "-1" && id[0] != "")//empty
            {
                spellSlots[k].GetComponent<Image>().sprite = spellLists[int.Parse(id[0])].spells[int.Parse(id[1])].image;
            }
        }
    }

    private void OpenSpellList(int spellListId)
    {
        //animate UI

        //načítať spely
        for (int i = 0; i < spellLists[spellListId].spells.Count; i++)
        {
            if (PlayerPrefs.GetInt("SpellList" + spellListId + "/" + i) == 1)
                spellsButtons[spellListId].GetChild(i).GetChild(0).gameObject.SetActive(false); // vypne lock image
            else
                spellsButtons[spellListId].GetChild(i).GetChild(0).gameObject.SetActive(true); // zapne lock image
        }
    }

    private void ShowOffensiveSpellDescription(int spellListId, int spellId)
    {
        spellDescriptionPanel.gameObject.SetActive(true);

        spellDescriptionPanel.GetChild(0).GetComponent<Text>().text = spellLists[spellListId].spells[spellId].name;
        spellDescriptionPanel.GetChild(1).GetComponent<Image>().sprite = spellLists[spellListId].spells[spellId].image;
        spellDescriptionPanel.GetChild(2).GetComponent<Text>().text = spellLists[spellListId].spells[spellId].type;
        spellDescriptionPanel.GetChild(3).GetComponent<Text>().text = spellLists[spellListId].spells[spellId].effect;
        spellDescriptionPanel.GetChild(4).GetComponent<Text>().text = spellLists[spellListId].spells[spellId].manaType;
        spellDescriptionPanel.GetChild(5).GetComponent<Text>().text = spellLists[spellListId].spells[spellId].mana.ToString();
        spellDescriptionPanel.GetChild(6).GetComponent<Text>().text = spellLists[spellListId].spells[spellId].dmg.ToString();
        spellDescriptionPanel.GetChild(7).GetComponent<Text>().text = spellLists[spellListId].spells[spellId].dps.ToString();
        spellDescriptionPanel.GetChild(9).GetComponent<Text>().text = spellLists[spellListId].spells[spellId].required.ToString();
        spellDescriptionPanel.GetChild(10).GetComponentInChildren<Text>().text = spellLists[spellListId].spells[spellId].price.ToString(); //button
        spellDescriptionPanel.GetChild(11).GetComponent<Text>().text = spellLists[spellListId].spells[spellId].description;

        if (PlayerPrefs.GetInt("SpellList" + spellListId + "/" + spellId) == 0)
            spellDescriptionPanel.GetChild(10).gameObject.SetActive(true); //show buy button
        else
            spellDescriptionPanel.GetChild(10).GetComponent<Button>().onClick.AddListener(() => BuySpell(spellListId, spellId));
    }

    public void BuySpell(int spellListId, int spellId)
    {
        int gems = PlayerPrefs.GetInt("CrystalGems");
        if (gems >= spellLists[spellListId].spells[spellId].price)
        {
            spellDescriptionPanel.gameObject.SetActive(false);
            spellsButtons[spellListId].GetChild(spellId).GetChild(0).gameObject.SetActive(false); // vypne lock image
            PlayerPrefs.SetInt("SpellList" + spellListId + "/" + spellId, 1);

            gems -= spellLists[spellListId].spells[spellId].price;
            PlayerPrefs.SetInt("CrystalGems", gems);
        }
        else
        {
            notEnoughtGemsPanel.SetActive(true);
        }
    }

    private void DisablePanels()
    {
        foreach (Transform item in spellsPanelsParent)
        {
            item.gameObject.SetActive(false);
        }

        foreach (Transform item in offensiveSpellsParent)
        {
            item.gameObject.SetActive(false);
        }
    }


    //Buttons
    public void MainButtons(int id)
    {
        foreach (Transform item in collectionPanelParent)
        {
            item.gameObject.SetActive(false);
        }
        collectionPanelParent.GetChild(id).gameObject.SetActive(true);
    }

    public void SpellsMainButtons(int id)
    {
        DisablePanels();
        spellsPanelsParent.GetChild(id).gameObject.SetActive(true);

        if (id == 2)
            LoadUnlockedSpells();
    }


    public void OpenOffensiveSpellsButton(int spellListId)
    {
        DisablePanels();
        offensiveSpellsParent.GetChild(spellListId).gameObject.SetActive(true);
        OpenSpellList(spellListId);
    }

    public void OpenDefensiveSpellsButton(int spellListId)
    {
        DisablePanels();
    }


    public void ShowOffSpellDescriptionButton(string id)
    {
        string[] i = id.Split(';');
        ShowOffensiveSpellDescription(int.Parse(i[0]), int.Parse(i[1]));
    }

    public void ShowDeffSpellDescriptionButton()
    {

    }


    public void CreateDraggableSpell()
    {
        Vector3 pos = EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>().position;
        string name = EventSystem.current.currentSelectedGameObject.transform.name;
        string[] id = name.Split(';');

        dragSystem.pickedSpell.spell = Instantiate(draggableUISpell, pos, Quaternion.identity, canvas);
        dragSystem.pickedSpell.spell.GetComponent<Image>().sprite = spellLists[int.Parse(id[0])].spells[int.Parse(id[1])].image;

        dragSystem.pickedSpell.isPicked = true;
        dragSystem.pickedSpell.image = spellLists[int.Parse(id[0])].spells[int.Parse(id[1])].image;
        dragSystem.pickedSpell.spellListId = int.Parse(id[0]);
        dragSystem.pickedSpell.spellId = int.Parse(id[1]);
    }

}


[System.Serializable]
public class SpellListMenu
{
    public List<SpellMenu> spells;
}

[System.Serializable]
public class SpellMenu
{
    public string name;
    public Sprite image;
    public string type;
    public string effect;
    public string manaType;
    public float mana;
    public float dmg;
    public float dps;
    public int required;
    public int price;
    public string description;
}

[System.Serializable]
public class PickedSpell
{
    public bool isPicked;
    public GameObject spell;
    public Sprite image;
    public int spellListId;
    public int spellId;
}
