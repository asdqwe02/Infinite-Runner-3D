using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlastWave : MonoBehaviour
{

    public int pointsCount;
    public float maxRadius;
    public float speed;
    public float startWidth;
    private LineRenderer lineRenderer;
    private void Awake() {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = pointsCount +1; 
    }
    private IEnumerator Blast()
    {
        float currentRadius = 0f;
        while(currentRadius<maxRadius)
        {
            currentRadius += Time.deltaTime*speed;
            Draw(currentRadius);
            yield return null;
        }
    }
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine(Blast());
        }
    }
    private void OnEnable() {
        StartCoroutine(Blast());
    }
    private void Draw(float currentRadius)
    {
        float angleBetweenPoints = 360f/pointsCount;
        for(int i=0; i <= pointsCount;i++)
        {
            float angle =  i*angleBetweenPoints*Mathf.Deg2Rad;
            Vector3 direction = new Vector3(Mathf.Sin(angle),Mathf.Cos(angle),0f);
            Vector3 pos = direction * currentRadius;
            lineRenderer.SetPosition(i,pos); 
        }
        lineRenderer.widthMultiplier = Mathf.Lerp(0f,startWidth, 1f - currentRadius/maxRadius);
    }
}
