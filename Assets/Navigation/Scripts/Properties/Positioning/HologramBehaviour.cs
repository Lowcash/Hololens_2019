using UnityEngine;
using System.Collections.Generic;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.SpatialMapping;
using System;

public class HologramClickEventArgs : EventArgs {
    public bool isClicked;
}

public class HologramBehaviour : PositioningProperty, IInputHandler {
    public static EventHandler<HologramClickEventArgs> OnClicked;

    [Header("Hologram objects")]
    public GameObject hologramContentObject;
    public GameObject hologramDraggableObject;

    [Header("Settings")]
    public float directionToPlaceHologram = 1.0f;

    private bool _isPlaced = false;

    private int _numColliders;

    private Ray _ray = new Ray();
    private RaycastHit _hit;
    private RaycastHit _cameraHit;

    private Rigidbody _rb;
    private Collider _tapTriggerCollider;

    private Interpolator _interpolatorScript;
    private TapToPlace _tapToPlaceScript;

    private List<GameObject> _childObjects = new List<GameObject>();

    private void Awake() {
        _tapTriggerCollider = gameObject.GetComponent<Collider>();

        _interpolatorScript = hologramContentObject.GetComponent<Interpolator>();
        _tapToPlaceScript = hologramContentObject.GetComponent<TapToPlace>();
    }

    private void Start() {
        _childObjects.AddRange(LayerHelper.FindObjectsInLayer(hologramContentObject, LayerName.UI));

        PrepareHologramForButtons();
    }

    private void Update() {
        if (_isPlaced && _numColliders == 0) {
            _ray.origin = Camera.main.transform.position;
            _ray.direction = hologramDraggableObject.transform.position - Camera.main.transform.position;

            var distance = Vector3.Distance(Camera.main.transform.position, hologramDraggableObject.transform.position);

            Debug.DrawLine(_ray.origin, _ray.origin + (_ray.direction * distance), Color.blue);

            if (Physics.Raycast(_ray, out _hit, distance)) {
                if(_hit.transform.name == hologramDraggableObject.transform.name) {
                    UnPlaceHologram();
                }  
            }
        }

        UpdateInterpolatorPosition();
    }

    private void OnTriggerEnter( Collider other ) {
        if (other.gameObject.layer == (int)LayerName.SpatialMapping) {
            _tapToPlaceScript.ParentGameObjectToPlace = other.gameObject;

            _numColliders++;

            if (_numColliders == 1) {
                PlaceHologram();
            }
        }
    }

    private void OnTriggerExit( Collider other ) {
        if (other.gameObject.layer == (int)LayerName.SpatialMapping) {
            _numColliders--;
        }
    }

    private void PlaceHologram() {
        DetachContentFromDragging();

        _tapToPlaceScript.enabled = true;

        _isPlaced = true;
    }

    private void UnPlaceHologram() {
        AttachContentToDragging();

        _tapToPlaceScript.ParentGameObjectToPlace = gameObject;

        _tapToPlaceScript.enabled = false;

        _isPlaced = false;
    }

    public void OnInputDown( InputEventData eventData ) {
        OnClicked(this, new HologramClickEventArgs() { isClicked = true });

        PrepareHologramForDragging();

        LayerHelper.SetObjetsLayer(ref _childObjects, LayerName.Navigation);
    }

    public void OnInputUp( InputEventData eventData ) {
        OnClicked(this, new HologramClickEventArgs() { isClicked = false });

        PrepareHologramForButtons();

        if (_isPlaced) {
            UpdateTapTriggerTransform();

            AttachContentToDragging();
        }

        LayerHelper.SetObjetsLayer(ref _childObjects, LayerName.UI);
    }

    private void UpdateTapTriggerTransform() {
        gameObject.transform.localPosition = hologramContentObject.transform.localPosition;
        gameObject.transform.localRotation = hologramContentObject.transform.localRotation;
    }

    private void UpdateInterpolatorPosition() {
        if (!_isPlaced)
            _interpolatorScript.SetTargetLocalPosition(gameObject.transform.position);
    }

    private void PrepareHologramForButtons() {
        ResetCollidersCounter();

        if (_rb) { Destroy(_rb); }

        _tapToPlaceScript.IsBeingPlaced = false;
        _tapTriggerCollider.enabled = false;
    }

    private void PrepareHologramForDragging() {
        if (!_rb) {
            _rb = gameObject.AddComponent<Rigidbody>();
            _rb.isKinematic = true;
        }

        _tapTriggerCollider.enabled = true;
    }

    private void AttachContentToDragging() {
        hologramContentObject.transform.SetParent(gameObject.transform);
    }

    private void DetachContentFromDragging() {
        hologramContentObject.transform.SetParent(gameObject.transform.parent);
    }

    private void ResetCollidersCounter() {
        _numColliders = 0;
    }
}
