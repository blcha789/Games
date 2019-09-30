using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollision : MonoBehaviour
{

    public float minDistance = 1.0f;
    public float maxDistance = 4.0f;
    public float smooth = 10f;

    public LayerMask mask;

    Vector3 dollyDir;
    public Vector3 dollyDirAdjusted;
    public float distance;

    // Use this for initialization
    void Awake()
    {
        dollyDir = transform.localPosition.normalized;
        distance = transform.localPosition.magnitude;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 desiredCameraPos = transform.parent.TransformPoint(dollyDir * maxDistance);
        RaycastHit hit;

        Debug.DrawLine(transform.parent.position, desiredCameraPos);

        if (Physics.Linecast(transform.parent.position, desiredCameraPos, out hit, mask.value))
        {
            distance = Mathf.Clamp((hit.distance * 0.9f), minDistance, maxDistance);
        }
        else
        {
            distance = maxDistance;
        }

        transform.localPosition = Vector3.Lerp(transform.localPosition, dollyDir * distance, Time.deltaTime * smooth);
    }
}
