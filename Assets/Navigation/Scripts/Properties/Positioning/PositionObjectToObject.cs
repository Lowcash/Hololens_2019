using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionObjectToObject : PositioningProperty {
    public GameObject objectToPosition;
    public GameObject positionTo;

    private void Update()
    {
        objectToPosition.transform.position = positionTo.transform.position;
    }
}
