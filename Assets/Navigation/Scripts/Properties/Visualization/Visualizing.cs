using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Visualizing : MonoBehaviour, IVisualizing {
    public List<VisualizingProperty> visualizingProperties = new List<VisualizingProperty>();

    private void Awake()
    {
        visualizingProperties.ForEach(v => v.Init());
    }

    public void StartVisualizing()
    {
        visualizingProperties.ForEach(v => v.IsEnable = true);
    }
}
