using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [System.Serializable]
    public struct Level
    {
        public Transform ground;
        public List<GameObject> obstacleList;
    }
    public List<Level> levelList;
    private float _offsetZ, _offsetX;
    public int counter;
    public int levelPassed; // use this to calculate point and maybe spawn obstacle
    public int nextLevelToGenerate;
    public int lastLevel;
    public static LevelManager instance;
    public float boundaryValue;
    // Start is called before the first frame update
    public float speed;
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        boundaryValue = levelList[0].ground.GetComponent<Renderer>().bounds.size.x / 2;
        counter = 0;
        _offsetZ = levelList[0].ground.GetComponent<Renderer>().bounds.size.z;
        _offsetX = levelList[0].ground.GetComponent<Renderer>().bounds.size.x;
        nextLevelToGenerate = 0;
        lastLevel = levelList.Count - 1;
    }
    private void Start()
    {
        for (int i = 0; i < levelList.Count - 1; i++) // ... WTF
        {
            levelList[i].ground.GetComponentInChildren<PlateController>().nextEnemySpannwer = levelList[i + 1].ground.GetComponentInChildren<EnemySpawnerController>();
            levelList[i + 1].ground.GetComponentInChildren<PlateController>().previousEnemySpannwer = levelList[i].ground.GetComponentInChildren<EnemySpawnerController>();
        }
        levelList[levelList.Count - 1].ground.GetComponentInChildren<PlateController>().nextEnemySpannwer = levelList[0].ground.GetComponentInChildren<EnemySpawnerController>();
        levelList[0].ground.GetComponentInChildren<PlateController>().previousEnemySpannwer = levelList[levelList.Count - 1].ground.GetComponentInChildren<EnemySpawnerController>();

        levelList[0].ground.GetComponentInChildren<PlateController>().currentEnenmySpawner.SpawnEnemyEntity(); // ... doesn't work ???? wtf
    }
    // Update is called once per frame
    void Update()
    {
        // kinda bad
        if (counter >= 2)
        {
            counter = 1;
            // level[nextLevelToGenerate].position = new Vector3(0,0,offsetZ*(levelPassed+2));
            levelList[nextLevelToGenerate].ground.position = new Vector3(0, 0, _offsetZ + levelList[lastLevel].ground.position.z);

            DespawnObstacle(levelList[nextLevelToGenerate]);

            foreach (MultiplyPlate mp in levelList[nextLevelToGenerate].ground.GetComponentsInChildren<MultiplyPlate>())
            {
                mp.RollMultiplyPlate();
            }

            // calculate next level to generate
            lastLevel = nextLevelToGenerate;
            nextLevelToGenerate++;
            if (nextLevelToGenerate > levelList.Count - 1)
                nextLevelToGenerate = 0;

            if (levelPassed >= 2)
            {
                SpawnObstacles(levelList[nextLevelToGenerate], 3, false);
                if (nextLevelToGenerate == levelList.Count - 1)
                    SpawnObstacles(levelList[0], 3, true);
                else
                    SpawnObstacles(levelList[nextLevelToGenerate + 1], 3, true);

            }
        }
    }
    private void FixedUpdate()
    {
        foreach (Level level in levelList)
        {
            level.ground.Translate(Vector3.back * speed * Time.deltaTime, Space.World);
        }
    }
    public void CheckBoundary(Transform tObject)
    {
        Vector3 current_pos = tObject.position;
        if (tObject.position.x <= -boundaryValue)
        {
            current_pos.x = -boundaryValue;
            tObject.position = current_pos;
        }
        if (tObject.position.x >= boundaryValue)
        {
            current_pos.x = boundaryValue;
            tObject.position = current_pos;
        }
    }
    public Transform GetCurrentLevel()
    {
        return levelList[nextLevelToGenerate].ground;
    }
    public void SpawnObstacles(Level level, int amount, bool beforePlate)
    {
        List<Vector3> spawnPosList = new List<Vector3>();
        float minOffsetZ, maxOffsetZ;
        if (beforePlate)
        {
            minOffsetZ = -_offsetZ / 2;
            maxOffsetZ = 0;
        }
        else
        {
            minOffsetZ = 0;
            maxOffsetZ = _offsetZ / 2;
        }
        for (int i = 0; i < amount; i++)
        {
            GameObject obstacle = ObjectPooler.instance.GetPooledObject("Obstacle");
            if (obstacle)
            {
                Vector3 spawnPos;
                spawnPos = GetObstacleSpawnPosition(_offsetX / 2, minOffsetZ, maxOffsetZ);
                while (spawnPosList.Contains(spawnPos))
                {
                    spawnPos = GetObstacleSpawnPosition(_offsetX / 2, -_offsetZ / 2, 0);
                }
                // base scale and position on ground
                // Vector3 scale = obstacle.transform.localScale;
                spawnPos.y = obstacle.transform.localPosition.y;

                obstacle.transform.parent = level.ground;
                // obstacle.transform.localScale = scale;
                obstacle.transform.position = spawnPos;
                obstacle.SetActive(true);
                level.obstacleList.Add(obstacle);
            }
            else return;
        }

    }
    public void DespawnObstacle(Level level)
    {
        foreach (var obstacle in level.obstacleList)
        {
            float yPos = obstacle.transform.localPosition.y;

            obstacle.transform.parent = ObjectPooler.instance.transform;
            obstacle.transform.localPosition = new Vector3(0,yPos,0);
            obstacle.SetActive(false);

        }
    }
    public Vector3 GetObstacleSpawnPosition(float offsetX, float minOffsetZ, float maxOffsetZ)
    {
        return new Vector3(Random.Range(-offsetX, offsetX), 0, Random.Range(minOffsetZ, maxOffsetZ));
    }
}
