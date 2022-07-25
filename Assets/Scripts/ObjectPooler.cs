using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ObjectPoolItem
{
    public GameObject objectToPool;
    public int amountToPool;
    public bool shouldExpand = false;
}
public class ObjectPooler : MonoBehaviour
{
    public List<GameObject> pooledObjects;
    // public GameObject objectToPool;
    // public int amountToPool;
    // public bool shouldExpand = false;
    public List<ObjectPoolItem> itemsToPool;
    public static ObjectPooler instance;
    private void Awake() {
        if (instance == null)
            instance = this;
        else 
        {
            Destroy(gameObject);
            return;
        }
        pooledObjects = new List<GameObject>();
        foreach (ObjectPoolItem item in itemsToPool) 
        {
            for (int i = 0; i < item.amountToPool; i++) 
            {
                GameObject obj = (GameObject)Instantiate(item.objectToPool,transform);
                obj.SetActive(false);
                pooledObjects.Add(obj);
            }
        }
    }
    private void Start() {
     
    }
    public GameObject GetPooledObject(string tag) {
        foreach(GameObject pObject in pooledObjects)
        {
            if (pObject.activeInHierarchy == false && pObject.tag == tag)
                return pObject;
        }
        // for (int i = 0; i < pooledObjects.Count; i++) {
        //     if (pooledObjects[i].activeInHierarchy==false && pooledObjects[i].tag == tag) 
        //     {
        //         return pooledObjects[i];
        //     }
        // }
        foreach (ObjectPoolItem item in itemsToPool) 
        {
            if (item.objectToPool.tag == tag) 
            {
                if (item.shouldExpand) {
                    GameObject obj = (GameObject)Instantiate(item.objectToPool);
                    obj.SetActive(false);
                    pooledObjects.Add(obj);
                    return obj;
                }
            }
        }
        return null;
    }
    public int GetActivePooledObject(string tag)
    {
        int c=0;
        for (int i=0; i<pooledObjects.Count;i++)
        {
            if (pooledObjects[i].activeInHierarchy && pooledObjects[i].tag == tag)
                c++;
        }
        return c;
    }
}