using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    public bool stop = false;
    public int tile_on;
    public int tile_on_type;
    private float tile_enter_time;
    private int last_tile_idx;
    private CharacterController controller;
    private Vector3 moveVector;

    public float life;
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
        life = 4.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
        if (Input.GetKeyDown(KeyCode.Space) == true)
        {
            stop = !stop;
            rb.isKinematic = stop;
        }

        if (life <= 1.5f)
        {
            Death();
        }


        if (isDead)
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
            if (Time.time - tile_enter_time > 0.4)
            {
                cm = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMotor>();
                // animation rotating camera  end at 1.0f, and  wait additional 2.0f;
                if (cm.rot_transition > 2.0f)
                {
                    Debug.Log("tile_on_type : " + tile_on_type + " cm.dx : " + cm.dx);
                    if (cm.dx > 1.5f)
                    {
                        //right
                        transform.Rotate(Vector3.up, 90.0f);
                    }
                    else if (cm.dx < -1.5f)
                    {
                        //left
                        transform.Rotate(Vector3.up, -90.0f);
                        
                    }
                }
            }
        }


        moveVector = (transform.forward * speed + transform.right * Input.GetAxisRaw("Horizontal") * speed);

        //controller.Move(moveVector * Time.deltaTime);

    }
    private void FixedUpdate()
    {
        if (stop)
            moveCharacter(Vector3.zero);
        else
            moveCharacter(moveVector);
    }
    void moveCharacter(Vector3 dir)
    {
        //dir.y = rb.velocity.y;
        dir.y = -Mathf.Abs(rb.velocity.y);
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

        /*if (collision.gameObject.tag == "Enemy")
        {
            Debug.Log("life--");
            life -= 0.5f;
        }*/
        if (collision != null)
        {
            if (collision.gameObject.tag == "Zombie")
            {
                if (Vector3.Distance(collision.transform.position, this.transform.position) < 1.1)
                {
                    life = 0.0f;
                }
            }
        }
    }

    private void Death()
    {
        isDead = true;
        GetComponent<Score>().OnDeath();
    }

}
