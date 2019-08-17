using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutputFluid : MonoBehaviour
{
    public LayerMask pipeMask;
    public string fluidName;
    public Color fluidColor;
    public float fluidAmount;

    private GameLogic gameLogic;

    private void Start()
    {
        gameLogic = GameObject.Find("GameLogic").GetComponent<GameLogic>();
    }

    private void Update()
    {
        if (gameLogic.isPlaying)
            SendFluid();
    }

    private void SendFluid()
    {
        Debug.DrawRay(transform.position, transform.right);

        RaycastHit hit;

        if (fluidAmount > 0)
        {
            if (Physics.Raycast(transform.position, transform.right, out hit, 1f, pipeMask.value))
            {
                if (hit.collider.CompareTag("Building/Pipe"))
                {
                    if (hit.collider.GetComponent<Pipe>().fluidAmount < 10 && (hit.collider.GetComponent<Pipe>().fluidName == null || hit.collider.GetComponent<Pipe>().fluidName == fluidName))
                    {
                        hit.collider.GetComponent<Pipe>().fluidAmount += 1 * Time.deltaTime;
                        hit.collider.GetComponent<Pipe>().fluidName = fluidName;
                        hit.collider.GetComponent<Pipe>().fluidColor = fluidColor;
                        fluidAmount -= 1 * Time.deltaTime;
                    }
                }
                else if (hit.collider.CompareTag("PipeBridge"))
                {
                    if (hit.collider.GetComponent<PipeBridge>().fluidAmount < 10 && (hit.collider.GetComponent<PipeBridge>().fluidName == null || hit.collider.GetComponent<PipeBridge>().fluidName == fluidName))
                    {
                        hit.collider.GetComponent<PipeBridge>().fluidAmount += 1 * Time.deltaTime;
                        hit.collider.GetComponent<PipeBridge>().fluidName = fluidName;
                        hit.collider.GetComponent<PipeBridge>().fluidColor = fluidColor;
                        fluidAmount -= 1 * Time.deltaTime;
                    }
                }
                else if (hit.collider.CompareTag("Building/PipeMachine"))
                {
                    if (hit.collider.GetComponent<CheckInputFluid>().fluidAmount < hit.collider.GetComponent<CheckInputFluid>().fluidMax && hit.collider.GetComponent<CheckInputFluid>().fluidName == fluidName)
                    {
                        hit.collider.GetComponent<CheckInputFluid>().fluidAmount += 1 * Time.deltaTime;
                        fluidAmount -= 1 * Time.deltaTime;
                    }
                }
                else if (hit.collider.CompareTag("Building/PipeInput"))
                {
                    if (hit.collider.GetComponent<PipeInput>().fluidName == fluidName)
                    {
                        hit.collider.GetComponent<PipeInput>().fluidCount += 1 * Time.deltaTime;
                        fluidAmount -= 1 * Time.deltaTime;
                    }
                }
            }
        }
    }
}
