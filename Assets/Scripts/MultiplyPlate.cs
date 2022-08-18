using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;
using static Utility;
using TMPro;
public class MultiplyPlate : MonoBehaviour
{
    // Start is called before the first frame update
    public int number;
    public TextMeshPro tm;
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
        tm = GetComponentInChildren<TextMeshPro>();
        RollMultiplyPlate();
        trigger = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            trigger = true;
        }
    }
    public void RollMultiplyPlate()
    {
        // tm.text="";
        number = Random.Range(2, 6);
        expression = (Expression)Random.Range(0, 4);
        switch (expression)
        {
            case Expression.PLUS:
                tm.text = "X+" + number;
                break;
            case Expression.MINUS:
                tm.text = "X-" + number;
                break;
            case Expression.MULTIPLY:
                tm.text = "X*" + number;
                break;
            case Expression.DIVIDE:
                tm.text = "X/" + number;
                break;
            default:
                Debug.Log("Error in multiply plate");
                break;
        }
    }
    public int CalculatePowerLevel()
    {
        int current_pl = PlayerController.instance.totalPowerLevel;
        switch (expression)
        {
            case Expression.PLUS:
                return current_pl + number;
            case Expression.MINUS:
                if (current_pl - number <= 0)
                    return 1;
                return current_pl - number;
            case Expression.MULTIPLY:
                return current_pl * number;
            case Expression.DIVIDE:
                if (current_pl / number <= 0)
                    return 1;
                return current_pl / number;
        }
        return 0; // => bug if got this
    }
    public int SpawnPlayerEntity(int calculatedPowerLevel) //small bug not spawning all the entity  | fixed
    {
        int currentPowerLevel = PlayerController.instance.totalPowerLevel;
        int activePlayerEntity = ObjectPooler.instance.ActivePooledObjectCount("PlayerEntity");
        int EntitySpawned = 0;
        for (int i = 0; i < calculatedPowerLevel - currentPowerLevel; i++) // check this condition again | fixed
        {
            GameObject playerEntity = ObjectPooler.instance.GetPooledObject("PlayerEntity");
            EntitySpawnPosition entitySpawnPos = PlayerController.instance.GetEntitySpawnPosition();
            if (playerEntity != null && entitySpawnPos != null)
            {
                Vector3 pos = entitySpawnPos.position;
                pos.y = playerEntity.transform.localPosition.y;

                playerEntity.transform.parent = PlayerController.instance.formation;
                playerEntity.transform.localPosition = pos; // keep local pos y 

                entitySpawnPos.entity = playerEntity.transform;
                playerEntity.SetActive(true);
                playerEntity.GetComponent<PlayerEntity>().ChangeAppearance();
                EntitySpawned++;
            }
            else return EntitySpawned;
        }
        return EntitySpawned;
    }
    public void SplitPlayerEntity(int calculatedPowerLevel, int activePlayerEntity) // despawn need more refine and tinkering
    {
        PlayerController.instance.RemoveAllEntity();
        for (int i = 0; i < calculatedPowerLevel; i++)
        {
            GameObject playerEntity = ObjectPooler.instance.GetPooledObject("PlayerEntity");
            EntitySpawnPosition entitySpawnPos = PlayerController.instance.GetEntitySpawnPosition();
            if (playerEntity != null && entitySpawnPos != null)
            {
                Vector3 pos = entitySpawnPos.position;
                pos.y = playerEntity.transform.localPosition.y;

                playerEntity.transform.parent = PlayerController.instance.formation;
                playerEntity.transform.localPosition = pos; // keep local pos y 

                entitySpawnPos.entity = playerEntity.transform;
                playerEntity.SetActive(true);
                playerEntity.GetComponent<PlayerEntity>().ChangeAppearance();
            }
        }
    }

    public void Activate()
    {
        int calculatedPowerLevel = CalculatePowerLevel();
        // LevelManager.instance.counter++;
        int EntitySpawned = 0;
        if (PlayerController.instance.totalPowerLevel < calculatedPowerLevel)
        {
            EntitySpawned = SpawnPlayerEntity(calculatedPowerLevel);
        }
        // if (calculatedPowerLevel < PlayerController.instance.maxUnit
        //     && calculatedPowerLevel < PlayerController.instance.totalPowerLevel) // look at this again this cause bug
        // {
        //     // Debug.Log("despawn player entity");
        //     DespawnPlayerEntity(calculatedPowerLevel);
        // }
        CalculatePowerLevelForEntity(calculatedPowerLevel, PlayerController.instance.totalPowerLevel, EntitySpawned);

        PlayerController.instance.totalPowerLevel = calculatedPowerLevel;
        PlayerController.instance.UpdatePowerLevel();
    }
    public void CalculatePowerLevelForEntity(int calculatedPowerLevel, int currentPowerLevel, int newSpawn)
    {
        // List<Transform> PlayerEntity = PlayerController.instance.entitySpawnPositions.Where(e => e.childCount>0).ToList<Transform>(); // change this, should get the list ref from object pool
        List<GameObject> PlayerEntity = ObjectPooler.instance.GetActivePoolObjects("PlayerEntity");
        int activePlayerEntity = ObjectPooler.instance.ActivePooledObjectCount("PlayerEntity");
        int offset = Mathf.Abs(calculatedPowerLevel - currentPowerLevel - newSpawn);

        // increase
        if (calculatedPowerLevel > currentPowerLevel)
        {
            if (offset > activePlayerEntity)
            {
                int[] plArr = PartitionPowerLevel(offset, activePlayerEntity);
                int index = 0;
                foreach (var entity in PlayerEntity) // get position with player entity
                {
                    PlayerEntity pe = entity.GetComponent<PlayerEntity>();
                    pe.powerLevel += plArr[index];
                    pe.ChangeAppearance(); // NOTE: move this somewhere else
                    index++;
                }
            }
            else if (offset <= activePlayerEntity && activePlayerEntity == PlayerController.instance.maxUnit)
            {
                PlayerEntity pe = PlayerEntity[Random.Range(0, PlayerEntity.Count)].GetComponent<PlayerEntity>();
                pe.powerLevel += (offset);
                pe.ChangeAppearance();
            }

            // grouping NOTE: Still have small bug that might break the power level calculation
            if (activePlayerEntity >= 10)
            {
                List<PlayerEntity> groupList =
                    Utility.GetRandomItemsFromList<PlayerEntity>(
                        Utility.ConvertGameObjsToSCript<PlayerEntity>(PlayerEntity),
                        Random.Range(3, 6)); //... wtf
                bool grouped = GroupPlayerEntity(groupList);
                // Debug.Log("group player entity: " + grouped); // debug
            }
        }

        // decrease
        if (calculatedPowerLevel < currentPowerLevel)
        {
            if (calculatedPowerLevel < PlayerController.instance.maxUnit)
            {
                SplitPlayerEntity(calculatedPowerLevel, activePlayerEntity);
                return;
            }
            if (offset > activePlayerEntity)
            {
                int[] plArr = PartitionPowerLevel(offset, activePlayerEntity);
                int index = 0;
                List<PlayerEntity> killList = new List<PlayerEntity>();
                foreach (var entity in PlayerEntity)
                {
                    PlayerEntity pe = entity.GetComponent<PlayerEntity>();
                    pe.powerLevel -= plArr[index];
                    pe.ChangeAppearance(); // NOTE: move this somewhere else
                    index++;
                    if (pe.powerLevel <= 0)
                    {
                        killList.Add(pe);
                        int pld = Mathf.Abs(pe.powerLevel);
                        if (index != plArr.Length)
                        {
                            plArr[index] += pld;
                        }
                        else
                        {
                            while (pld > 0)
                            {
                                // Debug.Log("loop offset > active");
                                PlayerEntity rdEntity = PlayerEntity[Random.Range(0, PlayerEntity.Count)].GetComponent<PlayerEntity>();
                                if (killList.Contains(rdEntity))
                                    continue;
                                rdEntity.powerLevel -= pld;
                                rdEntity.ChangeAppearance();// NOTE: move this somewhere else
                                if (rdEntity.powerLevel <= 0)
                                {
                                    pld = Mathf.Abs(rdEntity.powerLevel);
                                    killList.Add(rdEntity);
                                }
                                else break;
                            }

                        }
                    }

                }
                if (killList.Count > 0)
                {
                    foreach (var e in killList)
                        e.Kill();
                }
            }
            if (offset < activePlayerEntity && calculatedPowerLevel >= PlayerController.instance.maxUnit)
            {
                int pld = offset;
                List<PlayerEntity> killList = new List<PlayerEntity>();

                /* 3 outcomes: 
                * power level - pdl > 0 -> break
                * power level - pdl < 0 -> pld = Mathf.Abs(power level - pdl)
                * power level - pdl = 0 -> break
                *
                */
                while (pld > 0)
                {
                    // Debug.Log("loop offset < active");
                    PlayerEntity rdEntity = PlayerEntity[Random.Range(0, PlayerEntity.Count)].GetComponent<PlayerEntity>();
                    if (killList.Contains(rdEntity))
                        continue;
                    rdEntity.powerLevel -= pld;
                    rdEntity.ChangeAppearance(); // NOTE: move this somewhere else
                    if (rdEntity.powerLevel <= 0)
                    {
                        pld = Mathf.Abs(rdEntity.powerLevel);
                        killList.Add(rdEntity);
                    }
                    else break;
                }
                if (killList.Count > 0)
                {
                    foreach (var e in killList)
                        e.Kill();
                }
            }

        }

        // debugging
        // Debug.Log("new spawn: " + newSpawn + "\noffset: " + offset + "\n activate player entity: " + activePlayerEntity);
    }
    // partition power level into n chunks where difference between max and min value is minimum

    public bool GroupPlayerEntity(List<PlayerEntity> entities)
    {
        int sumPower = 0;
        int maxTier = 0;
        foreach (var entity in entities)
        {
            sumPower += entity.powerLevel;
            if (entity.GetTier() > maxTier)
                maxTier = entity.GetTier();
        }
        // PlayerEntity temp = ObjectPooler.instance.GetPooledObject("PlayerEntity").GetComponent<PlayerEntity>();
        PlayerEntity rdEntity = entities[Random.Range(0, entities.Count())];
        int tempPL = rdEntity.powerLevel;
        rdEntity.powerLevel = sumPower;
        if (rdEntity.GetTier() > maxTier)
        {
            // Debug.Log("sum power of 3 random entity: " + sumPower);
            rdEntity.ChangeAppearance();
            entities.Remove(rdEntity);
            foreach (var entity in entities)
            {
                entity.powerLevel = 1;
                PlayerController.instance.RemoveEntityFromFormation(entity.transform);
                ObjectPooler.instance.DeactivatePooledObject(entity.gameObject);

            }
            return true;
        }
        else rdEntity.powerLevel = tempPL; //return to the old power level
        return false;

    }
}
