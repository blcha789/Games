using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class CharactersActions : NetworkBehaviour
{
    public GameObject scoreBoardPrefab;

    GameObject playerGUI;
    GameObject menuSpellParent;
    GameObject escMenu;
    GameObject chooseTeam;
    GameObject scoreBoard;
    GameObject cameraList;
    GameObject redTeamButton;
    GameObject blueTeamButton;

    public bool isActiveEscMenu = false;
    public bool isActiveChooseTeam = true;
    public bool isCursorLocked;

    float time = 5f;

    void Start()
    {
        if (!isLocalPlayer)
            return;

        playerGUI = GameObject.Find("GameLogic").GetComponent<SceneGameObjects>().PlayerGUI;
        menuSpellParent = GameObject.Find("GameLogic").GetComponent<SceneGameObjects>().MenuSpell;
        escMenu = GameObject.Find("GameLogic").GetComponent<SceneGameObjects>().EscMenu;
        chooseTeam = GameObject.Find("GameLogic").GetComponent<SceneGameObjects>().ChooseTeam;
        scoreBoard = GameObject.Find("GameLogic").GetComponent<SceneGameObjects>().ScoreBoard;

        cameraList = GameObject.Find("WaitCamera");
        redTeamButton = GameObject.Find("RedTeamButton");
        blueTeamButton = GameObject.Find("BlueTeamButton");

        redTeamButton.GetComponent<Button>().onClick.AddListener(() => SetRedTeam());
        blueTeamButton.GetComponent<Button>().onClick.AddListener(() => SetBlueTeam());
    }

    void Update()
    {
        if (!isLocalPlayer)
            return;

        ChangeSpell();
        EscMenu();
        ScoreBoard();
        ChangeCamera();
    }

    void ChangeSpell()
    {
        if (Input.GetKeyDown(KeyCode.E) && !isActiveEscMenu && !isActiveChooseTeam)
        {
            menuSpellParent.SetActive(true);
            menuSpellParent.transform.GetChild(1).gameObject.SetActive(false);
            playerGUI.SetActive(false);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            isCursorLocked = false;
        }
        else if (Input.GetKeyUp(KeyCode.E) && !isActiveEscMenu && !isActiveChooseTeam)
        {
            if (!GetComponent<CharacterStats>().isDead)
            {
                menuSpellParent.SetActive(false);
                playerGUI.SetActive(true);

                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                isCursorLocked = true;
            }
            else
            {
                menuSpellParent.SetActive(false);

                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                isCursorLocked = true;
            }
        }
    }

    void EscMenu()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isActiveEscMenu = !isActiveEscMenu;

            if (!isActiveChooseTeam)
            {
                if (!GetComponent<CharacterStats>().isDead)
                {
                    if (isActiveEscMenu)
                    {
                        escMenu.SetActive(true);
                        playerGUI.SetActive(false);
                        menuSpellParent.SetActive(false);
                        scoreBoard.SetActive(false);

                        Cursor.lockState = CursorLockMode.None;
                        Cursor.visible = true;
                        isCursorLocked = false;
                    }
                    else
                    {
                        escMenu.SetActive(false);
                        playerGUI.SetActive(true);

                        Cursor.lockState = CursorLockMode.Locked;
                        Cursor.visible = false;
                        isCursorLocked = true;
                    }
                }
                else
                {
                    if (isActiveEscMenu)
                    {
                        escMenu.SetActive(true);
                        playerGUI.SetActive(false);
                        menuSpellParent.SetActive(false);
                        scoreBoard.SetActive(false);

                        Cursor.lockState = CursorLockMode.None;
                        Cursor.visible = true;
                        isCursorLocked = false;
                    }
                    else
                    {
                        escMenu.SetActive(false);

                        Cursor.lockState = CursorLockMode.Locked;
                        Cursor.visible = false;
                        isCursorLocked = true;
                    }
                }
            }
            else
            {
                if (isActiveEscMenu)
                {
                    escMenu.SetActive(true);
                    chooseTeam.SetActive(false);
                }
                else
                {
                    escMenu.SetActive(false);
                    chooseTeam.SetActive(true);
                }
            }
        }
    }


    void ScoreBoard()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && !isActiveEscMenu && !isActiveChooseTeam)
        {
            CharacterStats[] players = GameLogic.GetAllPlayers();
            scoreBoard.SetActive(true);
            ClearScoreBoard();

            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].Team == Teams.Red)
                {                  
                    GameObject PlayerUI = Instantiate(scoreBoardPrefab, scoreBoard.transform.GetChild(0).GetChild(1));
                    PlayerUI.transform.GetChild(0).GetComponent<Text>().text = players[i].name;
                    if (players[i].isDead)
                        PlayerUI.transform.GetChild(1).GetComponent<Text>().text = "Dead";
                    else
                        PlayerUI.transform.GetChild(1).GetComponent<Text>().text = "Alive";
                }
                else if (players[i].Team == Teams.Blue)
                {
                    GameObject PlayerUI = Instantiate(scoreBoardPrefab, scoreBoard.transform.GetChild(0).GetChild(3));
                    PlayerUI.transform.GetChild(0).GetComponent<Text>().text = players[i].name;
                    if (players[i].isDead)
                        PlayerUI.transform.GetChild(1).GetComponent<Text>().text = "Dead";
                    else
                        PlayerUI.transform.GetChild(1).GetComponent<Text>().text = "Alive";
                }
                else 
                {
                    GameObject PlayerUI = Instantiate(scoreBoardPrefab, scoreBoard.transform.GetChild(0).GetChild(5));
                    PlayerUI.transform.GetChild(0).GetComponent<Text>().text = players[i].name;
                    PlayerUI.transform.GetChild(1).GetComponent<Text>().text = "Waiting";
                }
            }
        }

        if (Input.GetKeyUp(KeyCode.Tab) && !isActiveEscMenu && !isActiveChooseTeam)
        {
            scoreBoard.SetActive(false);
        }
    }

    void ClearScoreBoard()
    {
        foreach (Transform item in scoreBoard.transform.GetChild(0).GetChild(1).transform)
        {
            Destroy(item.gameObject);
        }

        foreach (Transform item in scoreBoard.transform.GetChild(0).GetChild(3).transform)
        {
            Destroy(item.gameObject);
        }

        foreach (Transform item in scoreBoard.transform.GetChild(0).GetChild(5).transform)
        {
            Destroy(item.gameObject);
        }
    }

    void ChangeCamera()
    {
        if (GetComponent<CharacterStats>().isDead)
        {
            if (time >= 0)
                time -= Time.deltaTime;
            else
            {
                transform.GetChild(0).GetChild(0).gameObject.SetActive(false);//vypne wizard cameru
                cameraList.SetActive(true);

                if (!isActiveEscMenu && !isActiveChooseTeam)
                {
                    if (Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        bool isDone = false;
                        for (int i = 0; i < cameraList.transform.childCount; i++)
                        {
                            if (!isDone)
                            {
                                if (cameraList.transform.GetChild(i).gameObject.activeSelf)//camera je zapnuta
                                {
                                    cameraList.transform.GetChild(i).gameObject.SetActive(false);

                                    if ((i - 1) < 0)
                                        cameraList.transform.GetChild(cameraList.transform.childCount - 1).gameObject.SetActive(true);
                                    else
                                        cameraList.transform.GetChild(i - 1).gameObject.SetActive(true);

                                    isDone = true;
                                }
                            }
                        }
                    }

                    if (Input.GetKeyDown(KeyCode.Mouse1))
                    {
                        bool isDone = false;

                        for (int i = 0; i < cameraList.transform.childCount; i++)
                        {
                            if (!isDone)
                            {
                                if (cameraList.transform.GetChild(i).gameObject.activeSelf)//camera je zapnuta
                                {
                                    cameraList.transform.GetChild(i).gameObject.SetActive(false);

                                    if ((i + 1) >= cameraList.transform.childCount)
                                        cameraList.transform.GetChild(0).gameObject.SetActive(true);
                                    else
                                        cameraList.transform.GetChild(i + 1).gameObject.SetActive(true);

                                    isDone = true;
                                }
                            }
                        }
                    }
                }
            }
        }
        else
        {
            time = 5f;
            cameraList.SetActive(false);
        }
    }


    void SetRedTeam()
    {
        CharacterStats[] players = GameLogic.GetAllPlayers();

        int redTeamCount = 0;
        int blueTeamCount = 0;

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].Team == Teams.Red)
            {
                redTeamCount++;
            }
            else if (players[i].Team == Teams.Blue)
            {
                blueTeamCount++;
            }
        }

        if (redTeamCount <= blueTeamCount)
        {            
            CmdSetTeam(Teams.Red);
            SetTeam();
        }
        else
        {
            StartCoroutine(OffTeamInfo());
        }
    }

    void SetBlueTeam()
    {
        CharacterStats[] players = GameLogic.GetAllPlayers();

        int redTeamCount = 0;
        int blueTeamCount = 0;

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].Team == Teams.Red)
            {
                redTeamCount++;
            }
            else if (players[i].Team == Teams.Blue)
            {
                blueTeamCount++;
            }
        }

        if (redTeamCount >= blueTeamCount)
        {
            CmdSetTeam(Teams.Blue);
            SetTeam();
        }
        else
        {
            StartCoroutine(OffTeamInfo());
        }
    }

    IEnumerator OffTeamInfo()
    {
        chooseTeam.transform.GetChild(0).gameObject.SetActive(true);
        yield return new WaitForSeconds(3);
        chooseTeam.transform.GetChild(0).gameObject.SetActive(false);
    }

    void SetTeam()
    {
        if (!isLocalPlayer)
            return;

        chooseTeam.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isCursorLocked = true;
        isActiveChooseTeam = false;
    }

    [Command]
    void CmdSetTeam(Teams team)
    {
        //RpcSetTeam(team);
        GetComponent<CharacterStats>().Team = team;

        if(team == Teams.Red)
            GameLogic.RegisterPlayerRedTeam(GetComponent<CharacterSetup>().netID, GetComponent<CharacterStats>());
        else if(team == Teams.Blue)
            GameLogic.RegisterPlayerBlueTeam(GetComponent<CharacterSetup>().netID, GetComponent<CharacterStats>());
    }

    [ClientRpc]
    void RpcSetTeam(Teams team)
    {
        if (!isLocalPlayer)
        {
            GetComponent<CharacterStats>().Team = team;
        }
    }
}
