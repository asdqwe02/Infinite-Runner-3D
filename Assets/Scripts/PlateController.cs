using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateController : MonoBehaviour
{
    public List<MultiplyPlate> multiplyPlate;
    private void Awake() {
        foreach(MultiplyPlate obj in transform.parent.GetComponentsInChildren<MultiplyPlate>())
        {
            multiplyPlate.Add(obj);
        }
    }
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player"))
        {
            foreach(MultiplyPlate plate in multiplyPlate)
                if (plate.trigger)
                {
                    plate.Activate();
                    DeTriggerMultiplyPlate();
                    return;
                }
        }
    }
    private void DeTriggerMultiplyPlate()
    {
        foreach(MultiplyPlate plate in multiplyPlate)
            plate.trigger = false;
    }
}
