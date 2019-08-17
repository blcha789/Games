using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    public Transform endPos; //position where item on belt will go
    public float speed; //speed that item will have on belt

    private void OnTriggerStay(Collider col)
    {
        if (col.tag == "Item") //if item tag will be Item then it will move to endPos
        {
            Vector3 direction = (endPos.transform.position - col.transform.parent.position).normalized;
            col.GetComponentInParent<Rigidbody>().MovePosition(col.transform.parent.position + direction * speed * Time.deltaTime);
        }
    }
}
