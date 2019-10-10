using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;

public class DragSystem : MonoBehaviour
{
    public Sprite slotImage;

    private List<RaycastResult> hitObjects = new List<RaycastResult>();

    [HideInInspector]
    public PickedSpell pickedSpell;

    private CollectionsButtons collectionsButtons;

    private void Start()
    {
        collectionsButtons = GetComponent<CollectionsButtons>();
    }

    private void Update()
    {
        MouseClicks();
    }

    private void MouseClicks()
    {
        if (pickedSpell.spell != null)
        {
            if (Input.GetMouseButton(0))
            {
                pickedSpell.spell.transform.position = Input.mousePosition;
            }

            if (Input.GetMouseButtonUp(0))
            {
                Destroy(pickedSpell.spell);

                    var objectToReplace = GetDraggableTransformUnderMouse();

                    if (objectToReplace != null)
                    {
                        for (int i = 0; i < 9; i++)
                        {
                            if (PlayerPrefs.GetString("SpellSlot" + i) == pickedSpell.spellListId + ";" + pickedSpell.spellId)
                            {
                                PlayerPrefs.SetString("SpellSlot" + i, -1 + ";" + -1);

                                collectionsButtons.spellSlots[i].GetComponent<Image>().sprite = slotImage;
                            }
                        }

                        objectToReplace.GetComponent<Image>().sprite = pickedSpell.image;

                        PlayerPrefs.SetString("SpellSlot" + objectToReplace.name, pickedSpell.spellListId + ";" + pickedSpell.spellId);
                    }              
            }
        }
    }

    private GameObject GetObjectUnderMouse()
    {
        var pointer = new PointerEventData(EventSystem.current);

        pointer.position = Input.mousePosition;

        EventSystem.current.RaycastAll(pointer, hitObjects);

        if (hitObjects.Count <= 0) return null;

        return hitObjects.First().gameObject;
    }

    private Transform GetDraggableTransformUnderMouse()
    {
        var clickedObject = GetObjectUnderMouse();

        // get top level object hit
        if (clickedObject != null && clickedObject.tag == "SpellSlot")
        {
            return clickedObject.transform;
        }

        return null;
    }

}
