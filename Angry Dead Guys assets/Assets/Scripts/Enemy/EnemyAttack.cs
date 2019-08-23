using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public float attackDistance;
    public float damage;
    public float attackSpeed;
    public LayerMask buildingLayerMask;
    public AudioSource attackSound;

    private Transform player;

    private bool canAttack = true;
    private float currentAttackSpeed;
    private Animator anim;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Character").transform;
        anim = GetComponentInChildren<Animator>();
        currentAttackSpeed = attackSpeed;
    }

    void Update()
    {
        if (currentAttackSpeed <= 0)
        {
            canAttack = true;
            currentAttackSpeed = attackSpeed;
        }
        else
        {
            if (!canAttack)
                currentAttackSpeed -= Time.deltaTime;
        }

        float distance = Vector3.Distance(player.position, transform.position);

        if (canAttack)
        {
            RaycastHit hit;

            if (distance <= attackDistance)
            {
                AttackPlayer();
                attackSound.Play();
            }
            else if (Physics.Raycast(transform.position, transform.forward, out hit, 0.5f, buildingLayerMask.value))
            {
                AttackBuilding(hit);
                attackSound.Play();
            }
        }
    }

    private void AttackPlayer()
    {
        anim.SetTrigger("Attack");
        canAttack = false;
        player.GetComponent<CharacterStats>().TakeDamage(damage);
    }

    private void AttackBuilding(RaycastHit hit)
    {
        anim.SetTrigger("Attack");
        canAttack = false;
        hit.transform.GetComponent<BuildingStats>().TakeDamage(damage);
    }
}
