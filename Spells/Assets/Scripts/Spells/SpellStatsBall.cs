using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public enum SpellDecal { No, Yes}
public enum SpellDecalOn { Everything, Ground, Objects}

public class SpellStatsBall : NetworkBehaviour
{
    [Header("Main")]
    public float damageHit;
    public float lifeTime;

    [Header("SpellType")]
    public SpellType spellType;
    public float spellTypeDamage;
    public float spellTypeBonusDamage;
    public float spellTypeDuration;

    [Header("SpellEffect")]
    public SpellEffect spellEffect;
    public float spellEffectModifier;
    public float spellEffectDuration;

    [Header("Other")]
    public SpellDecal spellDecal;
    public SpellDecalOn spellDecalOn;
    public GameObject decal;

    [SyncVar]
    Vector3 syncPos;
    float lerpRate = 15;


    void Update()
    {
        if (lifeTime <= 0)
            CmdDestroySpell();
        else
            lifeTime -= Time.deltaTime;
    }


    void OnTriggerEnter(Collider col)
    {
        if (col.transform.tag == "Player")
        {
            if (col.GetComponent<CharacterShooting>().currentSpell != null)
            {
                if (col.GetComponent<CharacterShooting>().currentSpell.name != transform.name)
                {
                    col.transform.GetComponent<CharacterStats>().Damage(damageHit, (int)spellType - 1, spellTypeDamage, spellTypeBonusDamage, spellTypeDuration);
                    col.transform.GetComponent<CharacterStats>().Effect((int)spellEffect - 1, spellEffectModifier, spellEffectDuration);
                    CmdDestroySpell();
                }
            }
            else
            {
                col.transform.GetComponent<CharacterStats>().Damage(damageHit, (int)spellType - 1, spellTypeDamage, spellTypeBonusDamage, spellTypeDuration);
                col.transform.GetComponent<CharacterStats>().Effect((int)spellEffect - 1, spellEffectModifier, spellEffectDuration);
                CmdDestroySpell();
            }
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            if (spellDecal == SpellDecal.Yes)
            {
                if (spellDecalOn == SpellDecalOn.Ground)
                {
                    if (col.transform.tag == "Ground")
                        Instantiate(decal, hit.point, Quaternion.LookRotation(hit.point)).transform.SetParent(col.transform, true);
                }
                else if (spellDecalOn == SpellDecalOn.Objects)
                {
                    if (col.transform.tag == "Objects")
                        Instantiate(decal, hit.point, Quaternion.LookRotation(hit.point)).transform.SetParent(col.transform, true);
                }
                else
                    Instantiate(decal, hit.point, Quaternion.LookRotation(hit.point)).transform.SetParent(col.transform, true);
            }
        }
    }

    void FixedUpdate()
    {
        TransmitPosition();
        if (!hasAuthority)
            transform.position = Vector3.Lerp(transform.position, syncPos, Time.deltaTime * lerpRate);
    }

    [Command]
    void CmdTransmitPosition(Vector3 pos)
    {
        syncPos = pos;
    }

    [ClientCallback]
    void TransmitPosition()
    {
        if (hasAuthority)
            CmdTransmitPosition(transform.position);
    }

    [Command]
    void CmdDestroySpell()
    {
        NetworkServer.Destroy(this.gameObject);
    }
}
