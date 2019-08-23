using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearTrap : MonoBehaviour
{
    public float damage = 15f;
    public Animator anim;

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Enemy"))
        {
            col.GetComponent<EnemyStats>().TakeDamage(damage);
            col.GetComponent<EnemyMovement>().movementSpeed = 0;

            anim.SetTrigger("TriggerTrap");
            GetComponent<Collider>().enabled = false;
            transform.parent.parent = col.transform;
        }
    }
}
