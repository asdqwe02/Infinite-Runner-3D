using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    public static List<T> GetRandomItemsFromList<T>(List<T> list, int number)
    {
        // this is the list we're going to remove picked items from
        List<T> tmpList = new List<T>(list);
        // this is the list we're going to move items to
        List<T> newList = new List<T>();

        // make sure tmpList isn't already empty
        while (newList.Count < number && tmpList.Count > 0)
        {
            int index = Random.Range(0, tmpList.Count);
            newList.Add(tmpList[index]);
            tmpList.RemoveAt(index);
        }
        return newList;
    }
    public static List<T> ConvertGameObjsToSCript<T>(List<GameObject> list)
    {
        List<T> tmpList = new List<T>();
        foreach (var obj in list)
        {
            tmpList.Add(obj.GetComponent<T>());
        }
        return tmpList;
    }

    public static int[] PartitionPowerLevel(int powerLevel, int n)
    {
        int k = powerLevel / n;
        int ct = powerLevel % n;
        int i;
        int[] arr = new int[n];
        for (i = 0; i < ct; i++)
        {
            arr[i] = k + 1;
        }
        for (; i < n; i++)
            arr[i] = k;
        return arr;
    }
}

