using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletStats : MonoBehaviour
{
    public float damage;
    public float lifeTime;
    public GameObject hitEffect;

    void Update()
    {
        Destroy(this.gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Enemy"))
        {
            col.GetComponent<EnemyStats>().TakeDamage(damage);
            GameObject blood = Instantiate(hitEffect, col.transform.position, Quaternion.identity);
            Destroy(blood, 1f);
            Destroy(this.gameObject);
        }

        if(col.CompareTag("Barrel"))
        {
            col.GetComponentInParent<Barrel>().SetFire();
            Destroy(this.gameObject);
        }
    }
}
