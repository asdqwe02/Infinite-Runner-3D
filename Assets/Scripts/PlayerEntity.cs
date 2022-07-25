using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using System;
public class PlayerEntity : Entity
{
    [SerializeField] private float _offsetZ;
    public float catchUpSpeed;
    private Vector3 _default_pos;
    [SerializeField] private SkinnedMeshRenderer _renderer;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        PlayerController.instance.RotateEntity+=Rotate;
        _default_pos = transform.localPosition;
        _renderer = GetComponentInChildren<SkinnedMeshRenderer>();
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
        if (transform.localPosition!=_default_pos)
           CatchUp();
    }
    private void CatchUp()
    {
        Vector3 target_pos = _default_pos-transform.localPosition;
        transform.Translate(target_pos*catchUpSpeed*Time.deltaTime);
    }
    // chane appearance base on power level
    public void ChangeAppearance()
    {
        Color32 c = new Color32(151,156,156,255);
        if ( 5<=powerLevel && powerLevel<25)
            c = new Color32(0,173,255,255);
        if ( 25<=powerLevel && powerLevel<50)
            c = new Color32(231, 227, 45,255);
        if ( 50<=powerLevel && powerLevel<100)
            c= new Color32(171, 45, 231,255);
        if ( powerLevel>=100)
            c = new Color(241, 49, 44,255);
            
        if (_renderer.material.color!=c)
            _renderer.material.color = c;
    }
}
