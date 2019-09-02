using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Splitter : MonoBehaviour {


    public Transform splitPos; //position when spiliting
    public BuildingPower buildingPower;

    private bool split = false;

    private GameLogic gameLogic;

    private void Start()
    {
        gameLogic = GameObject.FindGameObjectWithTag("Hierarchy/GameLogic").GetComponent<GameLogic>();
    }

    private void OnTriggerEnter(Collider col)
    {
        if (!gameLogic.isPowerInLevel || buildingPower.capacity >= 1)
        {
            if (col.tag == "Item")//if item have tag Item
            {
                if (!split) // if not spliting then will item go forward next item will split
                {
                    split = !split;
                }
                else//if splitting item will change position to splitPos and next inem will not split
                {
                    col.transform.parent.position = splitPos.position;
                    split = !split;
                }

                buildingPower.capacity -= 0.1f;
            }
        }
    }
}
