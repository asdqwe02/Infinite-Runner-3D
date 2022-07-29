using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Entity : MonoBehaviour
{
    Rigidbody _rb;
    [SerializeField] private Vector3 _ogSize; // SerializeField to debug
    [SerializeField] private SkinnedMeshRenderer _renderer;

    public Rigidbody rb { get => _rb; set => _rb = value; } // kinda useless
    public Vector3 OGSize { get => _ogSize; set => _ogSize = value; }
    public SkinnedMeshRenderer Renderer { get => _renderer; set => _renderer = value; }

    public int powerLevel;
    public List<Tier> tiers;
    public EntityTier entityTier;

    protected void Awake() 
    {
        tiers = new List<Tier>(entityTier.tiers);
        _ogSize = transform.localScale;
    }
    public int GetTier() // this doesn't even work it's stupid as fuck
    {
        for (int i=0 ; i<tiers.Count-1;i++)
        {
            if (  powerLevel >= tiers[i].minPower  && powerLevel<=tiers[i].maxPower)
                return i;
        }
        return tiers.Count-1; // max tier
    }
    public virtual void Kill()
    {
        powerLevel = 1;
        ObjectPooler.instance.DeactivatePooledObject(gameObject);
    }

    // change appearance base on power level
    public virtual void ChangeAppearance()
    {
        float sizeIncrease = GetTier() * entityTier.sizeMultiplier;
        transform.localScale = _ogSize + new Vector3(sizeIncrease,sizeIncrease,sizeIncrease);
        if (Renderer.material.color != tiers[GetTier()].color)
        {
            Renderer.material.color = tiers[GetTier()].color;
            transform.localScale = OGSize + new Vector3(sizeIncrease,sizeIncrease,sizeIncrease);
        }
    }
    
}
