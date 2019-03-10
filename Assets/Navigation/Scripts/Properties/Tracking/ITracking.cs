using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITracking {

    void StartTracking();

    void StopTracking();

    void SetTrackingTo(GameObject gameObject);
}
