using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Linq;

public class GameLogic : NetworkBehaviour
{

    [SyncVar]
    public int state;//1- caka sa na hracov, 2 - hra sa

    public CharacterStats[] players;
    public CharacterStats[] currentPlayersRedTeam;
    public CharacterStats[] currentPlayersBlueTeam;

    [Header("Times")]
    [SyncVar]
    public float waitTime = 5;
    [SyncVar]
    public float gameTime = 300;

    [SyncVar]
    float currentTime;

    [Header("Dead Players Count")]
    [SyncVar]
    public int countDeadPlayersRedTeam = 0;
    [SyncVar]
    public int countDeadPlayersBlueTeam = 0;

    [Header("Respawn Positions")]
    public GameObject[] RespawnPositionsRedTeam;
    public GameObject[] RespawnPositionsBlueTeam;

    [Header("UI")]
    public Text TimerText;
    public Text RedTeamPointsText;
    public Text BlueTeamPointsText;

    [SyncVar]
    float minutes;
    [SyncVar]
    float seconds;

    [SyncVar]
    float redTeamPoints = 0;
    [SyncVar]
    float blueTeamPoints = 0;

    public override void OnStartServer()
    {
        state = 1;
        currentTime = waitTime;

        RespawnPositionsRedTeam = GameObject.FindGameObjectsWithTag("RespawnPosRedTeam");
        RespawnPositionsBlueTeam = GameObject.FindGameObjectsWithTag("RespawnPosBlueTeam");
    }

    void Update()
    {
        CountDownTime();
        SyncStats();

        if (!isServer)
            return;

        GameStates();
    }

    void GameStates()
    {
        currentPlayersRedTeam = GetAllPlayersRedTeam();
        currentPlayersBlueTeam = GetAllPlayersBlueTeam();

        if (state == 1)//caka sa na hracov 
        {
            if(currentPlayersRedTeam.Length >= 1 && currentPlayersBlueTeam.Length >= 0)
            {
                if (currentTime > 0)
                    currentTime -= Time.deltaTime;
                else
                {
                    currentTime = gameTime;
                    state = 2;
                }
            }
            else
            {
                currentTime = waitTime;
            }
        }
        else if(state == 2)//spawn hracov
        {
            int i = 0;
            foreach (CharacterStats player in currentPlayersRedTeam)
            {
                i += 1;
                Spawn(player.GetComponent<NetworkIdentity>(), i, "Red");
            }

            i = 0;
            foreach (CharacterStats player in currentPlayersBlueTeam)
            {
                i += 1;
                 Spawn(player.GetComponent<NetworkIdentity>(), i , "Blue");
            }

            state = 30;
        }
        else if(state == 3)//hra sa
        {
            countDeadPlayersRedTeam = 0;
            countDeadPlayersBlueTeam = 0;

            foreach (CharacterStats player in currentPlayersRedTeam)
            {
                if (player.GetComponent<CharacterStats>().isDead)
                {
                    countDeadPlayersRedTeam += 1;
                }
            }

            foreach (CharacterStats player in currentPlayersBlueTeam)
            {
                if (player.GetComponent<CharacterStats>().isDead)
                {
                    countDeadPlayersBlueTeam += 1;
                }
            }

            if(countDeadPlayersRedTeam == currentPlayersRedTeam.Length)
            {
                redTeamPoints =+ 1;
                state = 4;
            }

            if (countDeadPlayersBlueTeam == currentPlayersBlueTeam.Length)
            {
                blueTeamPoints = +1;
                state = 4;
            }

            if (currentTime > 0)
                currentTime -= Time.deltaTime;
            else
            {
                currentTime = waitTime;
                state = 4;
            }
        }
        else if(state == 4)//wait to respawn
        {
            if (currentTime > 0)
                currentTime -= Time.deltaTime;
            else
            {
                currentTime = waitTime;
                state = 1;
            }
        }
    }

    void CountDownTime()
    {
        minutes = Mathf.Floor(currentTime / 60);
        seconds = Mathf.Floor(currentTime - 60 * minutes);

        if (seconds >= 10)
            TimerText.text = minutes.ToString() + ":" + seconds.ToString();
        else
            TimerText.text = minutes.ToString() + ":0" + seconds.ToString();
    }

    void SyncStats()
    {
        players = GetAllPlayers();
        RedTeamPointsText.text = redTeamPoints.ToString();
        BlueTeamPointsText.text = blueTeamPoints.ToString();
    }

    void Spawn(NetworkIdentity netID, int i, string team)
    {
        if (team == "Red")
            SpawnServerRedTeam(netID, i);
        else
            SpawnServerBlueTeam(netID, i);
    }

    [Server]
    void SpawnServerRedTeam(NetworkIdentity netID, int i)
    {
        Vector3 position = RespawnPositionsRedTeam[i].transform.position;
        Quaternion rotation = RespawnPositionsRedTeam[i].transform.localRotation;
        netID.GetComponent<CharacterSetup>().RpcCreate(position, rotation);
    }

    [Server]
    void SpawnServerBlueTeam(NetworkIdentity netID, int i)
    {
        Vector3 position = RespawnPositionsBlueTeam[i].transform.position;
        Quaternion rotation = RespawnPositionsRedTeam[i].transform.localRotation;
        netID.GetComponent<CharacterSetup>().RpcCreate(position, rotation);
    }


    private static Dictionary<string, CharacterStats> playersAll = new Dictionary<string, CharacterStats>();
    private static Dictionary<string, CharacterStats> playersRedTeam = new Dictionary<string, CharacterStats>();
    private static Dictionary<string, CharacterStats> playersBlueTeam = new Dictionary<string, CharacterStats>();

    public static void RegisterPlayer(string netID, CharacterStats player)
    {
        string playerID = netID;
        playersAll.Add(playerID, player);
        player.transform.name = playerID;//medzi objektami v scene sa prepise meno objektu(nasej postavy)
    }

    public static void RegisterPlayerRedTeam(string netID, CharacterStats player)
    {
        string playerID = netID;
        playersRedTeam.Add(playerID, player);
    }

    public static void RegisterPlayerBlueTeam(string netID, CharacterStats player)
    {
        string playerID = netID;
        playersBlueTeam.Add(playerID, player);
    }


    public static void UnRegisterPlayer(string playerID, string team)
    {
        playersAll.Remove(playerID);

        if (team == "Red")
            playersRedTeam.Remove(playerID);
        else if(team == "Blue")
            playersBlueTeam.Remove(playerID);
    }


    public static CharacterStats GetPlayer(string playerID)
    {
        return playersAll[playerID];
    }

    public static CharacterStats GetPlayerRedTeam(string playerID)
    {
        return playersRedTeam[playerID];
    }

    public static CharacterStats GetPlayerBlueTeam(string playerID)
    {
        return playersBlueTeam[playerID];
    }


    public static CharacterStats[] GetAllPlayers()
    {
        return playersAll.Values.ToArray();
    }

    public static CharacterStats[] GetAllPlayersRedTeam()
    {
        return playersRedTeam.Values.ToArray();
    }

    public static CharacterStats[] GetAllPlayersBlueTeam()
    {
        return playersBlueTeam.Values.ToArray();
    }
}
