using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellStatsWall : MonoBehaviour
{
    [Header("Main")]
    public Spell spell;
    public float lifeTime;

    [Header("Positive")]
    public float healing;

    [Header("Negative")]
    public float damage;

    [Header("SpellType")]
    public SpellType spellType;
    public float spellTypeDamage;
    public float spellTypeBonusDamage;
    public float spellTypeDuration;

    [Header("SpellEffect")]
    public SpellEffect spellEffect;
    public float spellEffectModifier;
    public float spellEffectDuration;

    void Update()
    {
        if (lifeTime <= 0)
            DestroySpell();
        else
            lifeTime -= Time.deltaTime;
    }

    void OnTriggerStay(Collider col)
    {
        if (col.tag == "Player")
        {
            if (spell == Spell.Positive)
            {
                col.GetComponent<CharacterStats>().health += healing * Time.deltaTime;
            }
            else
            {
                col.GetComponent<CharacterStats>().health -= damage * Time.deltaTime;
            }

            col.transform.GetComponent<CharacterStats>().Damage(0, (int)spellType - 1, spellTypeDamage, spellTypeBonusDamage, spellTypeDuration);
            col.transform.GetComponent<CharacterStats>().Effect((int)spellEffect - 1, spellEffectModifier, spellEffectDuration);
        }
    }

    void DestroySpell()
    {
        Destroy(gameObject);
    }
}
