using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputeDistance : MeasurementProperty
{
    public float MeasuredDistance { get; private set; }

    private void Update()
    {
        if (ObjectToMeasure != null)
        {
            MeasuredDistance = Vector3.Distance(transform.position, ObjectToMeasure.transform.position);
        }
    }
}
