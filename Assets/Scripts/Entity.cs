using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [System.Serializable]
    public struct Tier
    {
        public int minPower;
        public int maxPower;
    }
    Rigidbody _rb;
    public Rigidbody rb { get => _rb; set => _rb = value; } // kinda useless
    public int powerLevel;
    public List<Tier> tiers;
    public int GetTier() // this is hardcoded and look stupid as fuck
    {
        for (int i=0 ; i<tiers.Count-1;i++)
        {
            if (tiers[i].minPower<=powerLevel && powerLevel<tiers[i].maxPower)
                return i;
        }
        return tiers.Count-1;
    }
}
