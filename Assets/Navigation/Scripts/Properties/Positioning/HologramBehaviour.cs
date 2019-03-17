using UnityEngine;
using System.Collections.Generic;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.SpatialMapping;

public class HologramBehaviour : PositioningProperty, IInputHandler
{
    [Header("Hologram objects")]
    public GameObject hologramContentObject;
    public GameObject hologramDraggableObject;

    [Header("Settings")]
    public float directionToPlaceHologram = 1.0f;

    private int _numColliders = 0;

    private bool _isPlaced = false;

    private Ray _ray;

    private Rigidbody _rb;
    private Collider _tapTriggerCollider;

    private Interpolator _interpolatorScript;
    private TapToPlace _tapToPlaceScript;
    private HandDraggable _handDraggableScript;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _tapTriggerCollider = gameObject.GetComponent<Collider>();

        _interpolatorScript = hologramContentObject.GetComponent<Interpolator>();
        _tapToPlaceScript = hologramContentObject.GetComponent<TapToPlace>();
        _handDraggableScript = hologramDraggableObject.GetComponent<HandDraggable>();
    }

    private void Start()
    {
        _ray = new Ray();

        _tapTriggerCollider.enabled = false;
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

        //var isSpatialLayerHitted = Physics.Raycast(_ray, out _hit, directionToPlaceHologram, (int)LayerHelper.LayerName.RaycastSpatialMapping);


        //if (isSpatialLayerHitted)
        //{
        //    //Debug.Log(Vector3.Dot(Camera.main.transform.forward, _ray.origin + _ray.direction));
        //    //Debug.Log(Camera.main.transform.forward);
        //}

        if (_numColliders == 0)
        {
            _interpolatorScript.SetTargetLocalPosition(gameObject.transform.position);
        }

        //Debug.Log(_numColliders);
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

        _isPlaced = true;
    }

    private void UnPlaceHologram()
    {
        hologramContentObject.transform.SetParent(gameObject.transform);

        _tapToPlaceScript.ParentGameObjectToPlace = gameObject;

        _tapToPlaceScript.enabled = false;

        _isPlaced = false;
    }

    public void OnInputDown(InputEventData eventData)
    {
        _rb = gameObject.AddComponent<Rigidbody>();

        _rb.isKinematic = true;
        _tapTriggerCollider.enabled = true;
    }

    public void OnInputUp(InputEventData eventData)
    {
        hologramContentObject.layer = (int)LayerHelper.LayerName.UI;

        Destroy(_rb);

        _interpolatorScript.enabled = false;

        _tapToPlaceScript.IsBeingPlaced = false;
        _tapTriggerCollider.enabled = false;

        _numColliders = 0;

        if (_isPlaced)
        {
            gameObject.transform.localPosition = hologramContentObject.transform.localPosition;
            hologramContentObject.transform.SetParent(gameObject.transform);
        }
    }
}
