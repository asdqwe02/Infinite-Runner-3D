using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class PlayerEntity : Entity
{
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        PlayerController.instance.RotateEntity+=Rotate;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void Rotate(object sender, RotateEventArgs rotateInfo)
    {
        if (rotateInfo.angle == 0)
        {
            transform.rotation = Quaternion.identity;
            return;
        }
        transform.Rotate(new Vector3(0,rotateInfo.angle,0),Space.World);

    }
}
