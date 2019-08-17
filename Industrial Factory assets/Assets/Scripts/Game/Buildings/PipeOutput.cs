using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeOutput : MonoBehaviour {

    public LayerMask pipeMask;

    public string fluidName;
    public Color fluidColor;

    private GameLogic gameLogic;

    private void Start()
    {
        gameLogic = GameObject.FindGameObjectWithTag("Hierarchy/GameLogic").GetComponent<GameLogic>();
    }

    private void Update()
    {
        if (gameLogic.isPlaying)
            SendFluid();
    }

    private void SendFluid()
    {
        RaycastHit hit;

        Debug.DrawRay(transform.position, transform.right);

        if (Physics.Raycast(transform.position, transform.right, out hit, 2f, pipeMask.value))
        {
            if (hit.collider.CompareTag("Building/Pipe"))
            {
                if (hit.collider.GetComponent<Pipe>().fluidAmount < 10 && (hit.collider.GetComponent<Pipe>().fluidName == null || hit.collider.GetComponent<Pipe>().fluidName == fluidName))
                {
                    hit.collider.GetComponent<Pipe>().fluidAmount += 1 * Time.deltaTime;
                    hit.collider.GetComponent<Pipe>().fluidName = fluidName;
                    hit.collider.GetComponent<Pipe>().fluidColor = fluidColor;
                }
            }
            else if (hit.collider.CompareTag("Building/PipeBridge"))
            {
                if (hit.collider.GetComponent<PipeBridge>().fluidAmount < 10 && (hit.collider.GetComponent<PipeBridge>().fluidName == null || hit.collider.GetComponent<PipeBridge>().fluidName == fluidName))
                {
                    hit.collider.GetComponent<PipeBridge>().fluidAmount += 1 * Time.deltaTime;
                    hit.collider.GetComponent<PipeBridge>().fluidName = fluidName;
                    hit.collider.GetComponent<Pipe>().fluidColor = fluidColor;
                }
            }
            else if (hit.collider.CompareTag("Building/PipeMachine"))
            {
                if (hit.collider.GetComponent<CheckInputFluid>().fluidMax > hit.collider.GetComponent<CheckInputFluid>().fluidAmount && hit.collider.GetComponent<CheckInputFluid>().fluidName == fluidName)
                {
                    hit.collider.GetComponent<CheckInputFluid>().fluidAmount += 1 * Time.deltaTime;
                }
            }
        }
    }
}
