using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recycler : MonoBehaviour {

    public AudioSource audioSource; //sound of building
    public BuildingPower buildingPower;
    public GameObject showObjectsAfterPlay; //smoke

    private GameLogic gameLogic;

    private void Start()
    {
        gameLogic = GameObject.FindGameObjectWithTag("Hierarchy/GameLogic").GetComponent<GameLogic>();
    }

    private void Update()
    {
        if (gameLogic.isPlaying)//if is in play mode
        {
            if (!gameLogic.isPowerInLevel || buildingPower.capacity >= 1)
            {
                if (!audioSource.isPlaying)//if sound of building is off then start building sound
                    audioSource.Play();

                showObjectsAfterPlay.SetActive(true);
            }
        }
        else//if is in build mode
        {
            if (audioSource.isPlaying) //if sound is playing then stop it
            {
                audioSource.Stop();
                showObjectsAfterPlay.SetActive(false);
                buildingPower.SetDefaults();
            }
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (!gameLogic.isPowerInLevel || buildingPower.capacity >= 1)
        {
            if (col.tag.Contains("ItemModel"))//destroy item which contain tag ItemModel
            {
                Destroy(col.gameObject);
                buildingPower.capacity -= 0.1f;
            }
        }
    }
}
