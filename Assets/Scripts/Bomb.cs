using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utility;
public class Bomb : MonoBehaviour
{
    public Transform _firePoint;
    public Transform target;
    public float time;
    public float speed;
    public float upSpeed;
    public float counter;
    Rigidbody rb;
    public Transform eplodeEffect;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        counter = time;
    }
    private void Update()
    {
    }
    private void FixedUpdate()
    {
        Vector3 direction = target.position - transform.position;
        rb.velocity = direction.normalized * speed;
       
        if (Vector3.Distance(transform.position, target.position) <= 1)
            Explode();
        // if (counter > 0)
        // {
        //     counter -= Time.fixedDeltaTime;
        //     rb.velocity = Vector3.up * upSpeed;

        // }
        // else
        // {
        //     Vector3 direction = target.position - transform.position;
        //     rb.velocity = direction.normalized * speed;
        //     transform.LookAt(target.position);
        //     if (Vector3.Distance(transform.position, target.position) <= 0)
        //         Explode();
        // }

    }
    private void Explode()
    {
        eplodeEffect.gameObject.SetActive(true);
    }

}
