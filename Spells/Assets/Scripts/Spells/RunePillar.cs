using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunePillar : MonoBehaviour
{

    public float mana;

    void OnTriggerStay(Collider col)
    {
        if (col.tag == "Player" || col.tag == "IPlayer")
        {
            col.GetComponent<CharacterStats>().mana += mana * Time.deltaTime;
        }
    }
}
