using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blade : MonoBehaviour
{
    public float damage = 10f;
    public float slowSpeed = 1f;

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Enemy"))
        {
            col.GetComponent<EnemyStats>().TakeDamage(damage);
            col.GetComponent<EnemyMovement>().Slow(slowSpeed);
        }
    }
}
