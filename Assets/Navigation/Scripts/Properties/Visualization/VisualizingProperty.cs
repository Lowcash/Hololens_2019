using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualizingProperty : Property, IVisualizing
{
    public void StartVisualizing()
    {
        IsEnable = true;
    }
}
