using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{
    public float damage = 20f;
    public float radius = 3f;
    public LayerMask enemyLayerMask;
    public GameObject explosionEffect;

    private void OnTriggerEnter(Collider col)
    {
        if(col.CompareTag("Enemy"))
        {
            Effect();
            ExplosionDamage();
            Destroy(this.gameObject);
        }
    }

    public void Effect()
    {
        GameObject smoke = Instantiate(explosionEffect, transform.position, Quaternion.identity);

        Destroy(smoke, 3f);
    }

    void ExplosionDamage()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius, enemyLayerMask.value);

        for (int i = 0; i < hitColliders.Length; i++)
        {
            float distance = Vector3.Distance(hitColliders[i].transform.position, transform.position);
            hitColliders[i].gameObject.GetComponent<EnemyStats>().TakeDamage(damage - distance * 4);
        }
    }
}
