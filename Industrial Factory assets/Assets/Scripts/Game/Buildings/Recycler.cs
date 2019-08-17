using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recycler : MonoBehaviour {

    public AudioSource audioSource; //sound of building

    private GameLogic gameLogic;

    private void Start()
    {
        gameLogic = GameObject.FindGameObjectWithTag("Hierarchy/GameLogic").GetComponent<GameLogic>();
    }

    private void Update()
    {
        if (gameLogic.isPlaying)//if is in play mode
        {
            if (!audioSource.isPlaying)//if sound of building is off then start building sound
                audioSource.Play();
        }
        else//if is in build mode
        {
            if (audioSource.isPlaying) //if sound is playing then stop it
                audioSource.Stop();
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.tag.Contains("ItemModel"))//destroy item which contain tag ItemModel
        {
            Destroy(col.gameObject);
        }
    }
}
