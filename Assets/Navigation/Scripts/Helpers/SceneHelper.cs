﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneHelper: MonoBehaviour {
    public static void SetCollidersActive( ref List<Collider> colliders, bool isActive ) {
        colliders.ForEach(c => c.enabled = isActive);
    }

    public static void SetGameObjectsActive( ref List<GameObject> gameObjects, bool isActive ) {
        foreach (var @object in gameObjects) {
            if (@object != null) {
                @object.SetActive(isActive);
            }
        }
    }
}
