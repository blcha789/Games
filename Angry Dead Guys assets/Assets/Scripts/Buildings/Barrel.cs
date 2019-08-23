using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : MonoBehaviour
{
    public float damage = 2f;
    public float fireTime = 10f;
    public GameObject barrel;
    public GameObject fireParticle;
    public Collider barrelCollider1;
    public Collider barrelCollider2;

    private bool isBurning = false;

    private GameLogic gameLogic;

    private void Start()
    {
        gameLogic = GameObject.FindGameObjectWithTag("GameLogic").GetComponent<GameLogic>();
    }

    private void Update()
    {
        if (gameLogic.statusMode == StatusMode.Build)
            if (isBurning)
                Destroy(transform.parent.gameObject);
    }

    public void SetFire()
    {
        StartCoroutine(SetFireEnumerator());
    }

    private IEnumerator SetFireEnumerator()
    {
        isBurning = true;
        Destroy(barrel);
        barrelCollider1.enabled = false;
        barrelCollider2.enabled = false;

        fireParticle.SetActive(true);
        
        yield return new WaitForSeconds(fireTime);
        Destroy(transform.parent.gameObject);
    }

    private void OnTriggerEnter(Collider col)
    {
        if (isBurning)
        {
            if (col.CompareTag("Enemy"))
            {
                col.GetComponent<EnemyStats>().Burning(damage);
            }
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (isBurning)
        {
            if (col.CompareTag("Enemy"))
            {
                col.GetComponent<EnemyStats>().Burning(damage);
            }
        }
    }
}
