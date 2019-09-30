using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CharacterMovement : NetworkBehaviour
{
    [Header("Main")]
    public float speed;
    public float strafeSpeed;
    public float jumpSpeed;
    public float slideSpeed = 2.0f;

    float _speed;
    float _strafeSpeed;

    [SyncVar]
    Vector3 syncPos;
    Vector3 move;
    float lerpRate = 15;

    public Animator anim;
    CharacterController characterController;

    float forward;
    float strafe;

    float h, v;

    private RaycastHit hit;
    private float slideLimit;
    private float rayDistance;
    private Vector3 contactPoint;

    public float antiBumpFactor = .75f;

    // Use this for initialization
    void Start()
    {
        if (!isLocalPlayer)
            return;

        characterController = GetComponent<CharacterController>();

        _speed = speed;
        _strafeSpeed = strafeSpeed;

        slideLimit = characterController.slopeLimit - .1f;
        rayDistance = characterController.height * .5f + characterController.radius;
    }

    void Update()
    {
        if (!isLocalPlayer)
            return;

        if (!GetComponent<CharacterStats>().isDead)
        {
            if (!GetComponent<CharacterStats>().spellEffect[0].effectIsOn && !GetComponent<CharacterStats>().spellEffect[1].effectIsOn)
            {

                if (GetComponent<CharacterStats>().spellEffect[3].effectIsOn)
                {
                    _speed = speed / GetComponent<CharacterStats>().spellEffect[3].effectStat;
                    _strafeSpeed = strafeSpeed / GetComponent<CharacterStats>().spellEffect[3].effectStat;
                }
                else
                {
                    _speed = speed;
                    _strafeSpeed = strafeSpeed;
                }

                if (characterController.isGrounded)
                {
                    if (!GetComponent<CharactersActions>().isActiveEscMenu)
                    {
                        h = Input.GetAxis("Horizontal");
                        v = Input.GetAxis("Vertical");
                    }
                    else
                    {
                        h = 0;
                        v = 0;
                    }

                    bool sliding = false;
                    if (Physics.Raycast(transform.position, -Vector3.up, out hit, rayDistance))
                    {
                        if (Vector3.Angle(hit.normal, Vector3.up) > slideLimit)
                            sliding = true;
                    }
                    else
                    {
                        Physics.Raycast(contactPoint + Vector3.up, -Vector3.up, out hit);
                        if (Vector3.Angle(hit.normal, Vector3.up) > slideLimit)
                            sliding = true;
                    }

                    if (sliding)
                    {
                        Vector3 hitNormal = hit.normal;
                        move = new Vector3(hitNormal.x, -hitNormal.y, hitNormal.z);
                        Vector3.OrthoNormalize(ref hitNormal, ref move);
                        move *= slideSpeed;
                    }
                    else
                    {
                        move = new Vector3(h * _strafeSpeed, -antiBumpFactor, v * _speed);
                        move = transform.TransformDirection(move);

                        if (Input.GetKeyDown(KeyCode.Space) && !GetComponent<CharactersActions>().isActiveEscMenu)
                            move.y = jumpSpeed;

                        forward = move.z;
                        strafe = move.x;
                    }
                }

                characterController.Move(move * Time.deltaTime);
            }
            else
            {
                forward = 0;
                strafe = 0;
            }
        }
        else
        {
            forward = 0;
            strafe = 0;
        }
        UpdateAnimator();
    }

    void FixedUpdate()
    {
        TransmitPosition();
        if (!isLocalPlayer)
            transform.position = Vector3.Lerp(transform.position, syncPos, Time.deltaTime * lerpRate);
    }

    [Command]
    void CmdTransmitPosition(Vector3 pos)
    {
        syncPos = pos;
    }

    [ClientCallback]
    void TransmitPosition()
    {
        if (isLocalPlayer)
            CmdTransmitPosition(transform.position);
    }

    void UpdateAnimator()
    {
        anim.SetFloat("Forward", forward, 0.1f, Time.deltaTime);
        anim.SetFloat("Strafe", strafe, 0.1f, Time.deltaTime);

        CmdUpdateAnimator(forward, strafe);
    }

    [Command]
    void CmdUpdateAnimator(float Forward, float Strafe)
    {
        RpcUpdateAnimator(Forward, Strafe);
    }

    [ClientRpc]
    void RpcUpdateAnimator(float Forward, float Strafe)
    {
        if (isLocalPlayer)
            return;

        anim.SetFloat("Forward", Forward, 0.1f, Time.deltaTime);
        anim.SetFloat("Strafe", Strafe, 0.1f, Time.deltaTime);
    }
}
