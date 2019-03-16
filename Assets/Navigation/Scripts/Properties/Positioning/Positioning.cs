using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Positioning : MonoBehaviour, IPositioning {
    public List<PositioningProperty> positioningProperties = new List<PositioningProperty>();

    private void Awake()
    {
        positioningProperties.ForEach(p => p.Init());
    }

    public void StartPositioning()
    {
        positioningProperties.ForEach(p => p.StartPositioning());
    }

    public void StopPositioning()
    {
        positioningProperties.ForEach(p => p.StopPositioning());
    }
}
