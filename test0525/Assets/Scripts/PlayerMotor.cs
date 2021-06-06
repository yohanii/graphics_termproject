using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    public int tile_on;
    public int tile_on_type;
    private float tile_enter_time;
    private int last_tile_idx;
    private CharacterController controller;
    private Vector3 moveVector;


    private float speed = 5.0f;
    private float verticalVelocity = 0.0f;
    private float gravity = 12.0f;
    private float animationDuration = 3.0f;
    private bool isDead = false;

    private Rigidbody rb;
    private CameraMotor cm;

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        controller = GetComponent<CharacterController>();
        //controller.Move(new Vector3(0.0f,0.2f, 0.0f));
        //move to abs position
        rb.MovePosition(new Vector3(0.0f, 0.0f, 5.0f));
        last_tile_idx = -1;
    }

    // Update is called once per frame
    void Update()
    {
        if(isDead)
           return;

        //moveVector = rb.velocity;
        
        if (Time.time < animationDuration)
        {
            moveVector = transform.forward * speed;
            //moveVector.z = speed;
            //moveVector = Vector3.forward;
            //controller.Move(Vector3.forward * speed * Time.deltaTime);
            return;
        }

        //moveVector = Vector3.zero;

        /*if(controller.isGrounded)
        {
            verticalVelocity = -0.5f;
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }*/


        //x - left and right
        //moveVector.x = Input.GetAxisRaw("Horizontal") * speed;
        //y - up and down
        //moveVector.y = 0.0f;
        //z - forward and backward
        //moveVector.z = speed;

        if (Input.GetKeyDown("a"))
        {
            transform.Rotate(Vector3.up, -90.0f);
        }
        if (Input.GetKeyDown("d"))
        {
            transform.Rotate(Vector3.up, 90.0f);
        }
        if(tile_on_type == 4 || tile_on_type == 5)
        {
            if (Time.time - tile_enter_time > 0.3)
            {
                cm = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMotor>();
                // animation rotating camera  end at 1.0f, and  wait additional 2.0f;
                if (cm.rot_transition > 2.0f)
                {
                    Debug.Log("tile_on_type : " + tile_on_type + " cm.dx : " + cm.dx);
                    if (cm.dx > 2.0f)
                    {
                        //left
                        transform.Rotate(Vector3.up, -90.0f);
                    }
                    else if (cm.dx < -2.0f)
                    {
                        //right
                        transform.Rotate(Vector3.up, 90.0f);
                    }
                }
            }
        }
        

        moveVector = transform.forward * speed + transform.right * Input.GetAxisRaw("Horizontal")* speed;

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
    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("collision Enter : " + collision.transform.parent.parent.name);
        //go -> floor
        // parent -> profile    
        // parent.parent -> Tile_Normal
        if (collision.transform.parent != null)
        {
            if (collision.transform.parent.parent != null)
            {
                if (collision.transform.parent.parent.CompareTag("Tile"))
                {
                    Tile_variable tv = collision.transform.parent.parent.GetComponent<Tile_variable>();
                    tile_on = tv.tile_idx;
                    tile_on_type = tv.tile_type;
                    if(tile_on != last_tile_idx)
                    {
                        tile_enter_time = Time.time;
                    }
                    //Debug.Log("tile_on : " + tile_on);
                    last_tile_idx = tile_on;
                }
            }
        }
    }

    //It is begin called every time our capsule hits something
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(hit == null)
        {
            return;
        }
        
        

        if (hit.point.z > transform.position.z + 0.1f && hit.gameObject.tag=="Enemy")
             Death();

    }

    private void Death()
    {
        isDead = true;
        GetComponent<Score>().OnDeath();
    }

}
