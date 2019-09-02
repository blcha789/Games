using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demolish : MonoBehaviour
{
    public Transform buildingsParent;//where are building placed
    public GameObject deleteBuildingsPanel; //panel that will show when we want to delete buildings
    private GameLogic gameLogic;

    public Texture2D selectionHighLight = null;
    public static Rect selection = new Rect(0, 0, 0, 0);
    private Vector3 startClick = -Vector3.one;

    private void Start()
    {
        gameLogic = GameObject.FindGameObjectWithTag("Hierarchy/GameLogic").GetComponent<GameLogic>();
    }

    private void Update()
    {
        if (gameLogic.constructionOperation == ConstructionOperation.Demolish)//if we are in demolish mode
        {
            if(Application.platform == RuntimePlatform.WindowsEditor)//if we are in editor then use mouse clicks
            MouseClicks();
            else if(Application.platform == RuntimePlatform.Android)//if we are on android the use touch
            {
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);

                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                            TouchPhaseBegan();
                            break;

                        case TouchPhase.Moved:
                            TouchPhaseMoved();
                            break;

                        case TouchPhase.Ended:
                            TouchPhaseEnded();
                            break;
                    }
                }
            }
        }
    }

    private void MouseClicks()
    {
        if (Input.GetMouseButtonDown(0))//on left mouse button down get position of mouse click
            startClick = Input.mousePosition;
        else if (Input.GetMouseButtonUp(0))//on left mouse button up we chceck if picked building is not start building then we open panel that will show if we want delete selected buildings
        {
            startClick = -Vector3.one;

            bool open = false;
            foreach (Transform item in buildingsParent)
            {
                if (item.GetComponent<BuildingInfo>().isSelected && !item.GetComponent<BuildingInfo>().startBuilding)
                    open = true;
            }
            if (open)
            {
                deleteBuildingsPanel.SetActive(true);
                gameLogic.constructionOperation = ConstructionOperation.None;
            }
        }

        if (Input.GetMouseButton(0))//then move mouse to create rectanle that will show which building will be selected
        {
            selection = new Rect(startClick.x, InvertMouseY(startClick.y), Input.mousePosition.x - startClick.x, InvertMouseY(Input.mousePosition.y) - InvertMouseY(startClick.y));

            if (selection.width < 0)
            {
                selection.x += selection.width;
                selection.width = -selection.width;
            }
            if (selection.height < 0)
            {
                selection.y += selection.height;
                selection.height = -selection.height;
            }
        }
    }

    private void TouchPhaseBegan()
    {
        startClick = Input.mousePosition;
    }

    private void TouchPhaseMoved()
    {
        selection = new Rect(startClick.x, InvertMouseY(startClick.y), Input.GetTouch(0).position.x - startClick.x, InvertMouseY(Input.GetTouch(0).position.y) - InvertMouseY(startClick.y));

        if (selection.width < 0)
        {
            selection.x += selection.width;
            selection.width = -selection.width;
        }
        if (selection.height < 0)
        {
            selection.y += selection.height;
            selection.height = -selection.height;
        }
    }

    private void TouchPhaseEnded()
    {
        startClick = -Vector3.one;

        bool open = false;
        foreach (Transform item in buildingsParent)
        {
            if (item.GetComponent<BuildingInfo>().isSelected && !item.GetComponent<BuildingInfo>().startBuilding)
                open = true;
        }
        if (open)
        {
            deleteBuildingsPanel.SetActive(true);
            gameLogic.constructionOperation = ConstructionOperation.None;
        }
    }

    //creating rectangle
    private void OnGUI()
    {
        if (startClick != -Vector3.one)
        {
            GUI.color = new Color(1, 1, 1, 0.5f);
            GUI.DrawTexture(selection, selectionHighLight);
        }
    }

    public static float InvertMouseY(float y)
    {
        return Screen.height - y;
    }
}
