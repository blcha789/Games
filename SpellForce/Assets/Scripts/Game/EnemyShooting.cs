using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    public GameObject spells[];
    public bool isMelee;
    public Transform shotPos;
    
    public float attackDistance;
    public float damage;
    public float attackSpeed;
    public AudioSource attackSound;
    public bool isSilenced = false;

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

        if (canAttack && !isSilenced)
        {
            RaycastHit hit;
            
            if (distance <= attackDistance)
            {
                if(isMelee)
                {
                   ShootSpell(); 
                }
                else
                {
                    AttackPlayer();
                    attackSound.Play();
                }
            }
        }
    }

    private void AttackPlayer()
    {
        anim.SetTrigger("AttackMelee");
        canAttack = false;
        player.GetComponent<CharacterStats>().TakeDamage(damage);
    }

    private void ShootSpell()
    {
        anim.SetTrigger("AttackRange");
        canAttack = false;
        int spellId = Random.Range(0, spells.lenght);
        Instantiate(spells[spellId], shotPos.position, Quartenion.identity);
    }
}
