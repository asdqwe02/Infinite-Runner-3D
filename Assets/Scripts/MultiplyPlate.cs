using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
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
    public bool trigger;
    void Start()
    {
        tm = GetComponentInChildren<TextMesh>();
        RollMultiplyPlate();
        trigger = false;
    }
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player"))
        {
            trigger = true;
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
    public void SpawnPlayerEntity(int calculatedPowerLevel) //small bug not spawning all the entity  | fixed
    {
        int currentPowerLevel = PlayerController.instance.totalPowerLevel;
        calculatedPowerLevel = CalculatePowerLevel();
        int activePlayerEntity = ObjectPooler.instance.GetActivePooledObject("PlayerEntity");

        for (int i=0; i<calculatedPowerLevel-currentPowerLevel; i++) // check this condition again | fixed
        {
            GameObject playerEntity = ObjectPooler.instance.GetPooledObject("PlayerEntity");
            Transform entitySpawnPos = PlayerController.instance.GetEntitySpawnPosition();
            if (playerEntity!=null && entitySpawnPos!=null)
            {
                Vector3 pos = playerEntity.transform.localPosition;
                playerEntity.transform.parent = entitySpawnPos;
                playerEntity.transform.localPosition = pos; // keep local pos
                playerEntity.GetComponent<PlayerEntity>().ChangeAppearance();
                playerEntity.SetActive(true);
            }
            else return;
        }
    }
    public void DespawnPlayerEntity(int calculatedPowerLevel) // despawn need more refine and tinkering
    {
        int spawnedPlayerEntity = ObjectPooler.instance.GetActivePooledObject("PlayerEntity");
        int deSpawnCount = 0;
            foreach(Transform entityPos in PlayerController.instance.entitySpawnPositions.AsEnumerable().Reverse())
                if (entityPos.childCount>0 
                    && deSpawnCount != (spawnedPlayerEntity-calculatedPowerLevel))
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
        number = Random.Range(2,6);
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
    public void Activate()
    {
        int calculatedPowerLevel = CalculatePowerLevel();
        LevelManager.instance.counter++;
        LevelManager.instance.levelPassed++;
        if (PlayerController.instance.totalPowerLevel < calculatedPowerLevel)
        {
            SpawnPlayerEntity(calculatedPowerLevel);
        }
        if (calculatedPowerLevel<PlayerController.instance.entitySpawnPositions.Count
            && calculatedPowerLevel<PlayerController.instance.totalPowerLevel)
        {
            Debug.Log("despawn player entity");
            DespawnPlayerEntity(calculatedPowerLevel);
        }
        CalculatePowerLevelForEntity(calculatedPowerLevel,PlayerController.instance.totalPowerLevel);
        PlayerController.instance.totalPowerLevel = calculatedPowerLevel;
        PlayerController.instance.UpdatePowerLevel();
    }
    public void CalculatePowerLevelForEntity(int calculatedPowerLevel, int currentPowerLevel)
    {
        // increasse
        if (calculatedPowerLevel > currentPowerLevel)
        {
            if (calculatedPowerLevel - currentPowerLevel < ObjectPooler.instance.GetActivePooledObject("PlayerEntity"))
            {
                int [] plArr = PartitionPowerLevel(calculatedPowerLevel,ObjectPooler.instance.GetActivePooledObject("PlayerEntity")); 
                int index = 0;
                foreach(Transform entity in PlayerController.instance.entitySpawnPositions.Where( e => e.childCount>0)) // get position with player entity
                {
                    PlayerEntity pe = entity.GetChild(0).GetComponent<PlayerEntity>();
                    pe.powerLevel += plArr[index];
                    pe.ChangeAppearance();
                    index++;
                }
            }
        }
    }
    // partition power level into n chunks where difference between max and min value is minimum
    public int[] PartitionPowerLevel(int powerLevel, int n)
    {
        int k = powerLevel/n;
        int ct = powerLevel % n;
        int i;
        int []arr = new int[n]; 
        for (i =1; i<ct;i++)
        {
            arr[i] = k+1;
        }
        for (;i<n;i++)
            arr[i]=k;
        return arr;
    }
}
