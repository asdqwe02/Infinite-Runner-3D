using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public  List<Transform> level;
    public float offsetZ = 100f;
    public int counter;
    public int levelPassed;
    public int nextLevelToGenerate;
    public static LevelManager instance;
    public float boundaryValue;
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
        boundaryValue = level[0].GetComponent<Renderer>().bounds.size.x/2;
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
            foreach (MultiplyPlate mp in   level[nextLevelToGenerate].GetComponentsInChildren<MultiplyPlate>())
            {
                mp.RollMultiplyPlate();
            }
            nextLevelToGenerate++;
            if (nextLevelToGenerate>level.Count-1)
                nextLevelToGenerate=0;
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
}
