using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [Header("Main")]
    public GameObject[] zombies;
    public float radiusSpawn;
    public Transform[] spawnPos;
    public WaveState waveState = WaveState.Waiting;

    [Header("Zombie Spawn Settings")]
    public float spawnRate = 5;
    public int startSpawnSize = 10;
    public int multiplierSPawnSize = 5;

    [Header("EnemyMinMaxSettings")]
    public float minHealth;
    public float maxHealth;
    public float minDamage, maxDamage;
    public float minAttackSpeed, maxAttackSpeed;
    public float minMoveSpeed, maxMoveSpeed;

    private int countEnemyInWave = 1;
    private float searchCountDown = 1f;
    private GameLogic gameLogic;

    private void Start()
    {
        gameLogic = GetComponent<GameLogic>();
    }

    private void Update()
    {
        if(waveState == WaveState.Complete)
        {
            if(!EnemyIsAlive() && gameLogic.statusMode == StatusMode.Play)
            {
                WaveComplete();
            }
        }
    }

    bool EnemyIsAlive()
    {
        searchCountDown -= Time.deltaTime;

        if (searchCountDown <= 0)
        {
            searchCountDown = 1f;
            if (GameObject.FindGameObjectWithTag("Enemy") == null)
            {
                return false;
            }
        }

        return true;
    }

    private void WaveComplete()
    {
        waveState = WaveState.Waiting;
        gameLogic.night++;
        gameLogic.StartDay();
    }

    public void SpawnWave()
    {
        countEnemyInWave = gameLogic.night * multiplierSPawnSize + startSpawnSize;
        StartCoroutine(SpawningWave(countEnemyInWave));
    }

    private IEnumerator SpawningWave(int countEnemy)
    {
        waveState = WaveState.Spawning;

        for (int i = 0; i < countEnemyInWave; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(1f / spawnRate);
        }

        waveState = WaveState.Complete;
        yield break;
    }

    private void SpawnEnemy()
    {
        Transform sp = spawnPos[Random.Range(0, spawnPos.Length)];
        Vector3 pos = RandomCircle(sp.position);
        GameObject e = Instantiate(zombies[Random.Range(0,zombies.Length)], pos, sp.rotation);

        e.GetComponent<EnemyStats>().health = Random.Range(minHealth, maxHealth);
        e.GetComponent<EnemyAttack>().damage = Random.Range(minDamage, maxDamage);
        e.GetComponent<EnemyAttack>().attackSpeed = Random.Range(minAttackSpeed, maxAttackSpeed);
        e.GetComponent<EnemyMovement>().movementSpeed = Random.Range(minMoveSpeed, maxMoveSpeed);
    }

    Vector3 RandomCircle(Vector3 center)
    {
        float ang = Random.value * 360;
        Vector3 pos;
        pos.x = center.x + radiusSpawn * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.y = center.y;
        pos.z = center.z + radiusSpawn * Mathf.Cos(ang * Mathf.Deg2Rad);
        return pos;
    }
}
