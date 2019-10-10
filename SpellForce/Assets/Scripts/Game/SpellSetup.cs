using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SpellSetup : MonoBehaviour
{

    public SpellList[] spellList;
    public Transform[] spellSlots;


    private CharacterShooting characterShooting;

    void Start()
    {
        characterShooting = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterShooting>();
        SetupSpells();
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
                spellSlots[i].GetChild(0).GetComponent<Image>().sprite = spellList[spellListId].spell[spellId].image;

                if (spellList[spellListId].spell[spellId].castType == CastType.Press) //press
                    spellSlots[i].GetComponent<Button>().onClick.AddListener(() => characterShooting.PressShooting(spellListId, spellId));
                else if (spellList[spellListId].spell[spellId].castType == CastType.Move) //move where will spell do damage
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
