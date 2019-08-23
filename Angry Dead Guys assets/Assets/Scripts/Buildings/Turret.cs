using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [Header("Attributes")]
    public float damage;
    public float fireRate;
    public int magazineSize;
    public float reloadTime;
    public float bulletSpeed;
    public float range;
    public float rotateSpeed;
    public Color color;

    [Header("Other")]
    public GameObject bulletPrefab;
    public Transform shotPos;
    public Transform partToRotate;
    public AudioSource shootingSound;

    private float fireCountDown, reloadCountDown;
    private int magazineCurrentSize;
    private Transform target;
    private GameLogic gameLogic;

    void Start()
    {
        gameLogic = GameObject.FindGameObjectWithTag("GameLogic").GetComponent<GameLogic>();

        reloadCountDown = reloadTime;
        magazineCurrentSize = magazineSize;
    }

    void Update()
    {
        if (gameLogic.statusMode == StatusMode.Play)
       {
            if (target == null)
                UpdateTarget();
            else
            {
                Shoot();
                Rotate();
            }
        }
    }

    private void UpdateTarget()
    {
        GameObject[] enemy = GameObject.FindGameObjectsWithTag("Enemy");

        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        for (int i = 0; i < enemy.Length; i++)
        {
            float distance = Vector3.Distance(transform.position, enemy[i].transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestEnemy = enemy[i];
            }
        }

        if (nearestEnemy != null && shortestDistance <= range)
            target = nearestEnemy.transform;
        else
            target = null;
    }
    
    private void Shoot()
    {
        if (magazineCurrentSize > 0)
        {
            if (fireCountDown <= 0)
            {
                GameObject bullet = Instantiate(bulletPrefab, shotPos.position, Quaternion.identity);

                bullet.GetComponent<TurretBulletStats>().SetParameters(damage, bulletSpeed, target);
                bullet.GetComponent<MeshRenderer>().material.color = color;

                --magazineCurrentSize;
                fireCountDown = fireRate;
                shootingSound.Play();
            }
            else
                fireCountDown -= Time.deltaTime;
        }
        else
        {
            if (reloadCountDown <= 0)
            {
                magazineCurrentSize = magazineSize;
                reloadCountDown = reloadTime;
            }
            else
                reloadCountDown -= Time.deltaTime;
        }
    }

    private void Rotate()
    {
        Vector3 dir = target.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(partToRotate.rotation, lookRotation, Time.deltaTime * rotateSpeed).eulerAngles;

        partToRotate.rotation = Quaternion.Euler(0f, rotation.y, 0f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
