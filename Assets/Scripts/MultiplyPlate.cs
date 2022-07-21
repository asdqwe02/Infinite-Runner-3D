using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplyPlate : MonoBehaviour
{
    // Start is called before the first frame update
    public int number;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player"))
        {
            LevelGenerator.instance.counter++;
            LevelGenerator.instance.levelPassed++;
            for (int i=0; i<number; i++)
            {
                GameObject playerEntity = ObjectPooler.instance.GetPooledObject();
                if (playerEntity!=null)
                {
                    Vector3 pos = playerEntity.transform.localPosition;
                    playerEntity.transform.parent = other.transform;
                    playerEntity.transform.localPosition = pos;
                    playerEntity.SetActive(true);
                }
            }
                
        }
    }
    
}
