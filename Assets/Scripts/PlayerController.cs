using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RotateEventArgs : EventArgs
{
    public float angle { get; set; }
    public RotateEventArgs() { }
    public RotateEventArgs(float angle)
    {
        this.angle = angle;
        // this.rotatedSideway = rotatedSideway;
    }
}
public class PlayerController : MonoBehaviour
{
    // public float moveSpeed;
    public float sidewaySpeed;
    public float rotationAngle;
    private bool entityRotatedSideway;
    public static PlayerController instance;
    public event EventHandler<RotateEventArgs> RotateEntity;
    private Vector3 _sidewayDirection;
    public Transform entityPrefab;
    public Transform formation;
    public List<EntitySpawnPosition> entitySpawnPositions;
    public int totalPowerLevel;
    public int maxUnit;
    private TextMesh plText;
    public enum TurningState
    {
        forward,
        left,
        right,
    }
    [SerializeField] private TurningState _turnState; // might become useless

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        _turnState = TurningState.forward;
        entityRotatedSideway = false;
        entitySpawnPositions = new List<EntitySpawnPosition>();
        plText = GetComponentInChildren<TextMesh>();
    }
    private void Start()
    {
        UpdatePowerLevel();
        foreach (var pos in formation.GetComponent<PlayerEntityFomrationSetup>().formation)
            entitySpawnPositions.Add(pos);
        maxUnit = GetComponentInChildren<PlayerEntityFomrationSetup>().FormationUnitCount();
    }
    private void Update()
    {
        ProcessInput();
        LevelManager.instance.CheckBoundary(transform);
    }
    private void FixedUpdate()
    {
        // transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime,Space.World);
        MoveEntitySideWay();

    }
    private void ProcessInput()
    {
        // BAD
        if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) && _turnState != TurningState.right)
        {
            _turnState = TurningState.left;
            _sidewayDirection = Vector3.left;
        }
        else if ((Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) && _turnState != TurningState.left)
        {
            _turnState = TurningState.right;
            _sidewayDirection = Vector3.right;

        }
        else if (_turnState != TurningState.forward) //reset rotation
        {
            entityRotatedSideway = false;
            _turnState = TurningState.forward;
            _sidewayDirection = Vector3.zero;
            OnRotateEntity(new RotateEventArgs(0f));

        }
    }
    public void MoveEntitySideWay()
    {
        transform.Translate(_sidewayDirection * sidewaySpeed * Time.deltaTime, Space.World);


        if (_turnState != TurningState.forward && entityRotatedSideway == false)
        {
            OnRotateEntity(new RotateEventArgs(_sidewayDirection.x * rotationAngle));
            entityRotatedSideway = true;
        }
    }
    protected virtual void OnRotateEntity(RotateEventArgs e)
    {
        // Debug.Log("entity turning event called");
        RotateEntity?.Invoke(this, e);
    }
    public EntitySpawnPosition GetEntitySpawnPosition()
    {
        foreach (var pos in entitySpawnPositions)
            if (pos.entity == null)
                return pos;
        return null;
    }
    public EntitySpawnPosition GetEntitySpawnPosition(Transform entity)
    {
        foreach (var pos in entitySpawnPositions)
            if (pos.entity == entity)
                return pos;
        return null;
    }
    public void UpdatePowerLevel()
    {
        plText.text = totalPowerLevel.ToString();
    }
    public void RemoveEntityFromFormation(Transform entity)
    {
        EntitySpawnPosition esp = GetEntitySpawnPosition(entity);
        if (esp != null)
        {
            esp.entity = null;
        }
    }
    public void ResetAllEntityPowerLevel()
    {
        foreach (var pos in entitySpawnPositions)
        {
            if (pos.entity != null)
            {
                PlayerEntity entity = pos.entity.GetComponent<PlayerEntity>();
                entity.powerLevel = 1;
                entity.ChangeAppearance();
            }
        }
    }
    public void RemoveAllEntity()
    {
        ResetAllEntityPowerLevel();
        foreach (var pos in entitySpawnPositions)
            if (pos.entity != null)
            {
                pos.entity = null;
            }
        ObjectPooler.instance.RemoveAllObjectWithTag("PlayerEntity");
    }

}