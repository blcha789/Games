using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingRenderer : MonoBehaviour {

    public Renderer[] objectRenderer;

    private  void OnBecameVisible()
    {
        /*if (objectRenderer[0].enabled == false)
        {
            /*for (int i = 0; i < objectRenderer.Length; i++)
            {
                objectRenderer[i].enabled = true;
            }*/
            objectRenderer[0].enabled = true;
            Debug.Log("VISIBLE");
        //}
    }

    private  void OnBecameInvisible()
    {
        if (objectRenderer[0].enabled == true)
        {
            /*for (int i = 0; i < objectRenderer.Length; i++)
            {
                objectRenderer[i].enabled = false;
            }*/
            objectRenderer[0].enabled = false;
            Debug.Log("INVISIBLE");
        }
    }
}
