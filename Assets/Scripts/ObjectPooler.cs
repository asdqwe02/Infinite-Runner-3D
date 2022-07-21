using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public List<GameObject> pooledObjects;
    public GameObject objectToPool;
    public int amountToPool;
    public static ObjectPooler instance;
    private void Awake() {
        if (instance == null)
            instance = this;
        else 
        {
            Destroy(gameObject);
            return;
        }
    }
    private void Start() {
        pooledObjects = new List<GameObject>();
        for (int i = 0; i < amountToPool; i++) {
            GameObject obj = (GameObject)Instantiate(objectToPool,transform); // remmeber to change parent when set active
            obj.SetActive(false); 
            pooledObjects.Add(obj);
        }
    }
    public GameObject GetPooledObject() {
        for (int i = 0; i < pooledObjects.Count; i++) {
            if (!pooledObjects[i].activeInHierarchy) {
              return pooledObjects[i];
            }
        }
        return null;
    }
}
