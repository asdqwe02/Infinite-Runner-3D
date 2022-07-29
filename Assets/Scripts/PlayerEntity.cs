using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using System;
public class PlayerEntity : Entity
{
    [SerializeField] private float _offsetZ;
    public float catchUpSpeed;
    private Vector3 _default_pos;
    public int currentTier; // for debugging only

    // is this even necessary ?
    protected void Awake() { 
        base.Awake();
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        PlayerController.instance.RotateEntity += Rotate;
        _default_pos = transform.localPosition;
        
        // _renderer = GetComponentInChildren<SkinnedMeshRenderer>(); // don't use this
    }

    // Update is called once per frame
    void Update()
    {
        currentTier = GetTier();
        LevelManager.instance.CheckBoundary(transform);
    }
    private void Rotate(object sender, RotateEventArgs rotateInfo)
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
    
    // change appearance base on power level
    // public override void ChangeAppearance()
    // {
    //     float sizeIncrease = GetTier() * 0.35f;
    //     if (Renderer.material.color != tiers[GetTier()].color)
    //     {
    //         Renderer.material.color = tiers[GetTier()].color;
    //         transform.localScale = OGSize + new Vector3(sizeIncrease,sizeIncrease,sizeIncrease);
    //     }
    // }
    public override void Kill()
    {
        powerLevel = 1;
        ChangeAppearance();
        PlayerController.instance.RemoveEntityFromFormation(transform);
        ObjectPooler.instance.DeactivatePooledObject(gameObject);
    }

}
