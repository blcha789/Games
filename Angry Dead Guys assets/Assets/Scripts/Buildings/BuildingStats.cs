using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingStats : MonoBehaviour
{
    public float health;
    public GameObject model;
    public AudioSource destroyBuildingSound;

    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0)
            StartCoroutine(waitToDestroy());
    }

    private IEnumerator waitToDestroy()
    {
        destroyBuildingSound.Play();
        model.SetActive(false);
        GetComponent<BoxCollider>().enabled = false;

        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}
