using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using System;
public class PlayerEntity : Entity
{
    [SerializeField] private float _offsetZ;
    public float catchUpSpeed;
    private Vector3 _default_pos;
    [SerializeField] private Vector3 _ogSize;
    public int currentTier;
    [SerializeField] private SkinnedMeshRenderer _renderer;
    private void Awake() {
        _ogSize = transform.localScale;
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
    // chane appearance base on power level
    public void ChangeAppearance()
    {
        Color32 c;// tier 0 collor
        float sizeIncrease = GetTier() * 0.35f;
        switch (GetTier())
        {
            case 0:
                c = new Color32(151, 156, 156, 255);
                break;
            case 1:
                c = new Color32(0, 173, 255, 255);
                break;
            case 2:
                c = new Color32(231, 227, 45, 255);
                break;
            case 3:
                c = new Color32(171, 45, 231, 255);
                break;
            case 4:
                c = new Color32(241, 49, 44, 255);
                break;
            default:
                c = new Color32(0, 0, 0, 255); ; // bug
                break;
        }
        // if (tiers[1].minPower<=powerLevel && powerLevel<tiers[1].maxPower) 
        // if (tiers[2].minPower<=powerLevel && powerLevel<tiers[2].maxPower)
        // if (tiers[3].minPower<=powerLevel && powerLevel<tiers[3].maxPower)
        // if (powerLevel>=tiers[4].minPower)

        if (_renderer.material.color != c)
        {
            _renderer.material.color = c;
            transform.localScale = _ogSize + new Vector3(sizeIncrease,sizeIncrease,sizeIncrease);
        }
    }
    public object Clone()
    {
        return this.MemberwiseClone();
    }
    public override void Kill()
    {
        powerLevel = 1;
        ChangeAppearance();
        PlayerController.instance.RemoveEntityFromFormation(transform);
        ObjectPooler.instance.DeactivatePooledObject(gameObject);
    }

}
