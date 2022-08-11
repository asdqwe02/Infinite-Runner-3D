using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
public class LaserController : MonoBehaviour
{
    public Transform target;
    public List<Transform> targetList;
    public VisualEffect vfx;
    PlayerEntity playerEntity;
    [SerializeField] private int _damage;
    public float offset = 0.5f;
    // Start is called before the first frame update
    void Awake()
    {
        targetList = new List<Transform>();
        if (vfx == null)
            vfx = GetComponent<VisualEffect>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerEntity.gameObject.activeSelf == false)
        {
            transform.parent.GetComponent<LaserPointHandler>().laser = null;
            transform.parent = ObjectPooler.instance.transform;
            gameObject.SetActive(false);
            return;
        }

        if (!targetList.Contains(target) || target.gameObject.activeSelf == false)
        {
            ResetTarget();
        }
        if (target != null && target.gameObject.activeSelf != false)
        {
            vfx.SetFloat("BeamLength", Vector3.Distance(target.position, transform.position) / 20f + offset); // abitrary and dumb
            transform.LookAt(target.position);
        }
        else if (targetList.Count != 0)
        {
            target = targetList[Random.Range(0, targetList.Count)];
        }

    }
    private void OnEnable()
    {
        target = null;
        StartCoroutine(DealDamage());
    }
    private void OnDisable()
    {
        targetList.Clear();
    }

    public void SetUp(PlayerEntity entity, float duration, int damage, Color color)
    {
        vfx.SetFloat("Duration", duration);
        float intensity = Mathf.Pow(2, 1.427857f); // intensity calculation is wrong
        Vector4 hdrColor = new Vector4(color.r * intensity, color.g * intensity, color.b * intensity, color.a* intensity);
        // Debug.Log("HDR COLOR:" + hdrColor);
        vfx.SetVector4("Beam Core Color", hdrColor);
        playerEntity = entity;
        transform.parent = entity.laserPoint;
        _damage = damage;

        transform.parent.GetComponent<LaserPointHandler>().laser = this;
        transform.localPosition = Vector3.zero;
        targetList.Clear();
    }
    public void ResetTarget()
    {
        targetList.Remove(target);
        target = null;
        transform.rotation = Quaternion.identity;
        vfx.SetFloat("BeamLength", 0);
    }
    public IEnumerator DealDamage()
    {
        while (gameObject.activeSelf)
        {
            yield return new WaitForSeconds(0.5f);
            if (target != null && target.gameObject.activeSelf)
            {
                EnemyEntity entity = target.GetComponent<EnemyEntity>();
                entity.TakeDamage(_damage);
                Debug.Log("deal laser damage ");
                if (entity.powerLevel <= 0)
                {
                    entity.powerLevel = 1; /// dumb fix to a small visual bug
                    targetList.Remove(target);
                    target = null;
                    entity.Kill();
                }
            }
        }
    }
}
