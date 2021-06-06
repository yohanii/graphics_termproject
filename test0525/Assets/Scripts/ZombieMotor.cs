using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieMotor : MonoBehaviour
{
    public Transform player;
    private CharacterController controller;
    private Vector3 moveVector;

    private float plife = 0.0f;
    private float timer = 3.0f;
    private float rotate = 0.0f;
    private float speed = 5.0f;
    private float verticalVelocity = 0.0f;
    private float gravity = 12.0f;
    private float animationDuration = 3.0f;
    private bool isDead = false;
    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        //controller.Move(new Vector3(-2.0f, 0.5f, -3.5f));
        rb = this.GetComponent<Rigidbody>();
        rb.MovePosition(new Vector3(0.0f, 0.0f, 1.5f));


    }

    // Update is called once per frame
    void Update()
    {
        plife = GameObject.Find("Player").GetComponent<PlayerMotor>().life;
        moveVector = rb.velocity;
        Debug.Log(plife);
        Quaternion qRotate = Quaternion.AngleAxis(rotate, Vector3.up);
        transform.position = player.position - qRotate * (new Vector3(0.0f, 0.0f, (3.0f + plife) - timer));
        if (timer < 3.0f)
        {
            timer += 1.0f * Time.deltaTime;
        }

        if (Input.GetKeyDown("a"))
        {
            rotate -= 90.0f;
            transform.Rotate(Vector3.up, -90.0f);
            timer = 0;

        }
        if (Input.GetKeyDown("d"))
        {
            rotate += 90.0f;
            transform.Rotate(Vector3.up, 90.0f);
            timer = 0;
        }

        //if (Time.time < animationDuration)
        //{
        //    moveVector.z = speed;
        //controller.Move(Vector3.forward * speed * Time.deltaTime);
        //    return;
        //}

        //moveVector = Vector3.zero;

        //x - left and right
        //moveVector.x = 0.0f;
        //y - up and down
        //moveVector.y = 0.0f;
        //z - forward and backward
        //moveVector.z = speed;

        //controller.Move(moveVector * Time.deltaTime);

    }
    private void FixedUpdate()
    {
        moveCharacter(moveVector);
    }
    void moveCharacter(Vector3 dir)
    {
        rb.velocity = dir;
    }

}
