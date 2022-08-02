using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utility;

public class PlayerEntity : Entity
{
    [SerializeField] private float _offsetZ;
    public float catchUpSpeed;
    private Animator animator;
    private Vector3 _default_pos;
    public int currentTier; // for debugging only
    public Transform laserTarget;
    public Transform laserPoint;
    
    [Range(3.5f, 5.5f)]
    public float flyUpYPosition;
    [Range(-65f,-90f)]
    public float flyUpRotationX;
    public bool flying;
  
    // is this even necessary ?
    protected void Awake() { 
        base.Awake();
        animator = GetComponent<Animator>();
    
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        PlayerController.instance.RotateEntity += RotateEntity;
        _default_pos = transform.localPosition;
        foreach(Transform child in transform)
        {
            laserTarget = child.Find("Target");
            if (laserTarget!=null)
                break;
        }
           
        // _renderer = GetComponentInChildren<SkinnedMeshRenderer>(); // don't use this
    }

    // Update is called once per frame
    void Update()
    {
        currentTier = GetTier();
        LevelManager.instance.CheckBoundary(transform);
    }
    private void RotateEntity(object sender, RotateEventArgs rotateInfo)
    {
        if (rotateInfo.angle == 0)
        {
            transform.rotation = Quaternion.identity;
            return;
        }
        transform.Rotate(new Vector3(0, rotateInfo.angle, 0), Space.World);
    }
    private void FixedUpdate()
    {
        if (transform.localPosition != _default_pos)
            CatchUp();
    }
    private void CatchUp()
    {
        Vector3 target_pos = _default_pos - transform.localPosition;
        transform.Translate(target_pos * catchUpSpeed * Time.deltaTime);
    }
    
    public override void Kill()
    {
        powerLevel = 1;
        ChangeAppearance();
        PlayerController.instance.RemoveEntityFromFormation(transform);
        ObjectPooler.instance.DeactivatePooledObject(gameObject);
    }
    public void FlyUp()
    {
        flying = true;
        flyUpYPosition = UnityEngine.Random.Range(3.5f, 5.5f); // arbitrary
        _default_pos.y = flyUpYPosition;
        // transform.Rotate(new Vector3(flyUpRotationX, 0, 0), Space.World); 
        animator.SetBool("IsFlying",true);
        animator.SetBool("Running",false);
    }
    public void BackToGround()
    {
        flying = false;
        // transform.rotation = Quaternion.identity;
        _default_pos.y = -PlayerController.instance.transform.position.y;
        animator.SetBool("IsFlying",false);
        animator.SetBool("Running",true);
    }
    private void OnDisable() {
        BackToGround();
  
    }
}
