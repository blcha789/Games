using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    public bool silence = false;

    public float attackDistance;
    public float damage;
    public float attackSpeed;

    private bool canAttack = true;
    private float currentAttackSpeed;
    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Character").transform;
        //anim = GetComponentInChildren<Animator>();
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

        if (canAttack && !silence)
        {
            if (distance <= attackDistance)
            {
                //AttackPlayer();
                //attackSound.Play();
            }
        }
    }
}
