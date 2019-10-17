﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    
    [Header("Settings")]
    public float minDamage;
    public float maxDamage;
    public float minAttackSpeed;
    public float maxAttackSpeed;
    public float minMoveSpeed;
    public float maxMoveSpeed;


    private float health;
    private BuffSystem buffSystem;
    private EnemyShooting enemyShooting;
    private EnemyMovement enemyMovement;

    void Start()
    {
        buffSystem = GetComponent<BuffSystem>();
        enemyShooting = GetComponent<EnemyShooting>();
        enemyMovement = GetComponent<EnemyMovement>();
        
        SetEnemy();
    }
    
    private void SetEnemy()
    {
        enemyShooting.AttackSettings(minDamage, maxDamage, minAttackSpeed, maxAttackSpeed);
        enemyMovement.MovementSettings(minMoveSpeed, maxMoveSpeed);
    }

    public void TakeDamagePerHit(float damage)
    {
        //buffSystem.AddBuff();
        TakeDamage(damage);
    }

    public void TakeDamagePerSecond(float damage)
    {
        TakeDamage(damage);
    }

    private void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0)
            Death();
    }

    private void Death()
    {

    }
}
