using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float movementSpeed;

    private Transform player;
    private Rigidbody rb;

    private bool isStuned = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Character").transform;
        rb = GetComponent<Rigidbody>();
    }
    
    public void MovementSettings(float minMoveSpeed, float maxMoveSpeed)
    {
        movementSpeed = Random.Range(minMoveSpeed, maxMoveSpeed);
    }
    
    public void Stun(bool _isStuned)
    {
      isStuned = _isStuned;
    }

    public void Slow(float slowSpeed)
    {
       movementSpeed += slowSpeed;
    }

    void FixedUpdate()
    {
      if(!isStuned)
      {        
        transform.LookAt(player.position);
        Vector3 direction = (player.position - transform.position).normalized;
        rb.MovePosition(transform.position + direction * movementSpeed * Time.deltaTime);
      }
   }
}
