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
        MULTIPLY,
        MINUS,
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
    public int SpawnPlayerEntity(int calculatedPowerLevel) //small bug not spawning all the entity  | fixed
    {
        int currentPowerLevel = PlayerController.instance.totalPowerLevel;
        int activePlayerEntity = ObjectPooler.instance.ActivePooledObjectCount("PlayerEntity");
        int EntitySpawned = 0;
        for (int i=0; i<calculatedPowerLevel-currentPowerLevel; i++) // check this condition again | fixed
        {
            GameObject playerEntity = ObjectPooler.instance.GetPooledObject("PlayerEntity");
            EntitySpawnPosition entitySpawnPos = PlayerController.instance.GetEntitySpawnPosition();
            if (playerEntity!=null && entitySpawnPos!=null)
            {
                Vector3 pos = entitySpawnPos.position;
                pos.y = playerEntity.transform.localPosition.y;

                playerEntity.transform.parent = PlayerController.instance.formation;
                playerEntity.transform.localPosition = pos; // keep local pos y 

                entitySpawnPos.entity = playerEntity.transform;
                playerEntity.GetComponent<PlayerEntity>().ChangeAppearance();
                playerEntity.SetActive(true);
                EntitySpawned++;
            }
            else return EntitySpawned;
        }
        return EntitySpawned;
    }
    public void DespawnPlayerEntity(int calculatedPowerLevel) // despawn need more refine and tinkering
    {
        int spawnedPlayerEntity = ObjectPooler.instance.ActivePooledObjectCount("PlayerEntity");
        int deSpawnCount = 0;
            foreach(var entityPos in PlayerController.instance.entitySpawnPositions.AsEnumerable().Reverse())
                if (entityPos.entity!=null 
                    && deSpawnCount != (spawnedPlayerEntity-calculatedPowerLevel))
                { 
                    Transform playerEntity =  entityPos.entity;
                    Vector3 pos = playerEntity.transform.localPosition;
                    pos.x = 0; pos.z=0; // shouldn't matter can delete
                    playerEntity.parent = ObjectPooler.instance.transform;
                    playerEntity.localPosition = pos; // keep local pos
                    playerEntity.GetComponent<PlayerEntity>().powerLevel = 1;
                    playerEntity.gameObject.SetActive(false);
                    entityPos.entity=null;
                    deSpawnCount++;
                   
                }    
    }
    public void RollMultiplyPlate()
    {
        // tm.text="";
        number = Random.Range(2,6);
        expression = (Expression) Random.Range(0,2);
        switch (expression)
        {
            case Expression.PLUS:
                tm.text="X+" + number;
                break;
            case Expression.MINUS:
                tm.text="X-" + number;
                break;
            case Expression.MULTIPLY:
                tm.text="X*" + number;
                break;
            case Expression.DIVIDE:
                tm.text="X/" + number;
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
        int EntitySpawned=0;
        if (PlayerController.instance.totalPowerLevel < calculatedPowerLevel)
        {
           EntitySpawned = SpawnPlayerEntity(calculatedPowerLevel);
        }
        if (calculatedPowerLevel<PlayerController.instance.maxUnit
            && calculatedPowerLevel<PlayerController.instance.totalPowerLevel)
        {
            // Debug.Log("despawn player entity");
            DespawnPlayerEntity(calculatedPowerLevel);
        }
        CalculatePowerLevelForEntity(calculatedPowerLevel,PlayerController.instance.totalPowerLevel,EntitySpawned);

        PlayerController.instance.totalPowerLevel = calculatedPowerLevel;
        PlayerController.instance.UpdatePowerLevel();
        Debug.Log("total power level from player controller: " + PlayerController.instance.totalPowerLevel);

    }
    public void CalculatePowerLevelForEntity(int calculatedPowerLevel, int currentPowerLevel, int newSpawn)
    {
        // List<Transform> PlayerEntity = PlayerController.instance.entitySpawnPositions.Where(e => e.childCount>0).ToList<Transform>(); // change this, should get the list ref from object pool
        List<GameObject> PlayerEntity = ObjectPooler.instance.GetActivePoolObjects("PlayerEntity");
        int activePlayerEntity = ObjectPooler.instance.ActivePooledObjectCount("PlayerEntity");
        int offset = calculatedPowerLevel - currentPowerLevel - newSpawn;
     
        // increase
        if (calculatedPowerLevel > currentPowerLevel)
        {
            if (offset > activePlayerEntity)
            {
                int [] plArr = PartitionPowerLevel(offset,activePlayerEntity); 
                    int index = 0;
                    foreach(var entity in PlayerEntity) // get position with player entity
                    {
                        PlayerEntity pe = entity.GetComponent<PlayerEntity>();
                        pe.powerLevel += plArr[index];
                        pe.ChangeAppearance(); // NOTE: move this somewhere else
                        index++;
                    }
            }
            else if  (offset <= activePlayerEntity && activePlayerEntity == PlayerController.instance.maxUnit)
            { 
                PlayerEntity pe = PlayerEntity[Random.Range(0,PlayerEntity.Count)].GetComponent<PlayerEntity>();
                pe.powerLevel+=(offset);
                pe.ChangeAppearance();
            }

      
        // testing 
            if (activePlayerEntity>=10)
            {
                List<PlayerEntity> groupList = 
                    Utility.GetRandomItemsFromList<PlayerEntity>(
                        Utility.ConvertGameObjToSCript<PlayerEntity>(PlayerEntity),
                        Random.Range(3,6)); //... wtf
                bool grouped = GroupPlayerEntity(groupList);
                Debug.Log("group player entity: " + grouped);
            }

            
        }

        // debugging
        Debug.Log("new spawn: " + newSpawn + "\noffset: " + offset + "\n activate player entity: " + activePlayerEntity);
        int countpl = 0;
        foreach (var pe in PlayerEntity)
        {
            countpl += pe.GetComponent<PlayerEntity>().powerLevel;
        }
        Debug.Log("total power level from player entity: " + countpl);
    }
    // partition power level into n chunks where difference between max and min value is minimum
    public int[] PartitionPowerLevel(int powerLevel, int n)
    {
        int k = powerLevel/n;
        int ct = powerLevel % n;
        int i;
        int []arr = new int[n]; 
        for (i = 0; i<ct;i++)
        {
            arr[i] = k+1;
        }
        for (;i<n;i++)
            arr[i]=k;
        return arr;
    }

    public bool GroupPlayerEntity(List<PlayerEntity> entities)
    {
        int sumPower=0;
        int maxTier = 0;
        foreach (var entity in entities)
        {
            sumPower+=entity.powerLevel;
            if (entity.GetTier() > maxTier)
                maxTier = entity.GetTier();
        }
        PlayerEntity temp = ObjectPooler.instance.GetPooledObject("PlayerEntity").GetComponent<PlayerEntity>();
        PlayerEntity rdEntity = entities[Random.Range(0,entities.Count())];

        temp.powerLevel = sumPower;
        if (temp.GetTier()>maxTier)
        {
            Debug.Log("sum power of 3 random entity: "+sumPower);
            temp.ChangeAppearance();
            temp.transform.parent = PlayerController.instance.formation;
            temp.transform.localPosition = rdEntity.transform.localPosition;
            temp.gameObject.SetActive(true);
            foreach (var entity in entities)
            {
                Vector3 pos = entity.transform.localPosition;
                pos.x = 0; pos.z=0;
                entity.transform.parent = ObjectPooler.instance.transform;
                entity.transform.localPosition = pos;
                entity.powerLevel = 1;
                entity.transform.gameObject.SetActive(false);
            }
            return true;
        }
        else temp.powerLevel=1; //reset the power level
        return false;
    }
}
