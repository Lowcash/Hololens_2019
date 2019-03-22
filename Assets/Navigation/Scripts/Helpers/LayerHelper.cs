using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LayerName {
    IgnoreRaycast = 2,
    UI = 5,
    SpatialMapping = 31,
    RaycastSpatialMapping = 1 << 31
}

public class LayerHelper {
    public static List<GameObject> FindObjectsInLayer( GameObject root, LayerName layer ) {
        var objects = new List<GameObject>() { root };

        for (int i = 0; i < objects.Count; i++) {
            foreach (Transform t in objects[i].transform) {
                if (t.gameObject.layer == (int)layer) {
                    objects.Add(t.gameObject);
                }
            }
        }

        return objects;
    }

    public static void SetObjetsLayer( ref List<GameObject> objects, LayerName layer ) {
        objects.ForEach(o => o.layer = (int)layer);
    }
}
