using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraMovement : MonoBehaviour
{
    public float smoothSpeed;
    public Vector3 offset;

    public Vector3 minPos;
    public Vector3 maxPos;

    private GameLogic gameLogic;
    private Transform player;
    private Camera cam;

    private Vector3 startPos, currentPos, camPos;

    private void Start()
    {
        gameLogic = GameObject.FindGameObjectWithTag("GameLogic").GetComponent<GameLogic>();
        player = GameObject.FindGameObjectWithTag("Character").transform;
        cam = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        if (gameLogic.statusMode == StatusMode.Play)//follow player
        {
           Vector3 desiredPosition = player.position + offset;
           Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
           transform.position = smoothedPosition;
        }
        else//move by touch
        {
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                MouseClicks();
            }
            else
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
                    }
                }
            }
        }
        ClampCamera();
    }

    private void MouseClicks()
    {
        if (!EventSystem.current.IsPointerOverGameObject())//if there is not any UI between mouse and building 
        {
            if (Input.GetMouseButtonDown(0))
            {
                startPos = Input.mousePosition;
                camPos = transform.position;
            }

            if (Input.GetMouseButton(0))
            {
                currentPos = Input.mousePosition;
                MoveCamera();
            }
        }
    }
    
    private void TouchPhaseBegan()
    {
        if (!IsPointerOverGameObject(Input.GetTouch(0).fingerId))
        {
            startPos = Input.GetTouch(0).position;
            camPos = transform.position;
        }
    }

    private void TouchPhaseMoved()
    {
        if (!IsPointerOverGameObject(Input.GetTouch(0).fingerId))
        {
            currentPos = Input.GetTouch(0).position;
            MoveCamera();
        }
    }

    private void MoveCamera()
    {
        Vector3 position = cam.ScreenToWorldPoint(new Vector3(currentPos.x, currentPos.y, cam.nearClipPlane)) - cam.ScreenToWorldPoint(new Vector3(startPos.x, startPos.y, cam.nearClipPlane));
        transform.position = camPos + (new Vector3(position.x, 0, position.z) * -40f);
    }

    private void ClampCamera()
    {
        Vector3 pos = transform.position; 
        pos.x = Mathf.Clamp(pos.x, minPos.x, maxPos.x);
        pos.y = 6.5f;
        pos.z = Mathf.Clamp(pos.z, minPos.z, maxPos.z);

        transform.position = pos;
    }

    private bool IsPointerOverGameObject(int fingerId)
    {
        EventSystem eventSystem = EventSystem.current;
        return (eventSystem.IsPointerOverGameObject(fingerId)
            && eventSystem.currentSelectedGameObject != null);
    }
}
