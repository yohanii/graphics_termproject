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

    public int tile_on;
    public int tile_on_type;
    private float tile_enter_time;
    private int last_tile_idx;
    private int turned;
    private float rotation_timer;

    public float dist = 4.5f;
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
        /*plife = GameObject.Find("Player").GetComponent<PlayerMotor>().life;
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
        }*/

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
        
        /*
        if ((Time.time - tile_enter_time) < 0.1f)
        {
            turned = 0;
        }

        if ((Time.time - tile_enter_time)  > 0.7f)
        {
            if (turned == 0)
            {
                

                if (tile_on_type == 4)
                {
                    transform.Rotate(Vector3.up, -45.0f);
                }
                else if (tile_on_type == 5)
                {
                    transform.Rotate(Vector3.up, 45.0f);
                }
                
                turned = 1;
            }
        }
        */

        if(rotation_timer > 0.5f)
        {

            Vector3 target_vec = (player.position - transform.position);
            /*float angle_diff = Vector3.Angle(transform.forward, target_vec);
            Vector3 cross = Vector3.Cross(Vector3.forward, target_vec);
            if (cross.y < 0) angle_diff = -angle_diff;
            transform.Rotate(Vector3.up, angle_diff);
            */
            transform.LookAt(player.position);
            Debug.Log("dist : " + Vector3.Magnitude(target_vec));
            if (Vector3.Magnitude(target_vec) < dist)
            {
                speed = 4.0f;
            }
            else
            {
                speed = 5.5f;
            }
            rotation_timer = 0.0f;
        }

        rotation_timer += Time.deltaTime;


        moveVector = transform.forward * speed;


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
                    if (tile_on != last_tile_idx)
                    {
                        tile_enter_time = Time.time;
                    }
                    //Debug.Log("tile_on : " + tile_on + " tile_on_type : " + tile_on_type);
                    last_tile_idx = tile_on;
                }
            }
        }
    }
    }
