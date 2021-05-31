using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 moveVector;

    private float speed = 5.0f;
    private float verticalVelocity = 0.0f;
    private float gravity = 12.0f;
    private float animationDuration = 3.0f;
    private bool isDead = false;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
            return;

        if(Time.time < animationDuration)
        {
            controller.Move(Vector3.forward * speed * Time.deltaTime);
            return;
        }

        moveVector = Vector3.zero;

        if(controller.isGrounded)
        {
            verticalVelocity = -0.5f;
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }

        //x - left and right
        moveVector.x = Input.GetAxisRaw("Horizontal") * speed;
        //y - up and down
        moveVector.y = verticalVelocity;
        //z - forward and backward
        moveVector.z = speed;

        controller.Move(moveVector * Time.deltaTime);
    }

    //It is begin called every time our capsule hits something
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.point.z > transform.position.z + controller.radius && hit.point.y> transform.position.y+ controller.height/4)
            Death();
    }

    private void Death()
    {
        isDead = true;
    }

}
