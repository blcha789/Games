using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SpellStatsWallBall : NetworkBehaviour {

    public GameObject WallPrefab;
    public float lifeTime;

    [SyncVar]
    Vector3 syncPos;
    float lerpRate = 15;

    void FixedUpdate()
    {
        lifeTime -= Time.deltaTime;

        if (lifeTime < 0)
            CmdDestroySpell(this.gameObject);

        TransmitPosition();
        if (!hasAuthority)
        {
            transform.position = Vector3.Lerp(transform.position, syncPos, Time.deltaTime * lerpRate);
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.transform.tag == "Ground")
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit))
            {
                Quaternion rotation = Quaternion.LookRotation(hit.normal);
                CmdCreateSpell(hit.point, rotation);
            }
            CmdDestroySpell(this.gameObject);
        }
    }

    [Command]
    void CmdCreateSpell(Vector3 pos, Quaternion rot)
    {
        GameObject spell = Instantiate(WallPrefab, pos, rot);
        NetworkServer.Spawn(spell);
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
    void CmdDestroySpell(GameObject spell)
    {
        NetworkServer.Destroy(spell);
    }
}
