using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public  List<Transform> level;
    private float _offsetZ;
    public int counter;
    public int levelPassed; // use this to calculate point
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
        boundaryValue = level[0].GetComponent<Renderer>().bounds.size.x/2;
        counter = 0;
        _offsetZ = level[0].GetComponent<Renderer>().bounds.size.z;
        nextLevelToGenerate = 0;
        lastLevel = level.Count-1;
    }
    private void Start() {
        for (int i =0; i<level.Count-1; i++) // ... WTF
        {
            level[i].GetComponentInChildren<PlateController>().nextEnemySpannwer = level[i+1].GetComponentInChildren<EnemySpawnerController>();
            level[i+1].GetComponentInChildren<PlateController>().previousEnemySpannwer = level[i].GetComponentInChildren<EnemySpawnerController>();
        }
        level[level.Count-1].GetComponentInChildren<PlateController>().nextEnemySpannwer = level[0].GetComponentInChildren<EnemySpawnerController>();
        level[0].GetComponentInChildren<PlateController>().previousEnemySpannwer = level[level.Count-1].GetComponentInChildren<EnemySpawnerController>();

        level[level.Count-1].GetComponentInChildren<PlateController>().nextEnemySpannwer.SpawnEnemyEntity();
    }
    // Update is called once per frame
    void Update()
    {
        if (counter>=2)
        {
            counter = 1;
            // level[nextLevelToGenerate].position = new Vector3(0,0,offsetZ*(levelPassed+2));
            level[nextLevelToGenerate].position = new Vector3(0,0,_offsetZ+level[lastLevel].position.z);
            foreach (MultiplyPlate mp in level[nextLevelToGenerate].GetComponentsInChildren<MultiplyPlate>())
            {
                mp.RollMultiplyPlate();
            }
            lastLevel = nextLevelToGenerate;
            nextLevelToGenerate++;
            if (nextLevelToGenerate>level.Count-1)
                nextLevelToGenerate=0;
        }
    }
    private void FixedUpdate() {
        foreach(Transform ground in level)
        {
            ground.Translate(Vector3.back*speed*Time.deltaTime,Space.World);
        }
    }
    public void CheckBoundary(Transform tObject)
    {
        Vector3 current_pos =  tObject.position;
        if (tObject.position.x <= -boundaryValue)
        {
            current_pos.x=-boundaryValue;
            tObject.position = current_pos;
        }
        if (tObject.position.x>=boundaryValue)
        {
            current_pos.x=boundaryValue;
            tObject.position = current_pos;
        }
    }   
    public Transform GetCurrentLevel()
    {
        return level[nextLevelToGenerate];
    }
}
