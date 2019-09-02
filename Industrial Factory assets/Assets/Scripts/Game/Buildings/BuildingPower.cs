using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPower : MonoBehaviour
{

    public float capacity = 0;
    public GameObject noElectricitySignal;

    private GameLogic gameLogic;
    private float setCapacity;

    private void Start()
    {
        gameLogic = GameObject.FindGameObjectWithTag("Hierarchy/GameLogic").GetComponent<GameLogic>();
        setCapacity = capacity;
    }

    private void Update()
    {
        if (gameLogic.isPlaying && gameLogic.isPowerInLevel)
        {
            if (capacity <= 0)
            {
                noElectricitySignal.SetActive(true);
            }
            else
            {
                noElectricitySignal.SetActive(false);
            }
        }
    }

    public void SetDefaults()
    {
        capacity = setCapacity;
        noElectricitySignal.SetActive(false);
    }
}
