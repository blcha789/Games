using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using UnityEngine.Networking.Match;

public class CreateJoinRoom : MonoBehaviour {

    [Header("Play")]
    public GameObject createPanel;
    public GameObject joinPanel;
    public GameObject waitToLoadPanel;
    public NetworkManager networkManager;

    [Header("Create")]
    public InputField roomName;
    public Dropdown map;

    [Header("Join")]
    public Transform roomList;
    public GameObject listItemPrefab;

    public void Create()
    {
        createPanel.SetActive(true);
        joinPanel.SetActive(false);
    }

    public void Join()
    {
        createPanel.SetActive(false);
        joinPanel.SetActive(true);

        Refresh();
    }

    public void CreateRoom()
    {
        waitToLoadPanel.SetActive(true);
        StartCoroutine(StopMatchShow());

        string matchName = roomName.text + "|" + map.value.ToString();
        networkManager.matchMaker.CreateMatch(matchName, 10, true, "", "", "", 0, 0, networkManager.OnMatchCreate);
    }

    public void RefreshList()
    {
        Refresh();
    }

    public void StopMatch()
    {
        networkManager.StopHost();
    }

    IEnumerator StopMatchShow()
    {
        yield return new WaitForSeconds(5);
        waitToLoadPanel.transform.GetChild(0).gameObject.SetActive(true);
    }

    void Refresh()
    {
        ClearList();

        networkManager.matchMaker.ListMatches(0, 20, "", true, 0, 0, OnMatchList);// "" - filter podla mena roomy
    }

    void OnMatchList(bool success, string ExtendedInfo, List<MatchInfoSnapshot> matchList)
    {
        if (!success || matchList == null)
        {
            //status.text = "Coulnd´t get room list.";
            return;
        }

        foreach (MatchInfoSnapshot match in matchList)
        {
            GameObject listItem = Instantiate(listItemPrefab, roomList);

            string[] split = match.name.Split('|');

            listItem.transform.GetChild(0).GetComponent<Text>().text = split[0];//name
            listItem.transform.GetChild(1).GetComponent<Text>().text = map.options[int.Parse(split[1])].text;//map
            listItem.transform.GetChild(2).GetComponent<Text>().text = match.currentSize + "/" + match.maxSize;//players
            listItem.transform.GetChild(3).GetComponent<JoinButton>().matchID = (ulong)match.networkId;
        }
    }

    void ClearList()
    {
        for (int i = 0; i < roomList.childCount; i++)
        {
            Destroy(roomList.GetChild(i).gameObject);
        }
    }
}
