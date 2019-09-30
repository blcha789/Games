using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleLayout : MonoBehaviour
{
    public float fDistance;
    [Range(0f, 360f)]
    public float MinAngle, MaxAngle, StartAngle;

    void Start()
    {
        CalculateCircle();
    }

    void CalculateCircle()
    {
        if (transform.childCount == 0)
            return;
        float fOffsetAngle = ((MaxAngle - MinAngle)) / (transform.childCount - 1);

        float fAngle = StartAngle;
        for (int i = 0; i < transform.childCount; i++)
        {
            RectTransform child = (RectTransform)transform.GetChild(i);
            if (child != null)
            {
                Vector3 vPos = new Vector3(Mathf.Cos(fAngle * Mathf.Deg2Rad), Mathf.Sin(fAngle * Mathf.Deg2Rad), 0);
                child.localPosition = vPos * fDistance;
                //Force objects to be center aligned, this can be changed however I'd suggest you keep all of the objects with the same anchor points.
                child.anchorMin = child.anchorMax = child.pivot = new Vector2(0.5f, 0.5f);
                fAngle += fOffsetAngle;
            }
        }
    }
}
