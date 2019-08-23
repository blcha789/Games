using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBoxSpawner : MonoBehaviour
{
    public TextAsset ammoBoxesTextFile;
    public Transform spawnPos;
    public Vector2 spawnRange;

    public List<GameObject> ammoBoxes = new List<GameObject>();
    public int maxAmmoBoxesOnGround;
    public LayerMask ammoBoxLayerMask;

    private int ammoBoxCounter;

    private GameLogic gameLogic;

    void Start()
    {
        gameLogic = GetComponent<GameLogic>();
        LoadAmmoBoxes();
    }

    private void LoadAmmoBoxes()
    {
        string[] lines = ammoBoxesTextFile.text.Split('\n');//split to lines

        for (int i = 0; i < lines.Length; i++)
        {
            string[] line = lines[i].Split('\t');//split line 

            ammoBoxes.Add(Resources.Load<GameObject>("Prefabs/AmmoBoxes/" + line[0]));
        }
    }

    public void SpawnAmmoBox()
    {
        StartCoroutine(SpawningAmmoBox());
    }

    public void DecreaseAmmoOnGround()
    {
        ammoBoxCounter--;
    }

    private IEnumerator SpawningAmmoBox()
    {
        while (gameLogic.statusMode == StatusMode.Play)
        {
            float t = Random.Range(3, 8);
            yield return new WaitForSeconds(t);

            if (ammoBoxCounter < maxAmmoBoxesOnGround)
            {
                Vector3 pos = new Vector3(Random.Range(-spawnRange.x, spawnRange.x), 0, Random.Range(-spawnRange.y, spawnRange.y)) + spawnPos.position;
                Collider[] hitColliders = Physics.OverlapSphere(pos, 1f, ammoBoxLayerMask.value);

                if (hitColliders.Length == 0)
                {
                    GameObject go = ammoBoxes[Random.Range(0, ammoBoxes.Count)];

                    GameObject ammo = Instantiate(go, pos, Quaternion.identity);
                    ammo.name = go.name;
                    ammoBoxCounter++;
                }
            }
        }
        yield break;
    }
}
