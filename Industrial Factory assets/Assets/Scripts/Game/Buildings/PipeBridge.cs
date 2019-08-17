using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeBridge : MonoBehaviour
{
    public LayerMask pipeMask;

    public string fluidName;
    public float fluidAmount;
    public Color fluidColor;

    public Renderer[] Fluid;
    private GameLogic gameLogic;

    private List<Pipe> pipes = new List<Pipe>();
    private List<PipeBridge> pipeBridges = new List<PipeBridge>();
    private List<PipeInput> pipeInputs = new List<PipeInput>();
    private List<CheckInputFluid> pipeMachines = new List<CheckInputFluid>();

    private void Start()
    {
        gameLogic = GameObject.FindGameObjectWithTag("Hierarchy/GameLogic").GetComponent<GameLogic>();
        SetDefaults();
        CheckSides();
        CheckSide();
    }

    private void Update()
    {
        if (gameLogic.isPlaying)
        {
            SendFluid();

            for (int i = 0; i < Fluid.Length; i++)
            {
                Fluid[i].material.color = fluidColor;
            }
        }
    }

    public void SetDefaults()
    {
        fluidName = null;
        fluidAmount = 0;
        fluidColor = Color.white;
        for (int i = 0; i < Fluid.Length; i++)
        {
            Fluid[i].material.color = Color.white;
        }
    }

    public void CheckSides()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.forward, out hit, 2f, pipeMask.value))
        {
            if (hit.transform.CompareTag("Building/Pipe"))
                hit.transform.GetComponent<Pipe>().CheckSide();
        }

        if (Physics.Raycast(transform.position, Vector3.back, out hit, 2f, pipeMask.value))
        {
            if (hit.transform.CompareTag("Building/Pipe"))
                hit.transform.GetComponent<Pipe>().CheckSide();
        }

        if (Physics.Raycast(transform.position, Vector3.left, out hit, 2f, pipeMask.value))
        {
            if (hit.transform.CompareTag("Building/Pipe"))
                hit.transform.GetComponent<Pipe>().CheckSide();
        }

        if (Physics.Raycast(transform.position, Vector3.right, out hit, 2f, pipeMask.value))
        {
            if (hit.transform.CompareTag("Building/Pipe"))
                hit.transform.GetComponent<Pipe>().CheckSide();
        }
    }

    private void SendFluid()
    {
        if (fluidAmount <= 0)
            SetDefaults();

        for (int i = 0; i < pipeMachines.Count; i++)
        {
            if (pipeMachines[i].fluidAmount < fluidAmount && (pipeMachines[i].fluidName == null || pipeMachines[i].fluidName == fluidName))
            {
                pipeMachines[i].fluidAmount += 1 * Time.deltaTime;
                fluidAmount -= 1 * Time.deltaTime;
            }
        }

        for (int i = 0; i < pipeInputs.Count; i++)
        {
            if (pipeInputs[i].fluidName == fluidName)
            {
                pipeInputs[i].fluidCount += 1 * Time.deltaTime;
                fluidAmount -= 1 * Time.deltaTime;
            }
        }

        for (int i = 0; i < pipeBridges.Count; i++)
        {
            if (pipeBridges[i].fluidAmount < fluidAmount && (pipeBridges[i].fluidName == null || pipeBridges[i].fluidName == fluidName))
            {
                pipeBridges[i].fluidAmount += 1 * Time.deltaTime;
                pipeBridges[i].fluidName = fluidName;
                pipeBridges[i].fluidColor = fluidColor;
                fluidAmount -= 1 * Time.deltaTime;
            }
        }

        for (int i = 0; i < pipes.Count; i++)
        {
            if (pipes[i].fluidAmount < fluidAmount && (pipes[i].fluidName == null || pipes[i].fluidName == fluidName))
            {
                pipes[i].fluidAmount += 1 * Time.deltaTime;
                pipes[i].fluidName = fluidName;
                pipes[i].fluidColor = fluidColor;
                fluidAmount -= 1 * Time.deltaTime;
            }
        }
    }

    public void CheckSide()
    {
        RaycastHit hit;

        pipes.Clear();
        pipeBridges.Clear();
        pipeMachines.Clear();
        pipeInputs.Clear();

        if (Physics.Raycast(transform.position, -transform.forward, out hit, 2f, pipeMask.value))
        {
            FindPipes(hit);
        }
        if (Physics.Raycast(transform.position, transform.forward, out hit, 2f, pipeMask.value)) 
        {
            FindPipes(hit);
        }
    }

    private void FindPipes(RaycastHit hit)
    {
        if (hit.collider.CompareTag("Building/Pipe"))
        {
            pipes.Add(hit.collider.GetComponent<Pipe>());
        }
        else if (hit.collider.CompareTag("PipeBridge"))
        {
            pipeBridges.Add(hit.collider.GetComponent<PipeBridge>());
        }
        else if (hit.collider.CompareTag("Building/PipeMachine"))
        {
            pipeMachines.Add(hit.collider.GetComponent<CheckInputFluid>());
        }
        else if (hit.collider.CompareTag("Building/PipeInput"))
        {
            pipeInputs.Add(hit.collider.GetComponent<PipeInput>());
        }
    }
}
