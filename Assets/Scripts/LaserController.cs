using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
public class LaserController : MonoBehaviour
{
    public Transform target;
    public List<Transform> targetList;
    public VisualEffect vfx;
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
        
        if (target != null && target.gameObject.activeSelf != false)
        {
            vfx.SetFloat("BeamLength", Vector3.Distance(target.position, transform.position) / 20f + offset); // abitrary
            transform.LookAt(target.position);
        }
        else if (targetList.Count != 0)
        {
            target = targetList[Random.Range(0, targetList.Count)];
        }
        
        if (!targetList.Contains(target) || target.gameObject.activeSelf == false)
        {
            ResetTarget();
        }

    }
    private void OnEnable()
    {
        target = null;
        StartCoroutine(DealDamage());
    }
    private void OnDisable() {
        targetList.Clear();
    }

    public void SetUp(float duration, Transform parent, int damage) // note: add power level in this shit
    {
        vfx.SetFloat("Duration", duration);
        transform.parent = parent;
        _damage = damage;

        parent.GetComponent<LaserPointHandler>().laser = this;
        transform.localPosition = Vector3.zero;
        targetList.Clear();
    }
    public void ResetTarget()
    {
        targetList.Remove(target);
        target = null;
        transform.rotation = Quaternion.identity;
    }
    public IEnumerator DealDamage()
    {
        while (gameObject.activeSelf)
        {
            yield return new WaitForSeconds(0.75f);
            if (target !=null && target.gameObject.activeSelf)
            {
                EnemyEntity entity = target.GetComponent<EnemyEntity>();
                entity.TakeDamage(_damage);
                Debug.Log("deal laser damage ");
                if (entity.powerLevel<=0)
                {
                    targetList.Remove(target);
                    target=null;
                    entity.Kill();
                }
            }
        }
    }
}
