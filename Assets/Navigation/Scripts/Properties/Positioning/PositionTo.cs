using System;
using UnityEngine;

public class PositionTo : PositioningProperty {
    public static Action OnTriggerStateApply;

    public bool isApplyOnAwakeState = false;
    public bool isApplyOnStartState = false;
    public bool isApplyOnUpdateState = false;
    public bool isApplyOnTriggerState = false;

    public bool isApplyOnAwakePlayerRotation = false;
    public bool isApplyOnStartPlayerRotation = false;
    public bool isApplyOnUpdatePlayerRotation = false;
    public bool isApplyOnTriggerPlayerRotation = false;

    public Vector3 awakeStateTranslation;
    public Vector3 startStateTranslation;
    public Vector3 updateStateTranslation;
    public Vector3 triggerStateTranslation;

    public bool positioningFromPlayer = true;

    private void Awake() {
        if (isApplyOnAwakeState) {
            SetPosition(awakeStateTranslation);

            if (isApplyOnAwakePlayerRotation) { transform.SetParent(Camera.main.transform); }
        }

        if (isApplyOnTriggerState) { OnTriggerStateApply += OnTriggerStateActivate; }
    }

    private void Start() {
        if (isApplyOnStartState) {
            SetPosition(startStateTranslation);

            if (isApplyOnStartPlayerRotation) { transform.SetParent(Camera.main.transform); }
        }
    }

    private void Update() {
        if (isApplyOnUpdateState) { SetPosition(updateStateTranslation); }
    }

    private void OnTriggerStateActivate() {
        if (isApplyOnTriggerState) {
            if (isApplyOnTriggerPlayerRotation) {
                var parent = transform.parent;

                transform.SetParent(Camera.main.transform);

                transform.localRotation = Quaternion.identity;
                transform.localPosition = triggerStateTranslation;

                transform.SetParent(parent);
            } else {
                SetPosition(triggerStateTranslation);
            }           
        }
    }

    private void SetPosition(Vector3 translation) {
        var position = transform.position;

        if (positioningFromPlayer) {
            position = Camera.main.transform.position + translation;
        }

        transform.position = position;
    }
}
