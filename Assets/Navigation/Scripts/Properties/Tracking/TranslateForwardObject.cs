using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslateForwardObject : TrackingProperty
{
    public GameObject applyOn;

    [HideInInspector]
    public float distanceOfTranslate = 0.0f;

    private void Update()
    {
        if (ObjectToTracking != null)
        {
            Vector3 localPosition = applyOn.transform.localPosition;

            applyOn.transform.localPosition = new Vector3(localPosition.x, localPosition.y, distanceOfTranslate);
        }
        else
        {
#if UNITY_EDITOR
            Debug.Log("Tracking translate object is not set!");
#endif
        }
    }
}
