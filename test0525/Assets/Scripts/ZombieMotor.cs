using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieMotor : MonoBehaviour
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
        controller.Move(new Vector3(-2.0f, 0.1f, -4.0f));

    }

    // Update is called once per frame
    void Update()
    {
        //if (isDead)
        //   return;

        if (Time.time < animationDuration)
        {
            controller.Move(Vector3.forward * speed * Time.deltaTime);
            return;
        }

        moveVector = Vector3.zero;

        if (controller.isGrounded)
        {
            verticalVelocity = -0.5f;
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }

        //x - left and right
        moveVector.x = 0.0f;
        //y - up and down
        moveVector.y = 0.0f;
        //z - forward and backward
        moveVector.z = speed;

        controller.Move(moveVector * Time.deltaTime);

    }


}
