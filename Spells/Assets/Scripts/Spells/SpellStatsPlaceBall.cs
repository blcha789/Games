using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SpellStatsPlaceBall : NetworkBehaviour {

    public GameObject prefab;
    public float lifeTime;

    public float startTimePrefabSpawn;
    public float timePrefabSpawn;
    public LayerMask mask;

    float _timePrefabSpawn;

    [SyncVar]
    Vector3 syncPos;
    float lerpRate = 15;

    void Start()
    {
        _timePrefabSpawn = timePrefabSpawn;
        timePrefabSpawn = 3f;
    }

    void FixedUpdate()
    {
        if (startTimePrefabSpawn <= 0)
            lifeTime -= Time.deltaTime;

        if (lifeTime < 0)
            CmdDestroy();

        TransmitPosition();
        if (!hasAuthority)
        {
            transform.position = Vector3.Lerp(transform.position, syncPos, Time.deltaTime * lerpRate);
        }
    }

    void Update()
    {
        if (startTimePrefabSpawn <= 0)
        {
            if (timePrefabSpawn <= 0)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, Vector3.down, out hit, 5f, mask.value))
                {
                    Quaternion rotation = Quaternion.LookRotation(hit.normal);
                    CmdCreateSpell(hit.point, rotation);
                    timePrefabSpawn = _timePrefabSpawn;
                }
            }
            else
                timePrefabSpawn -= Time.deltaTime;
        }
        else
        {
            startTimePrefabSpawn -= Time.deltaTime;
        }
    }

    [Command]
    void CmdDestroy()
    {
        NetworkServer.Destroy(this.gameObject);
    }

    [Command]
    void CmdCreateSpell(Vector3 pos, Quaternion rot)
    {
        GameObject spell = Instantiate(prefab, pos, rot);
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
}
