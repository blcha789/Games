using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorInput : MonoBehaviour {

    public GameObject item; 
    public string fluidName;

    public int itemCount;

    private void OnTriggerEnter(Collider col)
    {
        if (col.tag.Contains("Item"))  
        {
            if (col.tag.Contains(item.tag))
            {
                itemCount += 1;
                Destroy(col.gameObject);
            }
            else
            {
                Destroy(col.gameObject);
            }
        }
    }
}
