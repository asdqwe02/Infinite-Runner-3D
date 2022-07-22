using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplyPlate : MonoBehaviour
{
    // Start is called before the first frame update
    public int number;
    public TextMesh tm;
    public enum Expression
    {
        PLUS,
        MINUS,
        MULTIPLY,
        DIVIDE,
    }
    public Expression expression;

    void Start()
    {
        tm = GetComponentInChildren<TextMesh>();
        RollMultiplyPlate();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player"))
        {
            LevelManager.instance.counter++;
            LevelManager.instance.levelPassed++;
            if (PlayerController.instance.totalPowerLevel < CalculatePowerLevel())
            {
                SpawnPlayerEntity();
            }
            if (CalculatePowerLevel()<PlayerController.instance.entitySpawnPositions.Count
                && CalculatePowerLevel()<PlayerController.instance.totalPowerLevel)
            {
                Debug.Log("despawn player entity");
                DespawnPlayerEntity();
            }
            PlayerController.instance.totalPowerLevel = CalculatePowerLevel();
            PlayerController.instance.UpdatePowerLevel();
        }
    }
    public int CalculatePowerLevel()
    {
        int current_pl = PlayerController.instance.totalPowerLevel;
        switch (expression)
        {
            case Expression.PLUS:
                return current_pl+number;
            case Expression.MINUS:
                if (current_pl-number<=0)
                    return 1;
                return current_pl-number;
            case Expression.MULTIPLY:
                return current_pl*number;
            case Expression.DIVIDE:
              if (current_pl/number<=0)
                    return 1;
                return current_pl/number;
        }
        return 0; // => bug if got this
    }
    public void SpawnPlayerEntity() //small bug not spawning all the entity
    {
        int powerLevel = PlayerController.instance.totalPowerLevel;
        for (int i=0; i<CalculatePowerLevel()-powerLevel; i++) // check this condition again
        {
            GameObject playerEntity = ObjectPooler.instance.GetPooledObject("PlayerEntity");
            Debug.Log("SpawnPlayerEntity");
            Debug.Log(playerEntity);
            Debug.Log(ObjectPooler.instance.GetActivePooledObject("PlayerEntity"));
            if (playerEntity!=null)
            {
                Vector3 pos = playerEntity.transform.localPosition;
                playerEntity.transform.parent = PlayerController.instance.GetEntitySpawnPosition();
                playerEntity.transform.localPosition = pos; // keep local pos
                playerEntity.SetActive(true);
            }
            else return;
        }
    }
    public void DespawnPlayerEntity() // despawn need more refine and tinkering
    {
        int spawnedPlayerEntity = ObjectPooler.instance.GetActivePooledObject("PlayerEntity");
        int deSpawnCount = 0;
            foreach(Transform entityPos in PlayerController.instance.entitySpawnPositions)
                if (entityPos.childCount>0 
                    && deSpawnCount != (spawnedPlayerEntity-CalculatePowerLevel()))
                { 
                    Transform playerEntity =  entityPos.GetChild(0);
                    Vector3 pos = playerEntity.transform.localPosition;
                    playerEntity.parent = ObjectPooler.instance.transform;
                    playerEntity.localPosition = pos; // keep local pos
                    playerEntity.gameObject.SetActive(false);
                    deSpawnCount++;
                   
                }    
    }
    
    public void RollMultiplyPlate()
    {
        tm.text="";
        number = Random.Range(1,8);
        expression = (Expression) Random.Range(0,4);
        switch (expression)
        {
            case Expression.PLUS:
                tm.text+="X+" + number;
                break;
            case Expression.MINUS:
                tm.text+="X-" + number;
                break;
            case Expression.MULTIPLY:
                tm.text+="X*" + number;
                break;
            case Expression.DIVIDE:
                tm.text+="X/" + number;
                break;
            default:
                Debug.Log("Error in multiply plate");
                break;
        }
    }
}
