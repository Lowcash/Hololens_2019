using UnityEngine;
using System.Collections.Generic;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.SpatialMapping;

public class HologramBehaviour : PositioningProperty
{
    [Header("Hologram objects")]
    public GameObject hologramContentObject;
    public GameObject hologramDraggableObject;

    [Header("Settings")]
    public float directionToPlaceHologram = 1.0f;

    private int _spatialMappingLayer = 1 << 31;
    private int _hologramLayer = 1 << 30;

    public bool _isHologramPlaced = false;

    private RaycastHit _hit;
    private Ray _ray = new Ray();

    private Interpolator _interpolatorScript;
    private TapToPlace _tapToPlaceScript;
    private HandDraggable _handDraggableScript;

    private List<Collider> _colliders = new List<Collider>();

    private int _numColliders = 0;

    private void Awake()
    {
        _interpolatorScript = hologramContentObject.GetComponent<Interpolator>();
        _tapToPlaceScript = hologramContentObject.GetComponent<TapToPlace>();
        _handDraggableScript = hologramDraggableObject.GetComponent<HandDraggable>();
    }

    private void Update()
    {
        if (_handDraggableScript.MoveDirection != Vector3.zero)
        {
            _ray.origin = transform.position;
            _ray.direction = _handDraggableScript.MoveDirection;
        }

#if UNITY_EDITOR
        Debug.DrawLine(_ray.origin, _ray.origin + (_ray.direction * directionToPlaceHologram), Color.red);
#endif

        var isSpatialLayerHitted = Physics.Raycast(_ray, out _hit, directionToPlaceHologram, _spatialMappingLayer);


        if (isSpatialLayerHitted)
        {
            Debug.Log(Vector3.Dot(Camera.main.transform.forward, _ray.origin + _ray.direction));
            //Debug.Log(Camera.main.transform.forward);
        }

        if (_numColliders == 0)
        {
            _interpolatorScript.SetTargetLocalPosition(gameObject.transform.position);
        }

        //Debug.Log(_numColliders + " " + _tapToPlaceScript.ParentGameObjectToPlace.name);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Spatial Mapping"))
        {
            _tapToPlaceScript.ParentGameObjectToPlace = other.gameObject;

            _numColliders++;

            if (_numColliders == 1)
            {
                PlaceHologram();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Spatial Mapping"))
        {
            _numColliders--;

            if (_numColliders == 0)
            {
                UnPlaceHologram();
            }
        }
    }

    private void PlaceHologram()
    {
        hologramContentObject.transform.SetParent(gameObject.transform.parent);

        _tapToPlaceScript.enabled = true;
    }

    private void UnPlaceHologram()
    {
        hologramContentObject.transform.SetParent(gameObject.transform);

        _tapToPlaceScript.ParentGameObjectToPlace = gameObject;

        _tapToPlaceScript.enabled = false;
    }
}
