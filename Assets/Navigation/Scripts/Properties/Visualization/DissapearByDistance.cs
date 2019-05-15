using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissapearByDistance : VisualizingProperty
{
    public GameObject objectRenderWrapper;
    public GameObject objectRender;

    public ShaderExtensionEffectManager shaderExtensionEffectScript;

    [Header("Settings")]
    public float distanceToStartDissapear;

    private float _actualDistance;
    private float _previousDistance;

    public void SetActualDistance(float distance)
    {
        _actualDistance = distance;
    }

    private void Update()
    {
        if (_previousDistance != _actualDistance)
        {
            //SetObjectTransparency(Mathf.Max(GetDotRotation(), GetDistance()));
            SetObjectTransparency(GetDistance());

            _previousDistance = _actualDistance;
        }
    }

    private void SetObjectTransparency(float transparency)
    {
        shaderExtensionEffectScript.SetTransparency(transparency);
    }

    private float GetDistance()
    {
        return _actualDistance / distanceToStartDissapear;
    }

    private float GetDotRotation()
    {
        var localRotation = objectRenderWrapper.transform.localRotation;
        localRotation.w = Mathf.Abs(localRotation.w);

        return 1.0f - Quaternion.Dot(Quaternion.identity, localRotation);
    }
}
