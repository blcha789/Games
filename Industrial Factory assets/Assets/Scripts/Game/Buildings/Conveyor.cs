using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conveyor : MonoBehaviour
{

    public LayerMask conveyorMask; //mask for conveyors
    public Transform rotateConveyor; // transform of parent with conveyor models
    public Transform beltParent;

    public GameObject[] conveyorObjects;//variants of conveyors

    public bool onFront, onBack; // sides of conveyor

    private GameLogic gameLogic;

    void Start()
    {
        gameLogic = GameObject.FindGameObjectWithTag("Hierarchy/GameLogic").GetComponent<GameLogic>();
        CheckSides();
    }

    private void Update()
    {
        Debug.DrawRay(new Vector3(transform.position.x, transform.position.y - 0.35f, transform.position.z), beltParent.transform.right);
    }

    //This function is called when pipe is placed and will connect this pipe to other pipes
    private void CheckSides()
    {
        RaycastHit hit;

        if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y - 0.35f, transform.position.z), beltParent.transform.right, out hit, 1f, conveyorMask.value))
            if (hit.collider.CompareTag("Conveyor"))
            {
                float angle = Quaternion.Angle(hit.transform.GetComponentInParent<Conveyor>().beltParent.rotation, beltParent.rotation);
                if (angle < 10)
                    onFront = true;
            }

        if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y - 0.35f, transform.position.z), -beltParent.transform.right, out hit, 1f, conveyorMask.value))
            if (hit.collider.CompareTag("Conveyor"))
            {
                float angle = Quaternion.Angle(hit.transform.GetComponentInParent<Conveyor>().beltParent.rotation, beltParent.rotation);
                if (angle < 10)
                    onBack = true;
            }

        StartCoroutine(wait());
    }

    private IEnumerator wait()
    {
        yield return new WaitForEndOfFrame();
        RaycastHit hit;

        onFront = false;
        onBack = false;

        if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y - 0.35f, transform.position.z), beltParent.transform.right, out hit, 1f, conveyorMask.value))
            if (hit.collider.CompareTag("Conveyor"))
                if (hit.transform.GetComponentInParent<Conveyor>().beltParent.transform.rotation.y == beltParent.transform.rotation.y)
                    onFront = true;

        if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y - 0.35f, transform.position.z), -beltParent.transform.right, out hit, 1f, conveyorMask.value))
            if (hit.collider.CompareTag("Conveyor"))
                if (hit.transform.GetComponentInParent<Conveyor>().beltParent.transform.rotation.y == beltParent.transform.rotation.y)
                    onBack = true;

        SetSides();
    }

    public void CheckSide()
    {
        RaycastHit hit;

        onFront = false;
        onBack = false;

        if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y - 0.35f, transform.position.z), beltParent.transform.right, out hit, 1f, conveyorMask.value))
            if (hit.collider.CompareTag("Conveyor"))
            {
                float angle = Quaternion.Angle(hit.transform.GetComponentInParent<Conveyor>().beltParent.rotation, beltParent.rotation);
                if (angle < 10)
                    onFront = true;
            }

        if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y - 0.35f, transform.position.z), -beltParent.transform.right, out hit, 1f, conveyorMask.value))
            if (hit.collider.CompareTag("Conveyor"))
            {
                float angle = Quaternion.Angle(hit.transform.GetComponentInParent<Conveyor>().beltParent.rotation, beltParent.rotation);
                if (angle < 10)
                    onBack = true;
            }

        SetSides();
    }

    private void SetSides()
    {
        for (int i = 0; i < conveyorObjects.Length; i++)
        {
            conveyorObjects[i].SetActive(false);
        }

        if (onFront && onBack)//middle conveyor
        {
            conveyorObjects[2].SetActive(true);
        }
        else if (onFront && !onBack)
        {
            conveyorObjects[1].SetActive(true);
        }
        else if (!onFront && onBack)
        {
            conveyorObjects[3].SetActive(true);
        }
        else
        {
            conveyorObjects[0].SetActive(true);
        }
    }

    public void CheckSidesOnDestroyRotate()
    {
        RaycastHit hit;

        if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y - 0.35f, transform.position.z), beltParent.transform.right, out hit, 1f, conveyorMask.value))
            if (hit.collider.CompareTag("Conveyor"))
                hit.collider.GetComponentInParent<Conveyor>().CheckSide();

        if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y - 0.35f, transform.position.z), -beltParent.transform.right, out hit, 1f, conveyorMask.value))
            if (hit.collider.CompareTag("Conveyor"))
                hit.collider.GetComponentInParent<Conveyor>().CheckSide();

        if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y - 0.35f, transform.position.z), beltParent.transform.forward, out hit, 1f, conveyorMask.value))
            if (hit.collider.CompareTag("Conveyor"))
                hit.collider.GetComponentInParent<Conveyor>().CheckSide();

        if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y - 0.35f, transform.position.z), -beltParent.transform.forward, out hit, 1f, conveyorMask.value))
            if (hit.collider.CompareTag("Conveyor"))
                hit.collider.GetComponentInParent<Conveyor>().CheckSide();

        CheckSide();
    }

    public void CheckSidesOnMove()
    {
        Invoke("CheckSidesOnDestroyRotate", 0.3f);
    }
}
