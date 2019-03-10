using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToObject : TrackingProperty
{
    public float trackingSpeed;

    public GameObject applyOn;

    private void Update()
    {
        if (ObjectToTracking != null)
        {
            var targetDirection = ObjectToTracking.transform.position - applyOn.transform.position;

            float step = trackingSpeed * Time.deltaTime;

            Vector3 newDirection = Vector3.RotateTowards(applyOn.transform.forward, targetDirection, step, 0.0f);

            //Debug.DrawRay(transform.position, newDirection, Color.red);

            applyOn.transform.rotation = Quaternion.LookRotation(newDirection);
        }
        else
        {
#if UNITY_EDITOR
            Debug.Log("Tracking rotate object is not set!");
#endif
        }
    }
}
