using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float movementSpeed;

    private Transform player;
    private Rigidbody rb;

    private float halfMovementSpeedOnStart;
    private bool stun = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Character").transform;
        rb = GetComponent<Rigidbody>();

        halfMovementSpeedOnStart = movementSpeed / 3;
    }

    public void Slow(float slowSpeed)
    {
        if (stun)
        {
            movementSpeed -= slowSpeed;

            if (movementSpeed <= halfMovementSpeedOnStart)
                movementSpeed = halfMovementSpeedOnStart;
        }
    }

    public void Stun()
    {
        rb.constraints = RigidbodyConstraints.FreezeAll;
        stun = true;
    }

    void FixedUpdate()
    {
        transform.LookAt(player.position);
        Vector3 direction = (player.position - transform.position).normalized;
        rb.MovePosition(transform.position + direction * movementSpeed * Time.deltaTime);
    }
}
