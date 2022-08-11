using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeVFXController : MonoBehaviour
{
    [SerializeField] Transform blast;
    [SerializeField] Transform spark;
    ParticleSystemRenderer mainPSR, sparkPSR;
    LineRenderer blastLR;
    private void Awake()
    {
        blastLR = blast.GetComponent<LineRenderer>();
        sparkPSR = spark.GetComponent<ParticleSystemRenderer>();
        mainPSR = GetComponent<ParticleSystemRenderer>();
    }
    public void SetUp(Color color)
    {
        // main explosion material main color and hdr color
        mainPSR.material.color = color;
        mainPSR.material.EnableKeyword("_EMISSION");
        mainPSR.material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.None;
        mainPSR.material.SetColor("_EmissionColor", color * 1.427857f); // hdr intensity should be 1.5f or 1.43...

        // spark material main color and hdr color
        sparkPSR.material = mainPSR.material;
        sparkPSR.trailMaterial = mainPSR.material;

        // blast material main color and hdr color
        blastLR.material = mainPSR.material;



    }
}
