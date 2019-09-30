using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class CharacterShooting : NetworkBehaviour
{
    public GameObject currentSpell;
    public Transform shotPos;
    public Camera cam;

    int damage;
    public float fireRate;

    float timer;
    float timerLaser;
    float cancelTime = 1.0f;
    float castingLaserMana = 0;

    bool isCasting;
    bool isCastingLaser;

    public Animator anim;

    SpellDatabase spellDatabase;

    Image loadingBar;
    GameObject loadingBarBackground;

    void Start()
    {
        spellDatabase = GameObject.Find("Database").GetComponent<SpellDatabase>();

        if (!isLocalPlayer)
            return;

        loadingBar = GameObject.Find("GameLogic").GetComponent<SceneGameObjects>().loadingBar;
        loadingBarBackground = GameObject.Find("GameLogic").GetComponent<SceneGameObjects>().LoadingBarBackground;
    }

    void Update()
    {
        if (!isLocalPlayer)
            return;

        if (!GetComponent<CharacterStats>().isDead)
        {
            if (GetComponent<CharactersActions>().isCursorLocked)
            {
                ShotPos();
                FireSpell();
            }
        }
        else
        {
            isCasting = false;
            isCastingLaser = false;
            CmdDestroySpell();
        }

        FireBall();
        FireLaser();
        CancelSpell();
        UpdateLoadingBar();
    }

    void ShotPos()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000))
            shotPos.transform.LookAt(hit.point);
        else
            shotPos.transform.LookAt(ray.GetPoint(50));
    }


    void FireSpell()
    {
        if (!GetComponent<CharacterStats>().spellEffect[0].effectIsOn && !GetComponent<CharacterStats>().spellEffect[2].effectIsOn)
        {
            if (currentSpell == null)
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    if (GetComponent<CharacterStats>().mana >= spellDatabase.menuSpells[PlayerPrefs.GetInt("Mouse0MenuCount")].spells[PlayerPrefs.GetInt("Mouse0Spell")].mana)
                    {
                        if (spellDatabase.menuSpells[PlayerPrefs.GetInt("Mouse0MenuCount")].spells[PlayerPrefs.GetInt("Mouse0Spell")].spellCategory == SpellCategory.Laser)
                            CreateLaser(0);
                        else
                            CreateBall(0);
                    }
                }
                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    if (GetComponent<CharacterStats>().mana >= spellDatabase.menuSpells[PlayerPrefs.GetInt("Mouse1MenuCount")].spells[PlayerPrefs.GetInt("Mouse1Spell")].mana)
                    {
                        if (spellDatabase.menuSpells[PlayerPrefs.GetInt("Mouse1MenuCount")].spells[PlayerPrefs.GetInt("Mouse1Spell")].spellCategory == SpellCategory.Laser)
                            CreateLaser(1);
                        else
                            CreateBall(1);
                    }
                }
            }
            else
            {
                currentSpell.transform.position = shotPos.position;
                currentSpell.transform.rotation = shotPos.rotation;
            }
        }
        else
        {
            if (currentSpell != null)
            {
                isCasting = false;
                isCastingLaser = false;
                cancelTime = 1.0f;
                anim.SetTrigger("CancelSpell");
                CmdUpdateAnimatorTrigger("CancelSpell");

                Destroy(currentSpell);
            }
        }
    }

    void CancelSpell()
    {
        if (isCasting || isCastingLaser)
        {
            if (cancelTime <= 0)
            {
                if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Mouse1))
                {
                    isCasting = false;
                    isCastingLaser = false;
                    cancelTime = 1.0f;
                    anim.SetTrigger("CancelSpell");
                    CmdUpdateAnimatorTrigger("CancelSpell");

                    CmdDestroySpell();
                }
            }
            else
            {
                cancelTime -= Time.deltaTime;
            }
        }
    }


    void CreateBall(int mouseClick)
    {
        if (GetComponent<CharacterStats>().spellEffect[4].effectIsOn)//boost is on
        {
            if (mouseClick == 0)
            {
                CmdCreateSpell(PlayerPrefs.GetInt("Mouse0MenuCount"), PlayerPrefs.GetInt("Mouse0Spell"));
                fireRate = spellDatabase.menuSpells[PlayerPrefs.GetInt("Mouse0MenuCount")].spells[PlayerPrefs.GetInt("Mouse0Spell")].cooldown / 2;
            }
            else
            {
                CmdCreateSpell(PlayerPrefs.GetInt("Mouse1MenuCount"), PlayerPrefs.GetInt("Mouse1Spell"));
                fireRate = spellDatabase.menuSpells[PlayerPrefs.GetInt("Mouse1MenuCount")].spells[PlayerPrefs.GetInt("Mouse1Spell")].cooldown / 2;
            }
        }
        else
        {
            if (mouseClick == 0)
            {
                CmdCreateSpell(PlayerPrefs.GetInt("Mouse0MenuCount"), PlayerPrefs.GetInt("Mouse0Spell"));
                GetComponent<CharacterStats>().mana -= spellDatabase.menuSpells[PlayerPrefs.GetInt("Mouse0MenuCount")].spells[PlayerPrefs.GetInt("Mouse0Spell")].mana;
                fireRate = spellDatabase.menuSpells[PlayerPrefs.GetInt("Mouse0MenuCount")].spells[PlayerPrefs.GetInt("Mouse0Spell")].cooldown;
            }
            else
            {
                CmdCreateSpell(PlayerPrefs.GetInt("Mouse1MenuCount"), PlayerPrefs.GetInt("Mouse1Spell"));
                GetComponent<CharacterStats>().mana -= spellDatabase.menuSpells[PlayerPrefs.GetInt("Mouse1MenuCount")].spells[PlayerPrefs.GetInt("Mouse1Spell")].mana;
                fireRate = spellDatabase.menuSpells[PlayerPrefs.GetInt("Mouse1MenuCount")].spells[PlayerPrefs.GetInt("Mouse1Spell")].cooldown;
            }
        }

        UpdateAnimator();
        isCasting = true;
    }

    void CreateLaser(int mouseClick)
    {
        if (GetComponent<CharacterStats>().spellEffect[4].effectIsOn)//boost is on
        {
            if (mouseClick == 0)
            {
                CmdCreateSpell(PlayerPrefs.GetInt("Mouse0MenuCount"), PlayerPrefs.GetInt("Mouse0Spell"));
                fireRate = spellDatabase.menuSpells[PlayerPrefs.GetInt("Mouse0MenuCount")].spells[PlayerPrefs.GetInt("Mouse0Spell")].cooldown * 2;
                castingLaserMana = 0;
            }
            else
            {
                CmdCreateSpell(PlayerPrefs.GetInt("Mouse1MenuCount"), PlayerPrefs.GetInt("Mouse1Spell"));
                fireRate = spellDatabase.menuSpells[PlayerPrefs.GetInt("Mouse1MenuCount")].spells[PlayerPrefs.GetInt("Mouse1Spell")].cooldown * 2;
                castingLaserMana = 0;
            }
        }
        else
        {
            if(mouseClick == 0)
            {
                CmdCreateSpell(PlayerPrefs.GetInt("Mouse0MenuCount"), PlayerPrefs.GetInt("Mouse0Spell"));
                fireRate = spellDatabase.menuSpells[PlayerPrefs.GetInt("Mouse0MenuCount")].spells[PlayerPrefs.GetInt("Mouse0Spell")].cooldown;
                castingLaserMana = spellDatabase.menuSpells[PlayerPrefs.GetInt("Mouse0MenuCount")].spells[PlayerPrefs.GetInt("Mouse0Spell")].mana;
            }
            else
            {
                CmdCreateSpell(PlayerPrefs.GetInt("Mouse1MenuCount"), PlayerPrefs.GetInt("Mouse1Spell"));
                fireRate = spellDatabase.menuSpells[PlayerPrefs.GetInt("Mouse1MenuCount")].spells[PlayerPrefs.GetInt("Mouse1Spell")].cooldown;
                castingLaserMana = spellDatabase.menuSpells[PlayerPrefs.GetInt("Mouse1MenuCount")].spells[PlayerPrefs.GetInt("Mouse1Spell")].mana;
            }
        }

        isCastingLaser = true;
    }


    void FireBall()
    {
        if (isCasting)
        {
            if (timer <= fireRate)
                timer += Time.deltaTime;
            else
            {
                CmdFireBall();
                currentSpell = null;

                isCasting = false;
                cancelTime = 1.0f;
                anim.SetTrigger("CancelSpell");
                CmdUpdateAnimatorTrigger("CancelSpell");
            }
        }
        else
            timer = 0;
    }

    void FireLaser()
    {
        if (isCastingLaser)
        {
            if (GetComponent<CharacterStats>().mana >= 0)
            {
                if (timerLaser <= fireRate)
                {
                    timerLaser += Time.deltaTime;
                    GetComponent<CharacterStats>().mana -= castingLaserMana * Time.deltaTime;
                }
                else
                {
                    CmdDestroySpell();
                    currentSpell = null;
                    isCastingLaser = false;
                    cancelTime = 1.0f;
                    anim.SetTrigger("CancelSpell");
                    CmdUpdateAnimatorTrigger("CancelSpell");
                }
            }
            else
            {
                CmdDestroySpell();
                currentSpell = null;
                isCastingLaser = false;
                cancelTime = 1.0f;
                anim.SetTrigger("CancelSpell");
            }
        }
        else
            timerLaser = 0;
    }


    [Command]
    void CmdCreateSpell(int MouseMenuCount, int MouseSpell)
    {
        GameObject spell = Instantiate(spellDatabase.menuSpells[MouseMenuCount].spells[MouseSpell].prefab, shotPos.transform.position, shotPos.transform.rotation);
        NetworkServer.SpawnWithClientAuthority(spell, this.gameObject);
        RpcCreateSpell(spell);
    }

    [Command]
    void CmdFireBall()
    {
        RpcAddForce(currentSpell);
    }

    [ClientRpc]
    void RpcAddForce(GameObject spell)
    {
        spell.GetComponent<Rigidbody>().AddForce(shotPos.forward * 1000);
    }

    [Command]
    void CmdDestroySpell()
    {
        NetworkServer.Destroy(currentSpell);
    }

    [ClientRpc]
    void RpcCreateSpell(GameObject spell)
    {
        currentSpell = spell;
    }


    void UpdateLoadingBar()
    {
        float time = 0;

        if (isCasting)
            time = timer;
        else if (isCastingLaser)
            time = timerLaser;
        else
            time = 0;

        if (time <= 0)
            loadingBarBackground.SetActive(false);
        else
        {
            loadingBarBackground.SetActive(true);
            loadingBar.fillAmount = time / fireRate;
        }
    }

    void UpdateAnimator()
    {
        float animSpeed = 3.25f / fireRate;

        anim.SetFloat("Spell 2 Speed", animSpeed);//speed animacie
        anim.SetTrigger("Spell 2");

        //spell 2 anim speed 1  , fire rate 3.25f

        CmdUpdateAnimatorTrigger("Spell 2");
        CmdUpdateAnimatorSpeed("Spell 2 Speed", animSpeed);
    }

    [Command]
    void CmdUpdateAnimatorTrigger(string animation)
    {
        RpcUpdateAnimatorTrigger(animation);
    }

    [Command]
    void CmdUpdateAnimatorSpeed(string animation, float animSpeed)
    {
        RpcUpdateAnimatorSpeed(animation, animSpeed);
    }

    [ClientRpc]
    void RpcUpdateAnimatorTrigger(string animation)
    {
        if (isLocalPlayer)
            return;

        anim.SetTrigger(animation);
    }

    [ClientRpc]
    void RpcUpdateAnimatorSpeed(string animation, float animSpeed)
    {
        if (isLocalPlayer)
            return;

        anim.SetFloat(animation, animSpeed);
    }
}
