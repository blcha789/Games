using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public float health;
    BuffSystem buffSystem;

    void Start()
    {
        buffSystem = GetComponent<BuffSystem>();
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
