using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntityFomrationController : MonoBehaviour
{
    // Start is called before the first frame update
    public int column, row;
    public float offsetX, offsetZ;
    public Transform EntitySpawnPositionPrefab;
    public Transform PlayerEntityPrefab;
    Transform [,] formation;
    private void Awake() {
        formation = new Transform[row,column];
        float start_pos;
        if (column%2==0)
            start_pos = -(column/2)*offsetX + offsetX/2;
        else start_pos = -(column/2)*offsetX;
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                // Instantiate
                Vector3 pos = new Vector3(start_pos+offsetX*j,0,-offsetZ*i);
                formation[i,j] = Instantiate(EntitySpawnPositionPrefab,transform);
                formation[i,j].localPosition = pos;
            }
        }
        GameObject initialPlayerEnntity = ObjectPooler.instance.GetPooledObject("PlayerEntity");
        Vector3 initial_entity_pos = initialPlayerEnntity.transform.localPosition;
        initialPlayerEnntity.transform.parent = formation[0,column/2];
        initialPlayerEnntity.transform.localPosition = initial_entity_pos;
        initialPlayerEnntity.SetActive(true);
        // Instantiate(PlayerEntityPrefab,formation[0,column/2]);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
