using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class PlaySceneButtons : MonoBehaviour {

    NetworkManager networkManager;

    void Start()
    {
        networkManager = NetworkManager.singleton;
    }

	public void ExitGame()
    {
        MatchInfo matchInfo = networkManager.matchInfo;
        networkManager.matchMaker.DropConnection(matchInfo.networkId, matchInfo.nodeId, 0,networkManager.OnDropConnection);
        networkManager.StopHost();
        Application.Quit();
    }

    public void ExitMatch()
    {
        MatchInfo matchInfo = networkManager.matchInfo;
        networkManager.matchMaker.DropConnection(matchInfo.networkId, matchInfo.nodeId, 0, networkManager.OnDropConnection);
        networkManager.StopHost();
    }
}
