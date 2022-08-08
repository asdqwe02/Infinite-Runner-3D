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
        if (powerLevel <=0)
            return 0;
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
    public void ParticleExplode()
    {
        var particle = ObjectPooler.instance.GetPooledObject("ParticleBodyExplode");
        if (particle!=null)
        {
            particle.transform.parent = null;
            Vector3 pos = transform.position;
            float offsetY = Renderer.bounds.size.y/2;
            pos.y = offsetY;
            ParticleSystemRenderer psr = particle.GetComponentInChildren<ParticleSystemRenderer>();
            psr.material.color = tiers[GetTier()].color;

            psr.material.EnableKeyword("_EMISSION");
            psr.material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.None;
            psr.material.SetColor("_EmissionColor",tiers[GetTier()].color * 1f);

            particle.transform.position = pos;
            particle.SetActive(true);
            particle.GetComponentInChildren<ParticleSystem>().Play();
        }
       
    }
    public virtual void TakeDamage(int Damage)
    {
        powerLevel -= Damage;
    }
    
}
