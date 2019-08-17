using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingsUI : MonoBehaviour
{

    public Transform buildingCanvas;

    public GameObject buildingsItemsParent;
    public Transform[] buildingsItems;

    private Transform cam;
    private GameButtons gameButtons;

    private  bool rotatingCanvas, rotatingItems;

    private void Start()
    {

        //get scripts
        cam = GameObject.FindGameObjectWithTag("Hierarchy/Camera").transform;
        gameButtons = GameObject.FindGameObjectWithTag("Hierarchy/GameLogic").GetComponent<GameButtons>();

        buildingCanvas.GetComponent<Canvas>().worldCamera = Camera.main;

        //rotate canvas to camera
        ChangeCanvasItemsRotationToCamera();

        if (gameButtons.isInputsOutputsShowed && buildingsItemsParent != null)
            buildingsItemsParent.SetActive(true);
    }

    public void ChangeCanvasItemsRotation(Vector3 angles, float duration)
    {
        if (buildingCanvas != null)
            StartCoroutine(RotateCanvas(angles, duration));

        if (buildingsItems. Length > 0)
            StartCoroutine(RotateItems(angles, duration));
    }

    public void ChangeOnlyItemsRotation(Vector3 angles, float duration)
    {
        if (buildingsItems.Length > 0)
            StartCoroutine(RotateItems(angles, duration));
    }

    public void ChangeCanvasItemsRotationToCamera()
    {
        if (buildingCanvas != null && cam != null)
            buildingCanvas.rotation = Quaternion.Euler(35, cam.GetChild(0).rotation.eulerAngles.y, 0);

        if (buildingsItems.Length > 0 && cam != null)
        {
            for (int i = 0; i < buildingsItems.Length; i++)
            {
                buildingsItems[i].rotation = Quaternion.Euler(35, cam.GetChild(0).rotation.eulerAngles.y, 0);
            }
        }
    }

    private IEnumerator RotateCanvas(Vector3 angles, float duration)
    {
        rotatingCanvas = true;
        Quaternion startRotation = buildingCanvas.rotation;
        Quaternion endRotation = Quaternion.Euler(angles) * startRotation;
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            buildingCanvas.rotation = Quaternion.Lerp(startRotation, endRotation, t / duration);
            yield return null;
        }
        buildingCanvas.rotation = endRotation;
        rotatingCanvas = false;
    }

    private IEnumerator RotateItems(Vector3 angles, float duration)
    {
        rotatingItems = true;
        Quaternion startRotation = buildingsItems[0].rotation;
        Quaternion endRotation = Quaternion.Euler(angles) * startRotation;
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            for (int i = 0; i < buildingsItems.Length; i++)
            {
                buildingsItems[i].rotation = Quaternion.Lerp(startRotation, endRotation, t / duration);
            }
            yield return null;
        }

        for (int i = 0; i < buildingsItems.Length; i++)
        {
            buildingsItems[i].rotation = endRotation;
        }
        rotatingItems = false;
    }

}
