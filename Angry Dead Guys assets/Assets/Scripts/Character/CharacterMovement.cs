using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterMovement : MonoBehaviour {

    public Transform character;
    public float moveSpeed = 3f;
    //public AudioSource movementSound;

    private Camera cam;
    private Rigidbody rb;
    private FixedJoystick moveJoystick;
    private CircleJoystick rotateJoystick;
    private Animator anim;

    private void Awake()
    {
        cam = Camera.main;
        rb = GetComponent<Rigidbody>();
        anim = transform.GetChild(1).GetComponent<Animator>();
        moveJoystick = GameObject.FindGameObjectWithTag("Joystick").GetComponent<FixedJoystick>();
        rotateJoystick = GameObject.FindGameObjectWithTag("RotateJoystick").GetComponent<CircleJoystick>();
    }
	
	private void Update ()
    {
        Rotate();
	}

    private void FixedUpdate()
    {
        Movement();
    }

    public void ResetMovement()
    {
        moveJoystick.Horizontal = 0;
        moveJoystick.Vertical = 0;
        moveJoystick.handle.anchoredPosition = Vector2.zero;
    }

    private void Movement()
    {
        rb.velocity = new Vector3(moveJoystick.Horizontal, 0f, moveJoystick.Vertical) * moveSpeed;

        anim.SetFloat("JoystickHorizontal", moveJoystick.Horizontal);
        anim.SetFloat("JoystickVertical", moveJoystick.Vertical);

        if (rb.velocity.magnitude > moveSpeed)
        {
            rb.velocity = rb.velocity.normalized * moveSpeed;
            //movementSound.Play();
        }
    }

    private void Rotate()
    {
        character.eulerAngles = new Vector3(0, Mathf.Atan2(rotateJoystick.Horizontal, rotateJoystick.Vertical) * 180 / Mathf.PI, 0);
    }

    private void RotationMouse(Ray position)
    {
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayLength;

        if (groundPlane.Raycast(position, out rayLength))
        {
            Vector3 pointToLook = position.GetPoint(rayLength);

            character.LookAt(new Vector3(pointToLook.x, character.position.y, pointToLook.z));
        }
    }

    private void Rotation()
    {
        if (!IsPointerOverGameObject(Input.GetTouch(0).fingerId))
        {
            Ray position = cam.ScreenPointToRay(Input.GetTouch(0).position);

            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
            float rayLength;

            if (groundPlane.Raycast(position, out rayLength))
            {
                Vector3 pointToLook = position.GetPoint(rayLength);

                character.LookAt(new Vector3(pointToLook.x, character.position.y, pointToLook.z));
            }
        }

        if (!IsPointerOverGameObject(Input.GetTouch(1).fingerId))
        {
            Ray position = cam.ScreenPointToRay(Input.GetTouch(1).position);

            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
            float rayLength;

            if (groundPlane.Raycast(position, out rayLength))
            {
                Vector3 pointToLook = position.GetPoint(rayLength);

                character.LookAt(new Vector3(pointToLook.x, character.position.y, pointToLook.z));
            }
        }
    }

    private bool IsPointerOverGameObject(int fingerId)
    {
        EventSystem eventSystem = EventSystem.current;
        return (eventSystem.IsPointerOverGameObject(fingerId)
            && eventSystem.currentSelectedGameObject != null);
    }
}
