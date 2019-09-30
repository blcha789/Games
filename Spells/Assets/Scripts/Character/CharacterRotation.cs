using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CharacterRotation : NetworkBehaviour
{

    public enum RotationAxes { MouseX = 0, MouseY = 2 }
    public RotationAxes axes = RotationAxes.MouseX;

    [Header("MouseX")]
    public float sensitivityX;
    public float minimumX = -360F;
    public float maximumX = 360F;

    [SyncVar]
    Quaternion syncRot;
    float lerpRate = 15;

    [Header("MouseY")]
    public float sensitivityY;
    public float minimumY = -60F;
    public float maximumY = 60F;
    public Transform ShotPos;

    float rotationX = 0F;
    float rotationY = 0F;

    public Quaternion originalRotation;
    Quaternion xQuaternion;

    void Start()
    {
        originalRotation = transform.localRotation;

        if (axes == RotationAxes.MouseY)
        {
            Vector3 rot = transform.localRotation.eulerAngles;
            rotationY = rot.y;
        }
    }

    void FixedUpdate()
    {
        if (axes == RotationAxes.MouseX)
        {
            if (!GetComponent<CharacterStats>().isDead)
            {
                if (!GetComponent<CharacterStats>().spellEffect[0].effectIsOn)
                {
                    if (GetComponent<CharactersActions>().isCursorLocked)
                    {
                        rotationX += Input.GetAxis("Mouse X") * sensitivityX;
                        rotationX = ClampAngle(rotationX, minimumX, maximumX);

                        xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
                        transform.localRotation = originalRotation * xQuaternion;
                    }                
                }
            }

            TransmitPosition();
            if (!isLocalPlayer)
                transform.rotation = Quaternion.Lerp(transform.rotation, syncRot, Time.deltaTime * lerpRate);
        }
        else
        {
            if (!GetComponentInParent<CharacterStats>().isDead)
            {
                if (GetComponentInParent<CharactersActions>().isCursorLocked)
                {
                    if (!GetComponentInParent<CharacterStats>().spellEffect[0].effectIsOn)
                    {
                        rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
                        rotationY = ClampAngle(rotationY, minimumY, maximumY);
                        Quaternion yQuaternion = Quaternion.AngleAxis(-rotationY, Vector3.right);

                        transform.localRotation = originalRotation * yQuaternion;
                        ShotPos.localRotation = new Quaternion(yQuaternion.x, ShotPos.localRotation.y, ShotPos.localRotation.z, ShotPos.localRotation.w);// yQuaternion;
                    }
                }
            }
        }
    }

    [Command]
    void CmdTransmitPosition(Quaternion rot)
    {
        syncRot = rot;
    }

    [ClientCallback]
    void TransmitPosition()
    {
        if (isLocalPlayer)
            CmdTransmitPosition(transform.rotation);
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        angle = angle % 360;
        if ((angle >= -360F) && (angle <= 360F))
        {
            if (angle < -360F)
            {
                angle += 360F;
            }
            if (angle > 360F)
            {
                angle -= 360F;
            }
        }
        return Mathf.Clamp(angle, min, max);
    }
}
