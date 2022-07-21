using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public  List<Transform> level;
    public float offsetZ = 100f;
    public int counter;
    public int levelPassed;
    public int nextLevelToGenerate;
    public static LevelGenerator instance;
    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
            instance = this;
        else 
        {
            Destroy(gameObject);
            return;
        }
        counter = 0;
        nextLevelToGenerate = 0;
    }
    // Update is called once per frame
    void Update()
    {
        if (counter>=2)
        {
            counter = 1;
            level[nextLevelToGenerate].position = new Vector3(0,0,offsetZ*(levelPassed+2));
            nextLevelToGenerate++;
            if (nextLevelToGenerate>level.Count-1)
                nextLevelToGenerate=0;
        }
    }   
}
