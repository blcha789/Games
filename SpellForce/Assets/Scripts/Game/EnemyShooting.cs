using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    public GameObject spells[];
    public Transform shotPos;
    
    public bool isMelee;
    public float attackDistance;
    
    public AudioSource attackSound;

    private float damage;
    private float attackSpeed;
    
    private bool canAttack = true;
    private bool isSilenced = false;
    private float currentAttackSpeed;
    
    private Transform player;
    private Animator anim;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Character").transform;
        anim = GetComponentInChildren<Animator>();
        currentAttackSpeed = attackSpeed;
    }
    
    public void AttackSettings(float minDamage, float maxDamage, float minAttackSpeed, float maxAttackSpeed)
    {
        damage = Random.Range(minDamage, maxDamage);
        attackSpeed = Random.Range(minAttackSpeed, maxAttackSpeed);
    }

    private void Update()
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
            if (distance <= attackDistance)
            {
                if(!isMelee)
                {
                   ShootSpell(); 
                   attackSound.Play();
                }
                else
                {
                    AttackPlayer();
                    attackSound.Play();
                }
            }
        }
    }
    
    public void Silence(bool _isSilenced)
    {
        isSilenced = _isSilenced;
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
