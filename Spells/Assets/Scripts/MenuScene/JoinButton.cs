using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;

public class JoinButton : MonoBehaviour {

    public GameObject waitToLoadPanel;
    public NetworkManager networkManager;
    public ulong matchID;

void Start()
    {
        networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        waitToLoadPanel = GameObject.Find("GameLogic").GetComponent<CreateJoinRoom>().waitToLoadPanel;
    }

    public void JoinRoom()
    {
        waitToLoadPanel.SetActive(true);
        StartCoroutine(StopMatch());

        NetworkID match = (NetworkID)matchID;
        networkManager.matchMaker.JoinMatch(match, "", "", "", 0, 0, networkManager.OnMatchJoined);
    }

    IEnumerator StopMatch()
    {
        yield return new WaitForSeconds(5);
        waitToLoadPanel.transform.GetChild(0).gameObject.SetActive(true);
    }
}
