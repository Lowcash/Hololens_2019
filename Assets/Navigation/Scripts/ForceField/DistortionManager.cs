using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistortionManager : MonoBehaviour {
    private Renderer _renderer;

    private void Awake() {
        _renderer = GetComponent<Renderer>();    
    }

    public void SetDistortionStrength(int value) {
        _renderer.material.SetFloat("_DistortStrength", (value / 100.0f));
    }

    public void SetDistortionTime( int value ) {
        _renderer.material.SetFloat("_DistortTimeFactor", (value / 100.0f));
    }
}
