using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FluidDeposit : MonoBehaviour
{

    public string fluidName; //name of fluid in deposit
    public Color fluidColor;//color of fluid
    public float depositSize;//size of deposit
    public LayerMask buildingLayer; //layer of building

    private float size;

    private void Start()
    {
        //set size of deposit
        size = depositSize;
    }

    private void Update()
    {
        if (depositSize <= 0) //if deposit size is 0 or less then is empty and destroy it
            Destroy(gameObject);
    }

    //set default paremeters of deposit
    public void SetDefaults()
    {
        depositSize = size;
    }

    public void TriggerExit()
    {
        //play particle system
        GetComponent<ParticleSystem>().Play();
    }


    //when we move building call trigger exit
    private void OnTriggerExit(Collider col)
    {
        if (col.tag.Contains("Building"))
        {
            TriggerExit();
        }
    }

    //if is on deposit building then stop particle system
    private void OnTriggerStay(Collider col)
    {
        if (col.tag.Contains("Building"))
        {
            GetComponent<ParticleSystem>().Stop();
        }
    }
}
