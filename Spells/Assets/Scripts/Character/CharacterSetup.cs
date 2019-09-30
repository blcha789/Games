using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CharacterSetup : NetworkBehaviour
{
    public string netID;

    public GameObject playerGUI;
    public Animator anim;

    CharacterStats characterStats;

    void Start()
    {
        playerGUI = GameObject.Find("GameLogic").GetComponent<SceneGameObjects>().PlayerGUI;

        characterStats = GetComponent<CharacterStats>();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        netID = GetComponent<NetworkIdentity>().netId.ToString();// sem treba zdata steam meno 
        GameLogic.RegisterPlayer(netID, GetComponent<CharacterStats>());//zaregistruje hraca
    }


    [ClientRpc]
    public void RpcCreate(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        //GetComponent<CharacterRotation>().originalRotation = rotation;

        characterStats.health = 100;
        characterStats.mana = 100;
        characterStats.isDead = false;

        if (!hasAuthority)
            return;

        CmdCreateCharacter();
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Death"))
            CmdUpdateAnimator();

    }

    [Command]
    void CmdUpdateAnimator()
    {
        RpcUpdateAnimator("isAlive");
    }

    [Command]
    void CmdCreateCharacter()
    {
        RpcCreateCharacter();
    }

    [ClientRpc]
    void RpcCreateCharacter()
    {
        if (!isLocalPlayer)
            return;

        playerGUI.SetActive(true);//zapne GUI
        transform.GetChild(0).GetChild(0).gameObject.SetActive(true);//zapne cameru

        CharacterStats[] players = GameLogic.GetAllPlayers();

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].Team == characterStats.Team)
            {
                if (players[i].name != transform.name)
                {
                    players[i].transform.GetChild(4).gameObject.SetActive(true);
                    players[i].transform.GetChild(4).GetChild(0).GetComponent<Text>().text = players[i].name;
                    players[i].transform.GetChild(4).GetComponent<NamePlate>().FollowObject = transform.GetChild(0).GetChild(0).gameObject;
                }
                else
                {
                    players[i].transform.GetChild(4).gameObject.SetActive(false);
                }
            }
            else
            {
                players[i].transform.GetChild(4).gameObject.SetActive(false);
            }
        }
    }

    [ClientRpc]
    void RpcUpdateAnimator(string animation)
    {
        if (isLocalPlayer)
            return;

        anim.SetTrigger(animation);
    }
}
