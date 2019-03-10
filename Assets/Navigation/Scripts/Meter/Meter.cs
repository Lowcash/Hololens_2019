using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meter : MonoBehaviour {
    [Header("Transform property scripts")]
    public ScaleForwardObject scaleObjectScript;
    public TranslateForwardObject translateObjectScript;

    [Header("Measurement property scripts")]
    public ComputeDistance computeDistanceScript;

    //public List<PositioningProperty> positioningProperties = new List<PositioningProperty>();

    //private void Awake()
    //{
    //    positioningProperties.ForEach(t => t.Init());
    //}

    //private void Start()
    //{
    //    positioningProperties.ForEach(t => t.IsEnable = true);
    //}

    private void Update()
    {
        translateObjectScript.distanceOfTranslate = computeDistanceScript.MeasuredDistance / 2;
        scaleObjectScript.sizeOfScale = computeDistanceScript.MeasuredDistance;
    }
}
