using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionTo : PositioningProperty {
    public Vector3 translation;

    public List<ObjectHelper.ObjectLifeState> applyOnStates = new List<ObjectHelper.ObjectLifeState>() { ObjectHelper.ObjectLifeState.Awake };

    [Header("Player")]
    public bool positioningFromPlayer = true;
    public bool applyPlayerRotation = true;

    private bool _isApplyOnAwakeState = false;
    private bool _isApplyOnStartState = false;
    private bool _isApplyOnUpdateState = false;

    private void Awake() {
        _isApplyOnAwakeState = applyOnStates.Contains(ObjectHelper.ObjectLifeState.Awake);
        _isApplyOnStartState = applyOnStates.Contains(ObjectHelper.ObjectLifeState.Start);
        _isApplyOnUpdateState = applyOnStates.Contains(ObjectHelper.ObjectLifeState.Life);

        if (_isApplyOnAwakeState) {
            SetPosition();

            if (applyPlayerRotation) { transform.SetParent(Camera.main.transform); }
        }
    }

    private void Start() {
        if (_isApplyOnStartState) {
            SetPosition();

            if (applyPlayerRotation) { transform.SetParent(Camera.main.transform); }
        }
    }

    private void Update() {
        if (_isApplyOnUpdateState) { SetPosition(); }
    }

    private void SetPosition() {
        var position = transform.position;

        if (positioningFromPlayer) {
            position = Camera.main.transform.position + translation;
        }

        transform.position = position;
    }
}
