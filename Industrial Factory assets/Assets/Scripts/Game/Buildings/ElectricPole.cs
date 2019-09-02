using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricPole : MonoBehaviour
{
    public float capacity;
    public Transform connection;
    public Vector3 sizeConnection;
    public Vector3 sizePower;
    public LayerMask polesMask;
    public LayerMask machineMask;
    public LayerMask powerPlantMask;
    public GameObject cable;

    public GameObject powerPlantCable;
    public List<GameObject> cables = new List<GameObject>();
    public List<ElectricPole> electricPoles = new List<ElectricPole>();

    private GameLogic gameLogic;
    private List<BuildingPower> buildings = new List<BuildingPower>();

    void Start()
    {
        gameLogic = GameObject.FindGameObjectWithTag("Hierarchy/GameLogic").GetComponent<GameLogic>();
        SetDefaults();
    }

    void Update()
    {
        if (gameLogic.isPlaying)//if is in play mode 
        {
            SendElectricity();
        }
    }

    private void OnDisable()
    {
        ClearCables();
    }

    public void SetDefaults()
    {
        capacity = 0;
    }

    public void ClearCables()
    { 
        foreach (GameObject item in cables)
        {
            Destroy(item);
        }
        cables.Clear();
    }

    public void ClearPowerPlantCable()
    {
        Destroy(powerPlantCable);
        powerPlantCable = null;
    }

    public void FindPoles()
    {
        electricPoles.Clear();
        int oldLayer = gameObject.layer; // This variable now stored our original layer
        gameObject.layer = 0;

        Collider[] hitColliders = Physics.OverlapBox(transform.position, sizeConnection, Quaternion.identity, polesMask.value);//first is self
        gameObject.layer = oldLayer;

        for (int i = 0; i < hitColliders.Length; i++)
        {
            electricPoles.Add(hitColliders[i].GetComponent<ElectricPole>());
        }

        for (int i = 0; i < electricPoles.Count; i++)
        {
            electricPoles[i].FindPole();
        }

        FindPowerPlant();
    }

    public void FindPole()
    {
        electricPoles.Clear();

        int oldLayer = gameObject.layer; // This variable now stored our original layer
        gameObject.layer = 0;

        Collider[] hitColliders = Physics.OverlapBox(transform.position, sizeConnection, Quaternion.identity, polesMask.value);//first is self
        gameObject.layer = oldLayer;

        for (int i = 0; i < hitColliders.Length; i++)
        {
            electricPoles.Add(hitColliders[i].GetComponent<ElectricPole>());
        }

        if (electricPoles.Count > 0)
            CreateCable();
    }

    private void FindPowerPlant()
    {
        Collider[] hitColliders = Physics.OverlapBox(transform.position, sizeConnection, Quaternion.identity, powerPlantMask.value);
        if (hitColliders.Length > 0)
            hitColliders[0].GetComponent<PowerPlant>().FindPoles();
    }

    public void FindMachines()
    {
        buildings.Clear();

        Collider[] hitColliders = Physics.OverlapBox(transform.position, sizePower, Quaternion.identity, machineMask.value);

        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].tag != "Building/Conveyor")
                buildings.Add(hitColliders[i].GetComponent<BuildingPower>());
        }
    }

    private void CreateCable()
    {
        ClearCables();

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
            electricPoles[i].cables.Add(c);
        }
    }

    private void SendElectricity()
    {
        if (capacity > 0)
        {
            for (int i = 0; i < buildings.Count; i++)
            {
                if (buildings[i].capacity < 10)
                {
                    buildings[i].capacity += 10 * Time.deltaTime;
                    capacity -= 10 * Time.deltaTime;
                }
            }

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
