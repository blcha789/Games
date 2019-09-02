using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InputItem {Input1, Input2}

public class CheckInputItem : MonoBehaviour
{

    public TypeOfBuilding typeOfBuilding;//what type of building it is (Refinery, Solidifier,..)

    public string itemName; //item name that will accept
    public InputItem inputNumber; //input1, input2

    private void OnTriggerEnter(Collider col) //on trigger item
    {
        if (col.CompareTag("ItemModel")) //check if tag of item is ItemModel
        {
            if (col.name.Equals(itemName + "(Clone)"))//If item name equals item set on input then will increase in storage 
            {
                if (typeOfBuilding == TypeOfBuilding.assembler)
                {
                    if (inputNumber == InputItem.Input1)
                    {
                        GetComponentInParent<Assembler>().item1 += 1;
                    }
                    if (inputNumber == InputItem.Input2)
                    {
                        GetComponentInParent<Assembler>().item2 += 1;
                    }
                }
                else if (typeOfBuilding == TypeOfBuilding.solidifier)
                {
                    if (inputNumber == InputItem.Input1)
                    {
                        GetComponentInParent<Solidifier>().item1 += 1;
                    }
                    if (inputNumber == InputItem.Input2)
                    {
                        GetComponentInParent<Solidifier>().item2 += 1;
                    }
                }
                else if (typeOfBuilding == TypeOfBuilding.extruder)
                {
                    if (inputNumber == InputItem.Input1)
                    {
                        GetComponentInParent<Extruder>().item1 += 1;
                    }
                }
                else if (typeOfBuilding == TypeOfBuilding.buyer)
                {
                    if (inputNumber == InputItem.Input1)
                    {
                        GetComponentInParent<Buyer>().itemCount -= 1;
                    }
                }
                else if(typeOfBuilding == TypeOfBuilding.powerPlant)
                {
                    if (inputNumber == InputItem.Input1)
                    {
                        GetComponentInParent<PowerPlant>().itemCount += 1;
                    }
                }
                Destroy(col.gameObject);
            }
            else
            {
                if (typeOfBuilding == TypeOfBuilding.furnace)
                {
                    GetComponentInParent<Furnace>().items.Add(col.gameObject.name);
                }
                else if (typeOfBuilding == TypeOfBuilding.oreCrusher)
                {
                    GetComponentInParent<OreCrusher>().items.Add(col.gameObject.name);
                }
                Destroy(col.gameObject);
            }
        }
    }
}
