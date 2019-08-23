using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Baricade : MonoBehaviour
{
    public float damage = 10f;

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Enemy"))
        {
            col.GetComponent<EnemyStats>().TakeDamage(damage);
        }
    }
}
