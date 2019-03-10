using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MeasurementInfo : MonoBehaviour {
    public Text infoText;

    [Header("Measurement property scripts")]
    public ComputeDistance computeDistanceScript;

    private void Update()
    {
        infoText.text = string.Format("{0}m", computeDistanceScript.MeasuredDistance.ToString("0.#0"));
    }
}
