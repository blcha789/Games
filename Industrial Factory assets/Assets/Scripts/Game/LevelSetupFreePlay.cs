using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSetupFreePlay : MonoBehaviour
{

    public TextAsset itemDatabase;
    public TextAsset fluidDatabase;

    public InputField mapSizeX;
    public InputField mapSizeZ;
    public InputField buyers;

    public InputField[] oreDeposits;
    public InputField[] fluidDeposits;
    public InputField[] buildings;

    public GameObject[] oreDepositsPrefab;
    public GameObject[] fluidDepositsPrefab;

    private LevelSetup levelSetup;
    private BuildingList buildingList;

    private int sizeX, sizeZ;
    private string[] items, fluids;

    private List<int> buyersRotation = new List<int>();

    private List<Vector3> upPosition = new List<Vector3>();
    private List<Vector3> leftPosition = new List<Vector3>();
    private List<Vector3> downPosition = new List<Vector3>();
    private List<Vector3> rightPosition = new List<Vector3>();

    private TouchCamera touchCamera;

    public void Start()
    {
        levelSetup = GetComponent<LevelSetup>();
        buildingList = GetComponent<BuildingList>();
        touchCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<TouchCamera>();
    }

    public void Setup()
    {
        sizeX = int.Parse(mapSizeX.text);
        sizeZ = int.Parse(mapSizeZ.text);

        //map size
        levelSetup.sizeX = sizeX;
        levelSetup.sizeZ = sizeZ;

        LoadDatabase();
        Buyers();
        OreDeposits();
        FluidDeposits();
        Buildings();

        touchCamera.SetCameraDefault();
        levelSetup.GameSetup();
        buildingList.GameSetup();
    }

    private void LoadDatabase()
    {
        items = itemDatabase.text.Split('\n');
        fluids = fluidDatabase.text.Split('\n');
    }

    private void Buyers()
    {
        int count = int.Parse(buyers.text);

        int rot = 0;
        for (int i = 0; i < count; i++)
        {
            if (rot >= 4)
                rot = 0;

            buyersRotation.Add(rot);
            rot++;
        }

        levelSetup.buyerInput = new BuyerSetup[count];
        for (int i = 0; i < count; i++)
        {
            BuyerSetup buyer = new BuyerSetup();

            if(buyersRotation[i] == 0)
            {
                buyer.rotation = Rotation.Up;
                Vector3 pos = Vector3.zero;

                if (upPosition.Count > 0)
                {
                    pos = new Vector3(0, 1f, Random.Range(4, (sizeZ / 10) + 3));
                    pos = new Vector3(0, 1, pos.z) + new Vector3(0, 0, upPosition[upPosition.Count - 1].z);
                }
                else
                    pos = new Vector3(0, 1f, Random.Range(3, (sizeZ / 10) + 3));

                buyer.position = pos;
                upPosition.Add(pos);

            }
            else if(buyersRotation[i] == 1)
            {
                buyer.rotation = Rotation.Left;
                Vector3 pos = Vector3.zero;

                if (leftPosition.Count > 0)
                {
                    pos = new Vector3(Random.Range(4, sizeX / 10) + 3, 1f, 0);
                    pos = new Vector3(pos.x, 1f, 0) + new Vector3(leftPosition[leftPosition.Count - 1].x, 0, 0);
                }
                else
                    pos = new Vector3(Random.Range(3, (sizeX / 10) + 3), 1f, 0);

                buyer.position = pos;
                leftPosition.Add(pos);
            }
            else if(buyersRotation[i] == 2)
            {
                buyer.rotation = Rotation.Down;
                Vector3 pos = Vector3.zero;

                if (downPosition.Count > 0)
                {
                    pos = new Vector3(0, 1f, Random.Range(4, (sizeZ / 10) + 3));
                    pos = new Vector3(sizeX - 1, 1f, pos.z) + new Vector3(0, 0, downPosition[downPosition.Count - 1].z);
                }
                else
                    pos = new Vector3(sizeX - 1, 1f, Random.Range(3, (sizeZ / 10) + 3));

                buyer.position = pos;
                downPosition.Add(pos);
            }
            else
            {
                buyer.rotation = Rotation.Right;
                Vector3 pos = Vector3.zero;

                if (rightPosition.Count > 0)
                {
                    pos = new Vector3(Random.Range(4, sizeX / 10) + 3, 1f, 0);
                    pos = new Vector3(pos.x, 1, sizeZ - 1) + new Vector3(rightPosition[rightPosition.Count - 1].x, 0, 0);
                }
                else
                    pos = new Vector3(Random.Range(3, (sizeX / 10) + 3), 1f, sizeZ - 1);

                buyer.position = pos;
                rightPosition.Add(pos);
            }

            int itemId = Random.Range(1, items.Length);
            int fluidId = Random.Range(1, fluids.Length);

            int x = Random.Range(0, 10);
            if(x == 0) //item and fluid
            {
                string[] item = items[itemId].Split('\t');
                string[] fluid = fluids[fluidId].Split('\t');//meno, color
                string[] colors= fluid[1].Split(',');

                buyer.item = Resources.Load<GameObject>("Prefabs/Items/" + item[0]);
                buyer.fluidName = fluid[0];

                buyer.itemImage = Resources.Load("Images/Items/" + item[0], typeof(Sprite)) as Sprite;
                buyer.fluidColor = new Color(float.Parse(colors[0]) / 255f, float.Parse(colors[1]) / 255f, float.Parse(colors[2]) / 255f);

                buyer.itemAmount = 5;
                buyer.fluidAmount = 10;
            }
            else if(x > 0 && x < 3) //fluid
            {
                string[] fluid = fluids[fluidId].Split('\t');//meno, color
                string[] colors = fluid[1].Split(',');

                buyer.item = null;
                buyer.fluidName = fluid[0];

                buyer.itemImage = null;
                buyer.fluidColor = new Color(float.Parse(colors[0]) / 255f, float.Parse(colors[1]) / 255f, float.Parse(colors[2]) / 255f);

                buyer.itemAmount = 0;
                buyer.fluidAmount = 10;
            }
            else //item
            {
                string[] item = items[itemId].Split('\t');

                buyer.item = Resources.Load<GameObject>("Prefabs/Items/" + item[0]);
                buyer.fluidName = "";

                buyer.itemImage = Resources.Load("Images/Items/" + item[0], typeof(Sprite)) as Sprite;
                buyer.fluidColor = Color.clear;

                buyer.itemAmount = 5;
                buyer.fluidAmount = 0;
            }

            levelSetup.buyerInput[i] = buyer;
        }

    }

    private void OreDeposits()
    {
        int countDeposits = 0;
        int count = 0;

        for (int i = 0; i < oreDeposits.Length; i++)
        {
            countDeposits += int.Parse(oreDeposits[i].text);
        }

        levelSetup.oreDeposits = new Deposits[countDeposits];

        for (int i = 0; i < oreDeposits.Length; i++)
        {
            for (int j = 0; j < int.Parse(oreDeposits[i].text); j++)
            {
                Deposits deposit = new Deposits();

                deposit.Name = oreDepositsPrefab[i].name;
                deposit.prefab = oreDepositsPrefab[i];
                deposit.position = new Vector3(Random.Range(1, sizeX - 2), 0.5f, Random.Range(1, sizeZ - 2));
                deposit.depositSize = 20000;

                levelSetup.oreDeposits[count] = deposit;
                count++;
            }
        }
    }

    private void FluidDeposits()
    {
        int countDeposits = 0;
        int count = 0;

        for (int i = 0; i < fluidDeposits.Length; i++)
        {
            countDeposits += int.Parse(fluidDeposits[i].text);
        }

        levelSetup.fluidDeposits = new Deposits[countDeposits];

        for (int i = 0; i < fluidDeposits.Length; i++)
        {
            for (int j = 0; j < int.Parse(fluidDeposits[i].text); j++)
            {
                Deposits deposit = new Deposits();

                deposit.Name = fluidDepositsPrefab[i].name;
                deposit.prefab = fluidDepositsPrefab[i];
                deposit.position = new Vector3(Random.Range(1, sizeX - 2), 0.5f, Random.Range(1, sizeZ - 2));
                deposit.depositSize = 20000;

                levelSetup.fluidDeposits[count] = deposit;
                count++;
            }
        }
    }

    private void Buildings()
    {
        for (int i = 0; i < buildingList.chooseBuildings.Length; i++)
        {
            buildingList.chooseBuildings[i].amount = int.Parse(buildings[i].text);
        }
    }
}
