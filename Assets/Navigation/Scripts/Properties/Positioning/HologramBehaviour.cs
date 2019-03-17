using UnityEngine;
using System.Collections.Generic;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.SpatialMapping;
using System;

public class HologramClickEventArgs : EventArgs
{
    public bool value;
}

public class HologramBehaviour : PositioningProperty, IInputHandler
{
    public static EventHandler<HologramClickEventArgs> OnClicked;

    [Header("Hologram objects")]
    public GameObject hologramContentObject;
    public GameObject hologramDraggableObject;

    [Header("Settings")]
    public float directionToPlaceHologram = 1.0f;

    private bool _isPlaced = false;

    private int _numColliders;

    private Ray _ray = new Ray();

    private Rigidbody _rb;
    private Collider _tapTriggerCollider;

    private Interpolator _interpolatorScript;
    private TapToPlace _tapToPlaceScript;
    private HandDraggable _handDraggableScript;

    private List<GameObject> _childObjects = new List<GameObject>();

    private void Awake()
    {
        _tapTriggerCollider = gameObject.GetComponent<Collider>();

        _interpolatorScript = hologramContentObject.GetComponent<Interpolator>();
        _tapToPlaceScript = hologramContentObject.GetComponent<TapToPlace>();
        _handDraggableScript = hologramDraggableObject.GetComponent<HandDraggable>();
    }

    private void Start()
    {
        _childObjects.AddRange(LayerHelper.FindObjectsInLayer(hologramContentObject, LayerHelper.LayerName.UI));

        PrepareHologramForButtons();
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

        UpdateInterpolatorPosition();
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
        DetachContentFromDragging();

        _tapToPlaceScript.enabled = true;

        _isPlaced = true;
    }

    private void UnPlaceHologram()
    {
        AttachContentToDragging();

        _tapToPlaceScript.ParentGameObjectToPlace = gameObject;

        _tapToPlaceScript.enabled = false;

        _isPlaced = false;
    }

    public void OnInputDown(InputEventData eventData)
    {
        OnClicked(this, new HologramClickEventArgs() { value = true });

        PrepareHologramForDragging();

        LayerHelper.SetObjetsLayer(ref _childObjects, LayerHelper.LayerName.IgnoreRaycast);
    }

    public void OnInputUp(InputEventData eventData)
    {
        OnClicked(this, new HologramClickEventArgs() { value = false });

        PrepareHologramForButtons();

        if (_isPlaced)
        {
            UpdateTapTriggerTransform();

            AttachContentToDragging();
        }

        LayerHelper.SetObjetsLayer(ref _childObjects, LayerHelper.LayerName.UI);
    }

    private void UpdateTapTriggerTransform()
    {
        gameObject.transform.localPosition = hologramContentObject.transform.localPosition;
        gameObject.transform.localRotation = hologramContentObject.transform.localRotation;
    }

    private void UpdateInterpolatorPosition()
    {
        if (_numColliders == 0)
        {
            _interpolatorScript.SetTargetLocalPosition(gameObject.transform.position);
        }
    }

    private void PrepareHologramForButtons()
    {
        ResetCollidersCounter();

        if (_rb)
        {
            Destroy(_rb);
        }

        _tapToPlaceScript.IsBeingPlaced = false;
        _tapTriggerCollider.enabled = false;
    }

    private void PrepareHologramForDragging()
    {
        if (!_rb)
        {
            _rb = gameObject.AddComponent<Rigidbody>();
            _rb.isKinematic = true;
        }

        _tapTriggerCollider.enabled = true;
    }

    private void AttachContentToDragging()
    {
        hologramContentObject.transform.SetParent(gameObject.transform);
    }

    private void DetachContentFromDragging()
    {
        hologramContentObject.transform.SetParent(gameObject.transform.parent);
    }

    private void ResetCollidersCounter()
    {
        _numColliders = 0;
    }
}
