using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiningDrill : MonoBehaviour
{

    public Transform spawnPos;//Position where mined item will spawn
    public Transform rotatingObject;//object(drill) that will rotate
    public LayerMask depositLayerMask;//mask for ore deposits
    public GameObject itemOutputCanvas;
    public GameObject showObjectsAfterPlay;

    public float timeSpawn;//mining time

    private float setTimeSpawn;

    private GameLogic gameLogic;
    private Transform itemParent;
    private OreDeposit oreDeposit;
    private AudioSource audioSource;

    private void Start()
    {
        SetTime();
        gameLogic = GameObject.FindGameObjectWithTag("Hierarchy/GameLogic").GetComponent<GameLogic>();
        itemParent = GameObject.FindGameObjectWithTag("Hierarchy/Items").transform;
        audioSource = GetComponent<AudioSource>();
    }

    //set mining time
    private void SetTime()
    {
        setTimeSpawn = timeSpawn;
    }

    //This function set recipe and building to default state
    public void SetDefaults()
    {
        itemOutputCanvas.SetActive(false);
        itemOutputCanvas.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = null;
        showObjectsAfterPlay.SetActive(false);
        audioSource.Stop();
    }

    //This function is called on play to check if under mining drill is ore deposit and if is set mined item to output
    public void FindDeposit()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position + new Vector3(0, 0, 0.5f), Vector3.down, out hit, Mathf.Infinity, depositLayerMask.value))
        {
            oreDeposit = hit.collider.GetComponent<OreDeposit>();
            itemOutputCanvas.SetActive(true);
            itemOutputCanvas.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = hit.collider.GetComponent<OreDeposit>().outputImage;
            showObjectsAfterPlay.SetActive(true);
        }
    }

    private void Update()
    {
        if (oreDeposit != null) //if drill find deposit 
        {
            if (gameLogic.isPlaying)//if is in play mode 
            {
                if (rotatingObject != null) //if is not empty
                    rotatingObject.Rotate(0, 2, 0);//rotate drill

                if (!audioSource.isPlaying)//if building sound off then play it
                    audioSource.Play();

                if (timeSpawn <= 0)//if mining time is less than 0 spawn ore
                {
                    Instantiate(oreDeposit.ore, spawnPos.position, spawnPos.rotation, itemParent);
                    timeSpawn = setTimeSpawn;
                    oreDeposit.depositSize--;
                }
                else//if miniong time is greater than 0, decrease crafting time by time
                    timeSpawn -= Time.deltaTime;
            }
            else
            {
                timeSpawn = setTimeSpawn;
            }
        }
    }
}
