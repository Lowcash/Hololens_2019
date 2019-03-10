using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tracking : MonoBehaviour, ITracking
{
    public List<TrackingProperty> trackableProperties = new List<TrackingProperty>();

    private void Awake()
    {
        trackableProperties.ForEach(t => t.Init());
    }

    public void StartTracking()
    {
        trackableProperties.ForEach(p => p.IsEnable = true);
    }

    public void StopTracking()
    {
        trackableProperties.ForEach(p => p.IsEnable = false);
    }

    public void SetTrackingTo(GameObject gameObject)
    {
        trackableProperties.ForEach(p => p.ObjectToTracking = gameObject);
    }
}
