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
    public TextMeshPro textMesh;
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
        textMesh = GetComponentInChildren<TextMeshPro>();
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
                textMesh.text = "X+" + number;
                break;
            case Expression.MINUS:
                textMesh.text = "X-" + number;
                break;
            case Expression.MULTIPLY:
                textMesh.text = "X*" + number;
                break;
            case Expression.DIVIDE:
                textMesh.text = "X/" + number;
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
    public int SpawnPlayerEntity(int amount) 
    {
        // int currentPowerLevel = PlayerController.instance.totalPowerLevel;
        int EntitySpawned = 0;
        for (int i = 0; i < amount; i++) 
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
    public void Activate()
    {
        int calculatedPowerLevel = CalculatePowerLevel();
        int EntitySpawned = 0;
        if (PlayerController.instance.totalPowerLevel < calculatedPowerLevel)
        {
            EntitySpawned = SpawnPlayerEntity(calculatedPowerLevel - PlayerController.instance.totalPowerLevel);
        }
        CalculatePowerLevelForEntity(calculatedPowerLevel, PlayerController.instance.totalPowerLevel, EntitySpawned);

        PlayerController.instance.totalPowerLevel = calculatedPowerLevel;
        PlayerController.instance.UpdatePowerLevel();
    }
    public void CalculatePowerLevelForEntity(int calculatedPowerLevel, int currentPowerLevel, int newSpawn)
    {
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
                    pe.ChangeAppearance(); 
                    index++;
                }
            }
            else if (offset <= activePlayerEntity && activePlayerEntity == PlayerController.instance.maxUnit)
            {
                PlayerEntity playerEntity = PlayerEntity[Random.Range(0, PlayerEntity.Count)].GetComponent<PlayerEntity>();
                playerEntity.powerLevel += (offset);
                playerEntity.ChangeAppearance();
            }
            // grouping
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
            if (calculatedPowerLevel < PlayerController.instance.maxUnit) // split the player entity
            {
                PlayerController.instance.RemoveAllEntity();
                SpawnPlayerEntity(calculatedPowerLevel);
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
                    foreach (var entity in killList)
                        entity.Kill();
                }
            }
            if (offset < activePlayerEntity && calculatedPowerLevel >= PlayerController.instance.maxUnit)
            {
                int powerLevelDecrease = offset;
                List<PlayerEntity> killList = new List<PlayerEntity>();

                /* 3 outcomes: 
                * power level - pdl > 0 -> break
                * power level - pdl < 0 -> pld = Mathf.Abs(power level - pdl)
                * power level - pdl = 0 -> break
                *
                */
                while (powerLevelDecrease > 0)
                {
                    // Debug.Log("loop offset < active");
                    PlayerEntity randomEntity = PlayerEntity[Random.Range(0, PlayerEntity.Count)].GetComponent<PlayerEntity>();
                    if (killList.Contains(randomEntity))
                        continue;
                    randomEntity.powerLevel -= powerLevelDecrease;
                    randomEntity.ChangeAppearance(); // NOTE: move this somewhere else
                    if (randomEntity.powerLevel <= 0)
                    {
                        powerLevelDecrease = Mathf.Abs(randomEntity.powerLevel);
                        killList.Add(randomEntity);
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
    }
    
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
        PlayerEntity randomEntity = entities[Random.Range(0, entities.Count())];
        int tempPowerlevel = randomEntity.powerLevel;
        randomEntity.powerLevel = sumPower;
        if (randomEntity.GetTier() > maxTier)
        {
            // Debug.Log("sum power of 3 random entity: " + sumPower);
            randomEntity.ChangeAppearance();
            entities.Remove(randomEntity);
            foreach (var entity in entities)
            {
                entity.powerLevel = 1;
                PlayerController.instance.RemoveEntityFromFormation(entity.transform);
                ObjectPooler.instance.DeactivatePooledObject(entity.gameObject);

            }
            return true;
        }
        else randomEntity.powerLevel = tempPowerlevel; //return to the old power level
        return false;

    }
}
