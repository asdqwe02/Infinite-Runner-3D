using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// basic camera script
public class CameraController : MonoBehaviour 
{
    Vector3 offset;
    Transform target;
    // Start
    private void Start() {
        target = PlayerController.instance.transform;
        offset = transform.position - target.position;
    }
    // Update is called once per frame
    void Update()
    {
        transform.position = offset+target.position;
        LevelManager.instance.CheckBoundary(transform);

    }
}
