using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Measurement : MonoBehaviour, IMeasuring {
    public List<MeasurementProperty> measurableProperties = new List<MeasurementProperty>();

    private void Awake()
    {
        measurableProperties.ForEach(p => p.Init());
    }

    public void StartMeasuring()
    {
        measurableProperties.ForEach(p => p.IsEnable = true);
    }

    public void StopMeasuring()
    {
        measurableProperties.ForEach(p => p.IsEnable = false);
    }

    public void SetMeasurementTo(GameObject gameObject)
    {
        measurableProperties.ForEach(p => p.ObjectToMeasure = gameObject);
    }
}
