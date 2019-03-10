using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour {

    [Header("Visualization property scripts")]
    public DissapearByDistance dissapearByDistanceScript;

    [Header("Measurement property scripts")]
    public ComputeDistance computeDistanceScript;

    private void Update()
    {
        dissapearByDistanceScript.SetActualDistance(computeDistanceScript.MeasuredDistance);
    }
}
