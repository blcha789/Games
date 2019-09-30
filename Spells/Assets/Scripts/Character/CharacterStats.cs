using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public enum Teams {None, Red, Blue}

public class CharacterStats : NetworkBehaviour {

    [Header("Main Stats")]
    [SyncVar]
    public bool isDead = true;
    [SyncVar(hook = "OnChangeTeam")]
    public Teams Team;
    [SyncVar(hook = "OnHealthChanged")]
    public float health = 100;
    [SyncVar]
    public float mana = 100;

    public Animator anim;

    [Header("Buffs")]
    public spellEffect[] spellEffect;
    public spellEffectDmg[] spellEffectDmg;

    [Header("Prefabs")]
    public GameObject buffPrefab;

    Image healthBar;
    Image manaBar;
    Transform buffsParent;
    GameObject playerGUI;

    void OnDisable()
    {       
            GameLogic.UnRegisterPlayer(transform.name, Team.ToString());//ked sa disconecte hrac odregistruje hraca
    }

    void Start()
    {
        if (!isLocalPlayer)
            return;

        healthBar = GameObject.Find("GameLogic").GetComponent<SceneGameObjects>().healthBar.GetComponent<Image>();
        manaBar = GameObject.Find("GameLogic").GetComponent<SceneGameObjects>().manaBar.GetComponent<Image>();
        buffsParent = GameObject.Find("GameLogic").GetComponent<SceneGameObjects>().Buffs.transform;
        playerGUI = GameObject.Find("GameLogic").GetComponent<SceneGameObjects>().PlayerGUI;
    }

    void Update()
    {

        if (!isLocalPlayer)
            return;

        if (!isDead)
        {
            CheckEffects();
            CheckStats();
        }
    }

    public void Damage(float damageHit, int spellType, float spellTypeDamage, float spellTypeBonusDamage, float spellTypeDuration)
    {
        for (int i = 0; i < spellEffectDmg.Length; i++)
        {       
            if(i == spellType)
            {
                spellEffectDmg[i].effectIsOn = true;
                spellEffectDmg[i].effectStat = spellTypeDamage;
                spellEffectDmg[i].effectTime = spellTypeDuration;
            }
        }
        SpellTypeEffects(damageHit, spellType, spellTypeBonusDamage);
    }

    public void Effect(int effect, float spellEffectModifier, float spellEffectDuration)
    {
        for (int i = 0; i < spellEffect.Length; i++)
        {
            if (i == effect)
            {
                spellEffect[i].effectIsOn = true;
                spellEffect[i].effectStat = spellEffectModifier;
                spellEffect[i].effectTime = spellEffectDuration;
            }
        }
    }


    void SpellTypeEffects(float damageHit, int spellType, float effectBonusDmg)
    {
        if (spellEffectDmg[0].effectIsOn)//burn
        {
            if(spellType == 1)//water
            {
                spellEffectDmg[0].effectIsOn = false;//vypne burn ak sa dotkne vody
            }
        }

        if(spellEffectDmg[1].effectIsOn)//wet
        {
            if(spellType == 0)//fire
            {
                damageHit = 0;
                spellEffectDmg[0].effectIsOn = false;
            }
            else if(spellType == 2)//electric
            {
                damageHit += effectBonusDmg;
            }
        }

        CmdSetHealth(health - damageHit);
    }


    void CheckEffects()
    {
        for (int i = 0; i < spellEffect.Length; i++)
        {
            if (spellEffect[i].effectIsOn)
            {
                if (spellEffect[i].effectUI == null)
                {
                    GameObject buff = Instantiate(buffPrefab, buffsParent);
                    buff.name = i.ToString();
                    buff.GetComponent<Image>().sprite = spellEffect[i].icon;

                    spellEffect[i].effectUI = buff;
                }

                if (spellEffect[i].effectTime > 0)
                {
                    spellEffect[i].effectTime -= Time.deltaTime;
                    spellEffect[i].effectUI.GetComponentInChildren<Text>().text = spellEffect[i].effectTime.ToString("F1");
                }
                else
                {
                    spellEffect[i].effectIsOn = false;

                    Destroy(spellEffect[i].effectUI);
                }
            }
            else
                Destroy(spellEffect[i].effectUI);
        }

        for (int i = 0; i < spellEffectDmg.Length; i++)
        {
            if (spellEffectDmg[i].effectIsOn)
            {
                if (spellEffectDmg[i].effectUI == null)
                {
                    GameObject buff = Instantiate(buffPrefab, buffsParent);
                    buff.name = i.ToString();
                    buff.GetComponent<Image>().sprite = spellEffectDmg[i].icon;

                    spellEffectDmg[i].effectUI = buff;
                }

                if (spellEffectDmg[i].effectTime > 0)
                {
                    spellEffectDmg[i].effectTime -= Time.deltaTime;
                    CmdSetHealth(health - spellEffectDmg[i].effectStat * Time.deltaTime);
                    spellEffectDmg[i].effectUI.GetComponentInChildren<Text>().text = spellEffectDmg[i].effectTime.ToString("F1");
                }
                else
                {
                    spellEffectDmg[i].effectIsOn = false;

                    Destroy(spellEffectDmg[i].effectUI);
                }
            }
            else
                Destroy(spellEffectDmg[i].effectUI);
        }
    }

    void CheckStats()
    {
        if (health > 100)
            health = 100;
        if (mana > 100)
            mana = 100;

        health += 0.1f * Time.deltaTime;
        mana += 0.4f * Time.deltaTime;

        healthBar.fillAmount = health / 100;
        manaBar.fillAmount = mana / 100;

        if (health <= 0)
        {
            anim.SetTrigger("isDead");
            CmdUpdateAnimator("isDead");
            playerGUI.SetActive(false);
            isDead = true;
        }
    }


    private void OnChangeTeam(Teams team)
    {
        Team = team;
    }

    [Command]
    void CmdUpdateAnimator(string animation)
    {
        RpcUpdateAnimator(animation);
    }

    [ClientRpc]
    void RpcUpdateAnimator(string animation)
    {
        if (isLocalPlayer)
            return;

        anim.SetTrigger(animation);
    }

    [Command]
    void CmdSetHealth(float _health)
    {
        health = _health;
    }

    void OnHealthChanged(float _health)
    {
        health = _health;
    }
}


[System.Serializable]
public class spellEffect //root, stun, silence, speed
{
    public string name;
    public Sprite icon;
    public bool effectIsOn;
    public float effectStat;
    public float effectTime;
    public GameObject effectUI;
}

[System.Serializable]
public class spellEffectDmg //burn, wet, poison, ...
{
    public string name;
    public Sprite icon;
    public bool effectIsOn;
    public float effectStat;
    public float effectTime;
    public GameObject effectUI;
}

