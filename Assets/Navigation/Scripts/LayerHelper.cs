using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerHelper : MonoBehaviour {
    public enum LayerName {
        IgnoreRaycast = 2,
        UI = 5,
        SpatialMapping = 31,
        RaycastSpatialMapping = 1 << 31
    }

    public static List<GameObject> FindObjectsInLayer(GameObject root, int layer)
    {
        var objects = new List<GameObject>();

        foreach (Transform t in root.transform.GetComponentsInChildren(typeof(GameObject), true))
        {
            if (t.gameObject.layer == layer)
            {
                objects.Add(t.gameObject);
            }
        }

        return objects;
    }
}
