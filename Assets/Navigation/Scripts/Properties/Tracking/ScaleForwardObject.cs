using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleForwardObject : TrackingProperty
{
    public GameObject applyOn;

    [HideInInspector]
    public float sizeOfScale = 0.0f;

    private void Update()
    {
        if (ObjectToTracking != null)
        {
            Vector3 localScale = applyOn.transform.localScale;

            applyOn.transform.localScale = new Vector3(localScale.x, localScale.y, sizeOfScale);
        }
        else
        {
#if UNITY_EDITOR
            Debug.Log("Tracking scale object is not set!");
#endif
        }
    }
}
