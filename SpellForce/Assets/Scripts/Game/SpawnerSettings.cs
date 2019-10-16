using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Setting/SceneSettings/SpawnerSettings")]
public class SpawnerSettings : ScriptableObject
{
    [Header("Main")]
    public GameObject[] enemies;
    public float radiusSpawn;
    public Transform[] spawnPos;

    [Header("Spawn Settings")]
    public float spawnRate = 5;
    public int startSpawnSize = 10;
    public int multiplierSpawnSize = 5;

    [Header("EnemyMinMaxSettings")]
    public float minHealth;
    public float maxHealth;
    public float minDamage, maxDamage;
    public float minAttackSpeed, maxAttackSpeed;
    public float minMoveSpeed, maxMoveSpeed;
}
