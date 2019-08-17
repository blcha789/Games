using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FluidRig : MonoBehaviour {

    public LayerMask pipeMask;
    public LayerMask fluidLayerMask;
    public AudioSource audioSource;

    public GameObject fluidOutputCanvas;

    private GameLogic gameLogic;
    private FluidDeposit fluidDeposit;

    private void Start()
    {
        gameLogic = GameObject.FindGameObjectWithTag("Hierarchy/GameLogic").GetComponent<GameLogic>();
    }

    private void Update()
    {
        if (fluidDeposit != null)
        {
            if (gameLogic.isPlaying)
            {
                if (!audioSource.isPlaying)
                    audioSource.Play();

                SendFluid();
            }
        }
    }

    public void SetDefaults()
    {
        fluidOutputCanvas.SetActive(false);
        fluidOutputCanvas.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = Color.clear;
        audioSource.Stop();
    }

    public void FindDeposit()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.parent.position, Vector3.down, out hit, Mathf.Infinity, fluidLayerMask.value))
        {
            fluidDeposit = hit.collider.GetComponent<FluidDeposit>();
            fluidOutputCanvas.SetActive(true);
            fluidOutputCanvas.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = hit.collider.GetComponent<FluidDeposit>().fluidColor;
        }
    }

    private void SendFluid()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.right, out hit, 2f, pipeMask.value))
        {
            if (hit.collider.CompareTag("Building/Pipe"))
            {
                if (hit.collider.GetComponent<Pipe>().fluidAmount < 10 && (hit.collider.GetComponent<Pipe>().fluidName == null || hit.collider.GetComponent<Pipe>().fluidName == fluidDeposit.fluidName))
                {
                    hit.collider.GetComponent<Pipe>().fluidAmount += 1 * Time.deltaTime;
                    hit.collider.GetComponent<Pipe>().fluidName = fluidDeposit.fluidName;
                    hit.collider.GetComponent<Pipe>().fluidColor = fluidDeposit.fluidColor;

                    fluidDeposit.depositSize -= 1 * Time.deltaTime;
                }
            }
            else if (hit.collider.CompareTag("Building/PipeBridge"))
            {
                if (hit.collider.GetComponent<PipeBridge>().fluidAmount < 10 && (hit.collider.GetComponent<PipeBridge>().fluidName == null || hit.collider.GetComponent<PipeBridge>().fluidName == fluidDeposit.fluidName))
                {
                    hit.collider.GetComponent<PipeBridge>().fluidAmount += 1 * Time.deltaTime;
                    hit.collider.GetComponent<PipeBridge>().fluidName = fluidDeposit.fluidName;
                    hit.collider.GetComponent<Pipe>().fluidColor = fluidDeposit.fluidColor;

                    fluidDeposit.depositSize -= 1 * Time.deltaTime;
                }
            }
            else if (hit.collider.CompareTag("Building/PipeMachine"))
            {
                if (hit.collider.GetComponent<CheckInputFluid>().fluidMax > hit.collider.GetComponent<CheckInputFluid>().fluidAmount && hit.collider.GetComponent<CheckInputFluid>().fluidName == fluidDeposit.fluidName)
                {
                    hit.collider.GetComponent<CheckInputFluid>().fluidAmount += 1 * Time.deltaTime;

                    fluidDeposit.depositSize -= 1 * Time.deltaTime;
                }
            }
        }
    }
}
