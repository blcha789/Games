using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerPlant : MonoBehaviour
{
    [Header("Main")]
    public float craftingTime = 5f;
    public float capacity = 0;
    public GameObject cable;
    public Transform connection;
    public LayerMask polesMask;

    [Header("Inputs")]//Script checking what fluid is on inputs
    public CheckInputFluid inputFluid1;
    public CheckInputItem inputItem1;

    [Header("Storage")]
    public float fluidCount;
    public float itemCount;

    public List<ElectricPole> electricPoles = new List<ElectricPole>();
    public List<GameObject> cables = new List<GameObject>();

    private float needFluid = 1;
    private float needItem = 1;

    private float outputElectricityCapacity;

    private float setCraftingTime;
    private GameLogic gameLogic;
    private Transform itemParent;
    private BuildingsUI buildingsUI;

    private void Start()
    {
        gameLogic = GameObject.FindGameObjectWithTag("Hierarchy/GameLogic").GetComponent<GameLogic>();
        itemParent = GameObject.FindGameObjectWithTag("Hierarchy/Items").transform;
        buildingsUI = GetComponent<BuildingsUI>();
        setCraftingTime = craftingTime;
        SetDefaults();
    }

    public void SetDefaults()
    {
        fluidCount = 0;
        itemCount = 0;

        inputFluid1.fluidAmount = 0;
        capacity = 30;

        craftingTime = setCraftingTime;
    }

    public void FindPoles()
    {
        SetDefaults();
        electricPoles.Clear();

        Collider[] hitColliders = Physics.OverlapBox(transform.position, new Vector3(7.5f, 1, 7.5f), Quaternion.identity, polesMask.value);

        for (int i = 0; i < hitColliders.Length; i++)
        {
            electricPoles.Add(hitColliders[i].GetComponent<ElectricPole>());
        }

        for (int i = 0; i < electricPoles.Count; i++)
        {
            electricPoles[i].FindPole();
        }

        if (electricPoles.Count > 0)
            CreateCables();
    }

    private void CreateCables()
    {
        for (int i = 0; i < cables.Count; i++)
        {
            Destroy(cables[i]);
        }
        cables.Clear();

        for (int i = 0; i < electricPoles.Count; i++)
        {
            Vector3 pos = (electricPoles[i].connection.position + connection.position) / 2;

            GameObject c = Instantiate(cable, pos, Quaternion.identity);

            Vector3 dir = connection.position - c.transform.position;
            Quaternion rot = Quaternion.LookRotation(dir);
            c.transform.rotation = rot;

            //scale
            float distance = Vector3.Distance(connection.position, electricPoles[i].connection.position);
            c.transform.localScale = new Vector3(1, 1, distance);

            cables.Add(c);
            electricPoles[i].powerPlantCable = c;
        }
    }

    void Update()
    {
        if (gameLogic.isPlaying)
        {
            if (fluidCount >= needFluid && itemCount >= needItem)
            {
                if (craftingTime > 0)
                {
                    craftingTime -= Time.deltaTime;
                }
                else
                {
                    capacity += 20;
                    fluidCount -= needFluid;
                    itemCount -= needItem;
                    craftingTime = setCraftingTime;
                }
            }

            if(capacity > 0)
            {
                for (int i = 0; i < electricPoles.Count; i++)
                {
                    if (electricPoles[i].capacity < capacity)
                    {
                        electricPoles[i].capacity += 10 * Time.deltaTime;
                        capacity -= 10 * Time.deltaTime;
                    }
                }
            }
        }
    }
}
