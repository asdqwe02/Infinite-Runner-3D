using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utility;
public class EnemyEntity : Entity
{
    // Start is called before the first frame update
    public Transform target;
    public float speed;
    private Animator animator;
    public bool hit; // could be useless
    protected void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
        hit = false;
    }
    void FixedUpdate()
    {
        if (target != null && target.gameObject.activeSelf)
        {
            Vector3 pos = target.position;
            pos.y=0;
            transform.LookAt(pos);
            Vector3 direction = (target.position - transform.position).normalized;
            direction.y = 0;
            transform.Translate(direction * speed * Time.deltaTime, Space.World);
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("PlayerEntity") && !hit)
        {
            hit = true;
            PlayerEntity pe = other.collider.GetComponent<PlayerEntity>();
            pe.TakeDamage(powerLevel);
            if (pe.powerLevel <= 0)
            {
               
                target = null;
                pe.Kill();
            }
            else
            {
                pe.ChangeAppearance();
            }
            PlayerController.instance.UpdatePowerLevel();
            Kill();
        }
    }
    public override void Kill()
    {
        if (target!=null)
            target = null;
        GameManager.instance.AddScore(10);
        ParticleExplode();
        base.Kill();
    }
    private void OnEnable() {
        hit = false;
        target = null;
        animator.SetBool("Running", false);

    }
    public void SetTarget()
    {
        List<EntitySpawnPosition> playerEntityPos = GetSpawnPositionWithEntity(PlayerController.instance.entitySpawnPositions);
        target = playerEntityPos[Random.Range(0, playerEntityPos.Count - 1)].entity;
        animator.SetBool("Running", true);
    }
}