using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipe : MonoBehaviour
{
    public LayerMask pipeMask; //mask for pipes, machine pipes
    public Transform rotatePipe; // transform of pipe

    public string fluidName; //name of fluid in pipe
    public float fluidAmount; //amount fluid in pipe
    public Color fluidColor; // fluid color

    public Renderer Fluid; //object renderer on pipe that will change color by fluid that is in pipe
    public GameObject[] pipeObjects;//variants of pipe

    public bool onFront, onBack, onLeft, onRight; // sides of pipe

    private GameLogic gameLogic;

    private List<Pipe> pipes = new List<Pipe>(); //list of pipes that are connected to this pipe
    private List<PipeBridge> pipeBridges = new List<PipeBridge>(); // list of pipe bridges that are connected to this pipe
    private List<PipeInput> pipeInputs = new List<PipeInput>(); // list of pipe inputs
    private List<CheckInputFluid> pipeMachines = new List<CheckInputFluid>(); // list of pipe machines 

    private void Start()
    {
        gameLogic = GameObject.FindGameObjectWithTag("Hierarchy/GameLogic").GetComponent<GameLogic>();
        CheckSides();
        SetDefaults();
    }

    private void Update()
    {
        if (gameLogic.isPlaying)//if is in play mode 
        {
            SendFluid();
            Fluid.material.color = fluidColor;
        }
    }

    //This function set pipe to default state
    public void SetDefaults()
    {
        fluidName = null;
        fluidAmount = 0;
        Fluid.material.color = Color.white;
        fluidColor = Color.white;
    }


    //In this function is fluid send to other pipes
    private void SendFluid()
    {
        if (fluidAmount <= 0) //if fluid is 0 or less then set pipe to default
            SetDefaults();

        //send fluid to all pipe machines that are connected
        for (int i = 0; i < pipeMachines.Count; i++) 
        {
            if (pipeMachines[i].fluidName == fluidName) //only if fluid name in pipe machine is equal to fluid name in this pipe
            {
                pipeMachines[i].fluidAmount += 1 * Time.deltaTime; //increase fluid in pipe machine
                fluidAmount -= 1 * Time.deltaTime;//decrease fluid in this pipe
            }
        }

        for (int i = 0; i < pipes.Count; i++)
        {
            //send fluid only if fluid in pipe that will be send to is less than in this pipe and if that pipe is epty or is there same fluid          
            if (pipes[i].fluidAmount < fluidAmount && (pipes[i].fluidName == null || pipes[i].fluidName == fluidName))
            { 
                pipes[i].fluidAmount += 1 * Time.deltaTime;
                pipes[i].fluidName = fluidName;
                pipes[i].fluidColor = fluidColor;
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
    }

    //This function is called when pipe is placed and will connect this pipe to other pipes
    public void CheckSides()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.forward, out hit, 1f, pipeMask.value))
        {
            if (hit.collider.CompareTag("Building/Pipe"))
                hit.collider.GetComponent<Pipe>().CheckSide();
            if (hit.collider.CompareTag("PipeBridge"))
                hit.collider.GetComponent<PipeBridge>().CheckSide();
        }

        if (Physics.Raycast(transform.position, Vector3.back, out hit, 1f, pipeMask.value))
        {
            if (hit.collider.CompareTag("Building/Pipe"))
                hit.collider.GetComponent<Pipe>().CheckSide();
            if (hit.collider.CompareTag("PipeBridge"))
                hit.collider.GetComponent<PipeBridge>().CheckSide();
        }

        if (Physics.Raycast(transform.position, Vector3.left, out hit, 1f, pipeMask.value))
        {
            if (hit.collider.CompareTag("Building/Pipe"))
                hit.collider.GetComponent<Pipe>().CheckSide();
            if (hit.collider.CompareTag("PipeBridge"))
                hit.collider.GetComponent<PipeBridge>().CheckSide();
        }

        if (Physics.Raycast(transform.position, Vector3.right, out hit, 1f, pipeMask.value))
        {
            if (hit.collider.CompareTag("Building/Pipe"))
                hit.collider.GetComponent<Pipe>().CheckSide();
            if (hit.collider.CompareTag("PipeBridge"))
                hit.collider.GetComponent<PipeBridge>().CheckSide();
        }

        StartCoroutine(wait());
    }

    //waiting to end of frame to set list of pipes that are connected
    private IEnumerator wait()
    {
        yield return new WaitForEndOfFrame();
        RaycastHit hit;

        onFront = false;
        onBack = false;
        onLeft = false;
        onRight = false;

        pipes.Clear();
        pipeBridges.Clear();
        pipeMachines.Clear();
        pipeInputs.Clear();

        if (Physics.Raycast(transform.position, Vector3.forward, out hit, 1f, pipeMask.value))
        {
            onFront = true;
            FindPipes(hit);
        }

        if (Physics.Raycast(transform.position, Vector3.back, out hit, 1f, pipeMask.value))
        {
            onBack = true;
            FindPipes(hit);
        }

        if (Physics.Raycast(transform.position, Vector3.left, out hit, 1f, pipeMask.value))
        {
            onLeft = true;
            FindPipes(hit);
        }

        if (Physics.Raycast(transform.position, Vector3.right, out hit, 1f, pipeMask.value))
        {
            onRight = true;
            FindPipes(hit);
        }

        SetSides();
    }

    //this function is called when another pipes will be connected to this pipe and this pipe will connect to other pipe
    public void CheckSide()
    {
        RaycastHit hit;

        onFront = false;
        onBack = false;
        onLeft = false;
        onRight = false;

        pipes.Clear();
        pipeBridges.Clear();
        pipeMachines.Clear();
        pipeInputs.Clear();

        if (Physics.Raycast(transform.position, Vector3.forward, out hit, 1f, pipeMask.value))
        {
            onFront = true;
            FindPipes(hit);
        }

        if (Physics.Raycast(transform.position, Vector3.back, out hit, 1f, pipeMask.value))
        {
            onBack = true;
            FindPipes(hit);
        }

        if (Physics.Raycast(transform.position, Vector3.left, out hit, 1f, pipeMask.value))
        {
            onLeft = true;
            FindPipes(hit);
        }

        if (Physics.Raycast(transform.position, Vector3.right, out hit, 1f, pipeMask.value))
        {
            onRight = true;
            FindPipes(hit);
        }

        SetSides();
    }


    //this function is for seting list of pipes that are connected
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

    //Here will be pipe connected to other pipes
    private void SetSides() 
    {
        for (int i = 0; i < pipeObjects.Length; i++)
        {
            pipeObjects[i].SetActive(false);
        }

        if (onFront && onBack && onLeft && onRight)//plusPipe
        {
            pipeObjects[3].SetActive(true);
        }
        else if (onFront && onBack && !onLeft && !onRight)//linePipe front
        {         
            pipeObjects[0].SetActive(true);
            rotatePipe.rotation = Quaternion.Euler(0, 90, 0);
        }
        else if (onLeft && onRight && !onFront && !onBack)//linePipe side
        {           
            pipeObjects[0].SetActive(true);
            rotatePipe.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (onFront && onRight && onBack && !onLeft)//T Pipe 1
        {          
            pipeObjects[1].SetActive(true);
            rotatePipe.rotation = Quaternion.Euler(0, 90, 0);
        }
        else if (onRight && onBack && onLeft && !onFront)//T Pipe 2
        {           
            pipeObjects[1].SetActive(true);
            rotatePipe.rotation = Quaternion.Euler(0, 180, 0);
        }
        else if (onBack && onLeft && onFront && !onRight)//T Pipe 3
        {           
            pipeObjects[1].SetActive(true);
            rotatePipe.rotation = Quaternion.Euler(0, 270, 0);
        }
        else if (onLeft && onFront && onRight && !onBack)//T Pipe 4
        {           
            pipeObjects[1].SetActive(true);
            rotatePipe.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (onFront && onRight && !onLeft && !onBack)//L Pipe 1
        {           
            pipeObjects[2].SetActive(true);
            rotatePipe.rotation = Quaternion.Euler(0, 90, 0);
        }
        else if (onRight && onBack && !onFront && !onLeft)//L Pipe 2
        {           
            pipeObjects[2].SetActive(true);
            rotatePipe.rotation = Quaternion.Euler(0, 180, 0);
        }
        else if (onBack && onLeft && !onFront && !onRight)//L Pipe 3
        {           
            pipeObjects[2].SetActive(true);
            rotatePipe.rotation = Quaternion.Euler(0, 270, 0);
        }
        else if (onLeft && onFront && !onBack && !onRight)//L Pipe 4
        {           
            pipeObjects[2].SetActive(true);
            rotatePipe.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {           
            pipeObjects[0].SetActive(true);

            if (onFront || onBack)
                rotatePipe.rotation = Quaternion.Euler(0, 90, 0);
            else if(onRight || onLeft)
                rotatePipe.rotation = Quaternion.Euler(0, 0, 0);
        }
    }


    //when pipe is destroy it will send other pipes they are not connected to this pipe
    private void OnDestroy()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.forward, out hit, 1f, pipeMask.value))
        {
            if (hit.transform.CompareTag("Building/Pipe"))
                hit.transform.GetComponent<Pipe>().CheckSide();
        }

        if (Physics.Raycast(transform.position, Vector3.back, out hit, 1f, pipeMask.value))
        {
            if (hit.transform.CompareTag("Building/Pipe"))
                hit.transform.GetComponent<Pipe>().CheckSide();
        }

        if (Physics.Raycast(transform.position, Vector3.left, out hit, 1f, pipeMask.value))
        {
            if (hit.transform.CompareTag("Building/Pipe"))
                hit.transform.GetComponent<Pipe>().CheckSide();
        }

        if (Physics.Raycast(transform.position, Vector3.right, out hit, 1f, pipeMask.value))
        {
            if (hit.transform.CompareTag("Building/Pipe"))
                hit.transform.GetComponent<Pipe>().CheckSide();
        }
    }
}
