using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyTransformHelper : MonoBehaviour {
    public GameObject objectToCopy;

    private void Update() {
        transform.position = objectToCopy.transform.position;
        transform.rotation = objectToCopy.transform.rotation;
    }
}
