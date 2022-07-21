using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RotateEventArgs : EventArgs
{
    public float angle {get; set;}
    public RotateEventArgs(){}
    public RotateEventArgs(float angle)
    {
        this.angle = angle;
        // this.rotatedSideway = rotatedSideway;
    }
}
public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    public float sidewaySpeed;
    public float rotationAngle;
    private bool entityRotatedSideway;
    public static PlayerController instance;
    public event EventHandler<RotateEventArgs> RotateEntity;
    private Vector3 _sidewayDirection;
    public Transform entityPrefab;
    public enum TurningState
    {
        forward,
        left,
        right,
    }
    [SerializeField]private TurningState _turnState; // might become useless
    
    private void Awake() {
        if (instance == null)
            instance = this;
        else {
            Destroy(gameObject);
            return;
        }
        _turnState = TurningState.forward;
        entityRotatedSideway = false;
    }
    
    private void Update() {
        ProcessInput();
    }
    private void FixedUpdate() {
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime,Space.World);
        MoveEntitySideWay();
       
    }
    private void ProcessInput()
    { 
        // BAD
        if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) && _turnState!=TurningState.right) 
        {
            _turnState = TurningState.left;
            _sidewayDirection = Vector3.left;
        }
        else if ((Input.GetKey(KeyCode.D)|| Input.GetKey(KeyCode.RightArrow))  && _turnState!=TurningState.left)
        {
            _turnState = TurningState.right;
            _sidewayDirection = Vector3.right;

        }
        else if (_turnState!=TurningState.forward) //reset rotation
        {
            entityRotatedSideway = false;
            _turnState = TurningState.forward;
            _sidewayDirection = Vector3.zero;
            OnRotateEntity(new RotateEventArgs(0f));

        }
    }
    public void MoveEntitySideWay()
    {
        transform.Translate(_sidewayDirection * sidewaySpeed * Time.deltaTime,Space.World);
        if(_turnState!=TurningState.forward && entityRotatedSideway == false)
        {
            OnRotateEntity(new RotateEventArgs(_sidewayDirection.x*rotationAngle));
            entityRotatedSideway = true;
        }

    }
    protected virtual void OnRotateEntity(RotateEventArgs e)
    {
        // Debug.Log("entity turning event called");
        RotateEntity?.Invoke(this,e);
    }
}