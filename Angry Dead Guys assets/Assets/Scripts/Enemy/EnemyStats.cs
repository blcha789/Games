using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{

    [Header("Main")]
    public float health;
    public bool isDead = false;
    public AudioSource zombieSound;
    public GameObject fire;

    [Header("ItemsDrops")]
    public ItemDrop[] itemsDrop;

    [Header("Zombie Sounds")]
    public List<AudioClip> sound = new List<AudioClip>();

    private float currentHealth;
    private bool isBurning = false;
    private int burningTime = 3;

    void Start()
    {
        currentHealth = health;
        zombieSound.clip = sound[Random.Range(0, sound.Count)];
        zombieSound.Play();
    }

    public void TakeDamage(float damage)
    {
        if (isDead)
            return;

        currentHealth -= damage;

        if (currentHealth <= 0)
            Death();
    }

    public void Burning(float damage)
    {
        StartCoroutine(BurningDps(damage));
    }

    private IEnumerator BurningDps(float damage)
    {
        fire.SetActive(true);
        int counter = burningTime;

        while (counter > 0)
        {
            TakeDamage(damage);
            Debug.Log("burning");

            yield return new WaitForSeconds(1f);
            counter--;
        }
        fire.SetActive(false);
    }

    private void Death()
    {
        isDead = true;

        int number = Random.Range(0, 100);

        for (int i = 0; i < itemsDrop.Length; i++)
        {
            if( number >= itemsDrop[i].minChance && number < itemsDrop[i].maxChance)
                Instantiate(itemsDrop[i].item, new Vector3(transform.position.x, 0, transform.position.z), Quaternion.identity);
        }

        Destroy(gameObject);

        //animacia smrti , zvuky smrti
    }
}

[System.Serializable]
public class ItemDrop
{
    public string name;
    public GameObject item;
    public int minChance;
    public int maxChance;
}
