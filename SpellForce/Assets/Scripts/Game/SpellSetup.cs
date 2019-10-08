using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SpellSetup : MonoBehaviour
{

    public TextAsset[] spellTxt;
    public Transform[] spellSlots;

    [HideInInspector]
    public SpellListGame spellList;

    [HideInInspector]
    public List<SpellListGame> spellLists = new List<SpellListGame>();

    private CharacterShooting characterShooting;

    void Start()
    {
        characterShooting = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterShooting>();
        LoadDatabase();
        SetupSpells();
    }

    private void LoadDatabase()
    {
        for (int i = 0; i < spellTxt.Length; i++)
        {
            string[] lines = spellTxt[i].text.Split('\n');

            for (int j = 1; j < lines.Length; j++)
            {
                string[] line = lines[j].Split('\t');
                SpellGame spell = new SpellGame();

                spell.name = line[1];
                spell.prefab = Resources.Load("Prefabs/Spells/" + line[1], typeof(GameObject)) as GameObject;
                spell.projection = Resources.Load("Prefabs/SpellProjections/SpellProjection" + line[3], typeof(GameObject)) as GameObject;
                spell.image = Resources.Load("Images/Spells/" + line[1], typeof(Sprite)) as Sprite;
                spell.castType = line[2];
                spell.type = line[3];
                spell.level = int.Parse(line[4]);
                spell.manaType = line[6];
                spell.manaAmount = int.Parse(line[7]);

                spellList.spells.Add(spell);
            }
            spellLists.Add(spellList);
        }
    }

    private void SetupSpells()
    {
        for (int i = 0; i < 9; i++)
        {
            string[] spellSlotId = PlayerPrefs.GetString("SpellSlot" + i).Split(';');

            if (spellSlotId[0] != "-1" && spellSlotId[0] != "")
            {
                int spellListId = int.Parse(spellSlotId[0]);
                int spellId = int.Parse(spellSlotId[1]);

                spellSlots[i].name = spellSlotId[0] + ";" + spellSlotId[1];
                spellSlots[i].GetChild(0).GetComponent<Image>().sprite = spellLists[spellListId].spells[spellId].image;

                if(spellLists[spellListId].spells[spellId].castType == "Press") //press
                    spellSlots[i].GetComponent<Button>().onClick.AddListener(() => characterShooting.PressShooting(spellListId, spellId));
                else if (spellLists[spellListId].spells[spellId].castType == "Move") //move where will spell do damage
                {
                    EventTrigger trigger = spellSlots[i].GetComponent<EventTrigger>();
                    EventTrigger.Entry entry = new EventTrigger.Entry();
                    entry.eventID = EventTriggerType.PointerDown;
                    entry.callback.AddListener((data) => characterShooting.MoveShooting(spellListId, spellId));
                    trigger.triggers.Add(entry);
                }
                else //hold
                {
                    EventTrigger trigger = spellSlots[i].GetComponent<EventTrigger>();
                    EventTrigger.Entry entry = new EventTrigger.Entry();
                    
                    //pointerdown
                    entry.eventID = EventTriggerType.PointerDown;
                    entry.callback.AddListener((data) => characterShooting.HoldShootingPointerDown(spellListId, spellId));
                    trigger.triggers.Add(entry);
                    
                    //pointerup
                    entry.eventID = EventTriggerType.PointerUp;
                    entry.callback.AddListener((data) => characterShooting.HoldShootingPointerUp(spellListId, spellId));
                    trigger.triggers.Add(entry);
                }
            }
            else
                spellSlots[i].name = "-1;-1";
        }
    }
}

[System.Serializable]
public class SpellListGame
{
    public List<SpellGame> spells;
}

[System.Serializable]
public class SpellGame
{
    public string name;
    public string castType;
    public string type;
    public int level;
    public GameObject prefab;
    public GameObject projection;
    public Sprite image;
    public string manaType;
    public float manaAmount;
}
