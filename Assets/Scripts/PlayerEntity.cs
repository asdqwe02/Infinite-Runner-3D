using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class PlayerEntity : Entity
{
    [SerializeField] private float _offsetZ;
    public float catchUpSpeed;
    Vector3 default_pos;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        PlayerController.instance.RotateEntity+=Rotate;
        default_pos = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        LevelManager.instance.CheckBoundary(transform);
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
    private void FixedUpdate() {
        if (transform.localPosition!=default_pos)
           CatchUp();
    }
    private void CatchUp()
    {
        Vector3 target_pos = default_pos-transform.localPosition;
        transform.Translate(target_pos*catchUpSpeed*Time.deltaTime);
    }
}
