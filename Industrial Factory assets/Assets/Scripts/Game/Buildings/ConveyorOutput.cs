using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorOutput : MonoBehaviour
{
    public GameObject item;
    public Transform spawnPos;
    public GameObject showObjectsAfterPlay;

    public float timeSpawn;

    private float setTimeSpawn;

    private GameLogic gameLogic;
    private Transform itemParent;

    private void Start()
    {
        SetTime();
        gameLogic = GameObject.FindGameObjectWithTag("Hierarchy/GameLogic").GetComponent<GameLogic>();
        itemParent = GameObject.FindGameObjectWithTag("Hierarchy/Items").transform;
    }

    private void SetTime()
    {
        setTimeSpawn = timeSpawn;
    }

    private void Update()
    {
        if (gameLogic.isPlaying)
        {
            if (showObjectsAfterPlay != null)
                showObjectsAfterPlay.SetActive(true);

            if (timeSpawn <= 0)
            {
                Instantiate(item, spawnPos.position, spawnPos.rotation, itemParent);
                timeSpawn = setTimeSpawn;
            }
            else
                timeSpawn -= Time.deltaTime;
        }
        else
        {
            timeSpawn = setTimeSpawn;

            if (showObjectsAfterPlay != null)
                showObjectsAfterPlay.SetActive(false);
        }
    }
}
