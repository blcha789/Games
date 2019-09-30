using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NamePlate : MonoBehaviour {

    public GameObject FollowObject;

	// Update is called once per frame
	void Update () {
		
        if(FollowObject != null)
        {
            transform.rotation = Quaternion.LookRotation(transform.position - FollowObject.transform.position);
        }
	}
}
