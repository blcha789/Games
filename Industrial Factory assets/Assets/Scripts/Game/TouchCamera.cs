using UnityEngine;
using UnityEngine.EventSystems;

public class TouchCamera : MonoBehaviour {

    private Vector2 size;
    public LevelSetup LevelSetup;

    private Vector2?[] oldTouchPositions = {
		null,
		null
	};

    private Vector2 oldTouchVector;
    private float oldTouchDistance;
    private Transform cameraParent;
    private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
        cameraParent = transform.parent;

        size = new Vector2(LevelSetup.sizeX, LevelSetup.sizeZ);
    }

    private void Update()
    {
        //check how many touches we have (Zoom , move)
        if (Input.touchCount == 0)
        {
            oldTouchPositions[0] = null;
            oldTouchPositions[1] = null;
        }
        else if (Input.touchCount == 1)
        {
            if (!IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                if (oldTouchPositions[0] == null || oldTouchPositions[1] != null)
                {
                    oldTouchPositions[0] = Input.GetTouch(0).position;
                    oldTouchPositions[1] = null;
                }
                else
                {
                    Vector2 newTouchPosition = Input.GetTouch(0).position;

                    cameraParent.position += transform.TransformDirection((Vector3)((oldTouchPositions[0] - newTouchPosition) * Camera.main.orthographicSize / Camera.main.pixelHeight * 2f));

                    oldTouchPositions[0] = newTouchPosition;
                }
            }
        }
        else
        {
            if (oldTouchPositions[1] == null)
            {
                oldTouchPositions[0] = Input.GetTouch(0).position;
                oldTouchPositions[1] = Input.GetTouch(1).position;
                oldTouchVector = (Vector2)(oldTouchPositions[0] - oldTouchPositions[1]);
                oldTouchDistance = oldTouchVector.magnitude;
            }
            else
            {
                Vector2 screen = new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight);

                //get position of touches
                Vector2[] newTouchPositions = { Input.GetTouch(0).position, Input.GetTouch(1).position };
                Vector2 newTouchVector = newTouchPositions[0] - newTouchPositions[1];
                float newTouchDistance = newTouchVector.magnitude;

                cameraParent.position += transform.TransformDirection((Vector3)((oldTouchPositions[0] + oldTouchPositions[1] - screen) * Camera.main.orthographicSize / screen.y));

                //zoom
                cam.orthographicSize *= oldTouchDistance / newTouchDistance;
                cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, 1.6f, 16f);

                //pos
                cameraParent.position -= transform.TransformDirection((newTouchPositions[0] + newTouchPositions[1] - screen) * Camera.main.orthographicSize / screen.y);

                oldTouchPositions[0] = newTouchPositions[0];
                oldTouchPositions[1] = newTouchPositions[1];
                oldTouchVector = newTouchVector;
                oldTouchDistance = newTouchDistance;
            }
        }
        //cameraParent.localPosition = new Vector3(Mathf.Clamp(cameraParent.localPosition.x, -size.x / 2 * Screen.width / Screen.height, size.x / 10 * Screen.width / Screen.height), 18, Mathf.Clamp(cameraParent.localPosition.z, -size.y / 2 * Screen.width / Screen.height, size.y / 10 * Screen.width / Screen.height));//-      
    }

    private bool IsPointerOverGameObject(int fingerId)
    {
        EventSystem eventSystem = EventSystem.current;
        return (eventSystem.IsPointerOverGameObject(fingerId)
            && eventSystem.currentSelectedGameObject != null);
    }
}
