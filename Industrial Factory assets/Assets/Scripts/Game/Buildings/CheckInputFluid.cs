using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InputFluid { Input1, Input2, Input3 }

//this script is checking what fluid is on inputs of machines
public class CheckInputFluid : MonoBehaviour
{
    public TypeOfBuilding typeOfBuilding; //what type of building it is (Refinery, Solidifier,..)
    public InputFluid inputFluid; // input1 , input2, input3

    public string fluidName; //name of fluid
    public float fluidAmount; //storage 
    public float fluidMax; //maximum amount of fluid in storage

    private void Update()
    {
        if (fluidAmount > 0) //if is in storage more fluid than 0
        {
            if (typeOfBuilding == TypeOfBuilding.refinery) //check what type of building is this script attached
            {
                if (inputFluid == InputFluid.Input1)//if fluid is going to input 1 then send fluid to storage in refinery script
                    GetComponentInParent<Refinery>().fluid1 += 1 * Time.deltaTime;
                else if (inputFluid == InputFluid.Input2)
                    GetComponentInParent<Refinery>().fluid2 += 1 * Time.deltaTime;
                else
                    GetComponentInParent<Refinery>().fluid3 += 1 * Time.deltaTime;

                fluidAmount -= 1 * Time.deltaTime;//decrease fluid 
            }
            else if (typeOfBuilding == TypeOfBuilding.solidifier)
            {
                if (inputFluid == InputFluid.Input1)
                    GetComponentInParent<Solidifier>().fluid1 += 1 * Time.deltaTime;
                else if (inputFluid == InputFluid.Input2)
                    GetComponentInParent<Solidifier>().fluid2 += 1 * Time.deltaTime;

                fluidAmount -= 1 * Time.deltaTime;
            }
            else if(typeOfBuilding == TypeOfBuilding.buyer)
            {
                if (inputFluid == InputFluid.Input1)
                    GetComponentInParent<Buyer>().fluidCount -= 1 * Time.deltaTime;
            }
        }
    }
}
