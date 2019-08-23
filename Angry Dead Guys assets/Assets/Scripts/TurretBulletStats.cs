using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBulletStats : MonoBehaviour
{

    public float damage;
    public float speed;
    public Transform target;
    public GameObject hitEffect;

    public void SetParameters(float _damage, float _speed, Transform _target)
    {
        damage = _damage;
        speed = _speed;
        target = _target;
    }

    void Update()
    {
        if (target == null)
            Destroy(gameObject);
        else
            Move();
    }

    private void Move()
    {
        Vector3 distance = target.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        transform.Translate(distance.normalized * distanceThisFrame, Space.World);
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Enemy")
        {
            col.GetComponent<EnemyStats>().TakeDamage(damage);
            GameObject blood = Instantiate(hitEffect, col.transform.position, Quaternion.identity);
            Destroy(blood, 1f);
            Destroy(this.gameObject);
        }
    }
}
