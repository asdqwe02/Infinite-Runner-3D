using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Power level text controller
public class PLTextController : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.forward = Camera.main.transform.forward;
    }
}
