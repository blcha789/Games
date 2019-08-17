using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Furnace : MonoBehaviour
{

    [Header("Main")]
    public float craftingTime;//Time to craft item
    public GameObject[] inputPrefab;//set items that will accept on input
    public GameObject[] outputItemPrefab;//items that will be crafted
    public Transform spawnPos;//Position where crafted item will spawn
    public CheckInputItem input;//Script checking what item is on input 1

    public List<string> items = new List<string>(); //list of items that will be smelt 

    private float setCraftingTime;//Stored crafting time

    private GameLogic gameLogic;
    private Transform itemParent;
    private AudioSource audioSource;

    private void Start()
    {
        gameLogic = GameObject.FindGameObjectWithTag("Hierarchy/GameLogic").GetComponent<GameLogic>();
        itemParent = GameObject.FindGameObjectWithTag("Hierarchy/Items").transform;
        audioSource = GetComponent<AudioSource>();

        setCraftingTime = craftingTime;
        input.itemName = "...";
    }

    //This function set recipe and building to default state
    public void SetDefaults()
    {
        items.Clear();
        craftingTime = setCraftingTime;
        audioSource.Stop();
    }

    private void Update()
    {
        if (gameLogic.isPlaying)//if is in play mode 
        {
            if (!audioSource.isPlaying)//if sound of building is off then play sound
                audioSource.Play();

            if (items.Count >= 1) //if is in list 1 or more items
            {
                if (craftingTime > 0) //if crafting time is greater than 0, decrease crafting time by time
                {
                    craftingTime -= Time.deltaTime;
                }
                else// if is less than 0 spawn crafted item
                {
                    bool isNotThere = true;
                    //according to which item is melted, spawn crafted item on output
                    for (int i = 0; i < inputPrefab.Length; i++)
                    {
                        if (inputPrefab[i].name + "(Clone)" == items[0]) 
                        {
                            Instantiate(outputItemPrefab[i], spawnPos.position, Quaternion.identity, itemParent);
                            craftingTime = setCraftingTime;
                            items.RemoveAt(0);
                            isNotThere = false;
                            break;
                        }
                    }

                    if (isNotThere)//when done melting remove first item in list
                    {
                        items.RemoveAt(0);
                        craftingTime = setCraftingTime;
                    }
                }
            }
        }
    }
}
