using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStats : MonoBehaviour
{
    [Header("Main")]
    public float startingHealth;
    public bool isDead = false;

    [Header("Other")]
    public Image HealthBar;
    public Image damageImage;
    public float flashSpeed = 5f;
    public Color flashColour = new Color(1f, 0f, 0f, 0.1f);
    public AudioSource dieSound;

    [Header("PickUpMoney")]
    public int minMoney = 5;
    public int maxMoney = 50;

    private float currentHealth;
    private bool damaged = false;

    private CharacterMovement characterMovement;
    private CharacterShooting characterShooting;
    private GameLogic gameLogic;
    private Animator anim;
    private Vector3 characterStartPos;

    private void Awake()
    {
        characterMovement = GetComponent<CharacterMovement>();
        characterShooting = GetComponent<CharacterShooting>();
        anim = transform.GetChild(1).GetComponent<Animator>();
        gameLogic = GameObject.FindGameObjectWithTag("GameLogic").GetComponent<GameLogic>();
    }

    private void Start()
    {
        ResetHealth();
        characterStartPos = transform.position;
    }

    void Update()
    {
        if (damaged)
            damageImage.GetComponent<Image>().color = flashColour;
        else
            damageImage.GetComponent<Image>().color = Color.Lerp(damageImage.GetComponent<Image>().color, Color.clear, flashSpeed * Time.deltaTime);

        damaged = false;
    }

    public void TakeDamage(float damage)
    {
        if (isDead)
            return;

        damaged = true;
        currentHealth -= damage;
        HealthBar.fillAmount = currentHealth / 100;

        if (currentHealth <= 0)
        {
            StartCoroutine(WaitToReset());
            gameLogic.GameOver();
        }
    }

    public void ResetHealth()
    {
        currentHealth = startingHealth;
        HealthBar.fillAmount = currentHealth / 100;
    }

    public void ResetCharacter()
    {
        ResetHealth();
        isDead = false;

        characterMovement.enabled = true;
        characterShooting.enabled = true;
    }

    public void StopCharacter()
    {
        characterMovement.enabled = false;
        characterShooting.enabled = false;

        transform.position = characterStartPos;
        anim.SetFloat("JoystickHorizontal", 0);
        anim.SetFloat("JoystickVertical", 0);
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("HealthBox"))
        {
            ResetHealth();
            Destroy(col.gameObject);
        }
        else if(col.CompareTag("Money"))
        {
            int money = Random.Range(minMoney, maxMoney);
            gameLogic.GetMoney(money);
            Destroy(col.gameObject);
        }
        else if(col.CompareTag("FuelCanister"))
        {
            gameLogic.GetFuel();
            Destroy(col.gameObject);
        }
    }

    private IEnumerator WaitToReset()
    {
        isDead = true;
        characterMovement.enabled = false;
        characterMovement.ResetMovement();
        characterShooting.enabled = false;
        characterShooting.ResetShooting();
        dieSound.Play();

        yield return new WaitForSeconds(1.5f);

        transform.position = new Vector3(0, 0, 0);
        anim.SetFloat("JoystickHorizontal", 0);
        anim.SetFloat("JoystickVertical", 0);
    }
}

