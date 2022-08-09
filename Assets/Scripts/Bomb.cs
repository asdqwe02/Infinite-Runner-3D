using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utility;
public class Bomb : MonoBehaviour
{
    [SerializeField] private Transform _firePoint;
    [SerializeField] private Transform _target;
    public float time;
    public float speed;
    public int damage;
    float startTime;
    Vector3 centerPoint, startRelCenter, endRelCenter;
    public Transform eplodeEffect;
    [SerializeField] private List<EnemyEntity> enemyEntities;
    private void Awake()
    {
        enemyEntities = new List<EnemyEntity>();
    }
    private void Update()
    {
        if (Vector3.Distance(transform.position, _target.position) <= 1 || _target.gameObject.activeSelf == false)
        {
            Explode();
        }
        GetCenter(Vector3.up);
        float fracComplete = (Time.time - startTime) / time * speed;
        transform.position = Vector3.Slerp(startRelCenter, endRelCenter, fracComplete * speed);
        transform.position += centerPoint;
    }
    public void SetUp(Transform target, Transform firepoint, int damage)
    {
        _target = target;
        _firePoint = firepoint;
        this.damage = damage;
        startTime = Time.time;
        AudioManager.instance.PlaySound(AudioManager.Sound.BombSizzle);
    }
 
    public void GetCenter(Vector3 direction)
    {
        centerPoint = (_firePoint.position + _target.position) * .5f;
        centerPoint -= direction;
        startRelCenter = _firePoint.position - centerPoint;
        endRelCenter = _target.position - centerPoint;
    }
    private void Explode()
    {
        // Deal damage
        foreach (EnemyEntity ee in enemyEntities)
        {
            if (ee.gameObject.activeSelf)
            {
                ee.TakeDamage(damage);
                if (ee.powerLevel <= 0)
                {
                    ee.Kill();
                }
            }
        }

        // spawn Explosion VFX
        GameObject explodeVFX = ObjectPooler.instance.GetPooledObject("ExplosionVFX");
        if (explodeVFX != null)
        {
            explodeVFX.transform.parent = null;
            explodeVFX.transform.position = transform.position;
            explodeVFX.SetActive(true);
            AudioManager.instance.PlaySound(AudioManager.Sound.BombExplode, transform.position);
        }


        // disable the bomb
        transform.parent = ObjectPooler.instance.transform;
        enemyEntities.Clear();
        gameObject.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyEntity"))
        {
            enemyEntities.Add(other.GetComponent<EnemyEntity>());
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("EnemyEntity"))
        {
            EnemyEntity ee = other.GetComponent<EnemyEntity>();
            if (enemyEntities.Contains(ee))
            {
                enemyEntities.Remove(ee);
            }
        }
    }
}
