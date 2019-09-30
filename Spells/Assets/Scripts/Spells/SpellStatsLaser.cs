using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SpellStatsLaser : NetworkBehaviour {

    [Header("Main")]
    public float damageHit;

    [Header("SpellType")]
    public SpellType spellType;
    public float spellTypeDamage;
    public float spellTypeBonusDamage;
    public float spellTypeDuration;

    [Header("SpellEffect")]
    public SpellEffect spellEffect;
    public float spellEffectModifier;
    public float spellEffectDuration;

    [SyncVar]
    Vector3 syncPos;
    [SyncVar]
    Quaternion syncRot;
    float lerpRate = 15;

    void OnParticleCollision(GameObject col)
    {
        if (col.transform.tag == "Player")
        {
            col.GetComponent<CharacterStats>().health -= damageHit * Time.deltaTime;
            col.transform.GetComponent<CharacterStats>().Damage(0, (int)spellType - 1, spellTypeDamage, spellTypeBonusDamage, spellTypeDuration);
            col.transform.GetComponent<CharacterStats>().Effect((int)spellEffect - 1, spellEffectModifier, spellEffectDuration);
        }
    }

    void FixedUpdate()
    {
        TransmitPosition();
        if (!hasAuthority)
        {
            transform.position = Vector3.Lerp(transform.position, syncPos, Time.deltaTime * lerpRate);
            transform.rotation = Quaternion.Lerp(transform.rotation, syncRot, Time.deltaTime * lerpRate);
        }
    }

    [Command]
    void CmdTransmitPosition(Vector3 pos, Quaternion rot)
    {
        syncPos = pos;
        syncRot = rot;
    }

    [ClientCallback]
    void TransmitPosition()
    {
        if (hasAuthority)
            CmdTransmitPosition(transform.position, transform.rotation);
    }
}
