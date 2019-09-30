using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleFollowPath : MonoBehaviour
{

    public float time;

    public GameObject manaParticle;

    bool tripDone;

    int count;
    Vector3 nodePosEnd;
    Vector3 nodePosStart;

    // Use this for initialization
    void Start()
    {
        tripDone = false;
        iTween.MoveTo(manaParticle, iTween.Hash("path", iTweenPath.GetPath(GetComponent<iTweenPath>().pathName), "easetype", iTween.EaseType.easeInOutSine, "time", time));
        count = GetComponent<iTweenPath>().nodeCount - 1;

        nodePosEnd = new Vector3(GetComponent<iTweenPath>().nodes[count].x, GetComponent<iTweenPath>().nodes[count].y, GetComponent<iTweenPath>().nodes[count].z);
        nodePosStart = new Vector3(GetComponent<iTweenPath>().nodes[0].x, GetComponent<iTweenPath>().nodes[0].y, GetComponent<iTweenPath>().nodes[0].z);
    }

    void Update()
    {
        if (tripDone == false)
        {
            if (Mathf.Approximately(manaParticle.transform.position.x, nodePosEnd.x) && Mathf.Approximately(manaParticle.transform.position.y, nodePosEnd.y) && Mathf.Approximately(manaParticle.transform.position.z, nodePosEnd.z))
            {
                manaParticle.transform.position = nodePosStart;
                tripDone = true;
            }
        }
        else
        {
            iTween.MoveTo(manaParticle, iTween.Hash("path", iTweenPath.GetPath(GetComponent<iTweenPath>().pathName), "easetype", iTween.EaseType.easeInOutSine, "time", time));
            tripDone = false;
        }
    }
}
