using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public Transform target;
    public float moveSpeed = 3f;

    private FixedJoystick moveJoystick;
    private Animator anim;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();

        moveJoystick = GameObject.FindGameObjectWithTag("Joystick").GetComponent<FixedJoystick>();
    }

    void FixedUpdate()
    {
        Movement();
        Rotate();
    }

    private void Movement()
    {
        rb.velocity = new Vector3(moveJoystick.Horizontal, 0f, moveJoystick.Vertical) * moveSpeed;

        anim.SetFloat("Forward", moveJoystick.Horizontal);
        anim.SetFloat("Strafe", moveJoystick.Vertical);

        if (rb.velocity.magnitude > moveSpeed)
        {
            rb.velocity = rb.velocity.normalized * moveSpeed;
            //movementSound.Play();
        }
    }

    private void Rotate()
    {
        if (target == null)
        {
            Vector3 targetDirection = new Vector3(moveJoystick.Horizontal, 0f, moveJoystick.Vertical);

            if (targetDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
                transform.rotation = targetRotation;
            }
        }
        else
            transform.LookAt(target.position);
    }
}
