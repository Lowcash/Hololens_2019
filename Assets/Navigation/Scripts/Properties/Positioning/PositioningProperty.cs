using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositioningProperty : Property, IPositioning
{
    public void StartPositioning()
    {
        IsEnable = true;
    }

    public void StopPositioning()
    {
        IsEnable = false;
    }
}
