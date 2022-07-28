using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utility;
public class EnemySpawnerController : MonoBehaviour
{
    // Start is called before the first frame update
    public float offsetZ,offsetX;
    public List<Transform> multiplyPlate;
    int maxPowerLevel = 0;
    List<Vector3> enemyPos;
    void Start()
    {
        enemyPos = new List<Vector3>();
        offsetX = GetComponent<Renderer>().bounds.size.x/2;
        offsetZ = GetComponent<Renderer>().bounds.size.z/2;

    }
    public void CalculatePowerLevel()
    {
        maxPowerLevel = 0;
        foreach (var plate in multiplyPlate)
        {
            int pl  = plate.GetComponent<MultiplyPlate>().CalculatePowerLevel();
            if (pl > maxPowerLevel)
                maxPowerLevel = pl;
        }
    }
    public void SpawnEnemyEntity()
    {
        if (maxPowerLevel<=PlayerController.instance.maxUnit)
        {
            for (int i = 0; i < maxPowerLevel; i++)
            {
                Vector3 randomPos = new Vector3(Random.Range(-offsetX,offsetX),0,Random.Range(-offsetZ,offsetZ));
                while(enemyPos.Contains(randomPos))
                {
                    randomPos = new Vector3(Random.Range(-offsetX,offsetX),0,Random.Range(-offsetZ,offsetZ));
                }
                enemyPos.Add(randomPos);
                GameObject ee = ObjectPooler.instance.GetPooledObject("EnemyEntity");
                if (ee!=null)
                {
                    ee.transform.parent = transform;
                    ee.transform.localPosition = randomPos;
                    ee.SetActive(true);
                }
            }
        }
    }
    public void DespawnEnemyEntity()
    {
        ObjectPooler.instance.RemoveAllObjectWithTag("EnemyEntity");
    }
    
}
