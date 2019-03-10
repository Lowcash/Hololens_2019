using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissapearByDistance : VisualizingProperty
{
    public float distanceToStartDissapear;

    public GameObject objectRenderWrapper;
    public GameObject objectRender;

    private float _actualDistance;

    private Renderer _objectRenderer;

    public void SetActualDistance(float distance)
    {
        _actualDistance = distance;
    }

    private void Start()
    {
        _objectRenderer = objectRender.GetComponent<Renderer>();
    }

    private void Update()
    {
        var localRotation = objectRenderWrapper.transform.localRotation;
        localRotation.w = Mathf.Abs(localRotation.w);

        float dotRotation = 1.0f - Quaternion.Dot(Quaternion.identity, localRotation);
        float distance = _actualDistance / distanceToStartDissapear;

        if (_actualDistance < distanceToStartDissapear)
        {
            var objectColor = _objectRenderer.material.GetColor("_Color");

            objectColor.a = Mathf.Max(dotRotation, distance);

            _objectRenderer.material.SetColor("_Color", objectColor);
        }
    }
}
