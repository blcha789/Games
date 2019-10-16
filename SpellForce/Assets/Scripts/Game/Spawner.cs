using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public SpawnerSettings spawnerSettings;
    public WaveState waveState = WaveState.Waiting;
    
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
            yield return new WaitForSeconds(1f / spawnerSettings.spawnRate);
        }

        waveState = WaveState.Complete;
        yield break;
    }

    private void SpawnEnemy()
    {
        Transform sp = spawnPos[Random.Range(0, spawnerSettings.spawnPos.Length)];
        Vector3 pos = RandomCircle(sp.position);
        Instantiate(spawnerSettings.enemies[Random.Range(0,spawnerSettings.enemies.Length)], pos, sp.rotation);
    }

    Vector3 RandomCircle(Vector3 center)
    {
        float ang = Random.value * 360;
        Vector3 pos;
        pos.x = center.x + spawnerSettings.radiusSpawn * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.y = center.y;
        pos.z = center.z + spawnerSettings.radiusSpawn * Mathf.Cos(ang * Mathf.Deg2Rad);
        return pos;
    }
}
