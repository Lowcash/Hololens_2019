using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpatialObserver : MonoBehaviour {

    private List<MeshCollider> _meshColliders = new List<MeshCollider>();

	// Use this for initialization
	void Start () {
        GetChildren();
        SetColliders(false);

        InvokeRepeating("GetChildren", 5f, 5f);
	}
	
	// Update is called once per frame
	private void GetChildren() {
        _meshColliders.Clear();

        List<GameObject> generatedObjects = LayerHelper.FindObjectsInLayer(gameObject, LayerName.SpatialMapping, true);

        if (generatedObjects[0].name == "SpatialMapping") { generatedObjects.RemoveAt(0); }
        generatedObjects.ForEach(g => _meshColliders.Add(g.GetComponent<MeshCollider>()));

    }

    public void SetColliders (bool isEnable) {
        _meshColliders.ForEach(c => c.enabled = isEnable);
    }
}
