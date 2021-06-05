using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class obs_script : MonoBehaviour
{
    public float pushStrength = 6.0f;
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;
        if(body == null || body.isKinematic)
        {
            return;
        }
        if(hit.moveDirection.y < -0.3f)
        {
            return;
        }
        Vector3 Direction = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
        body.velocity = Direction * pushStrength;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
