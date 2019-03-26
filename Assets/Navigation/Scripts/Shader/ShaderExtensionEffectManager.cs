using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShaderPropertyType {
    Float
}

public class ShaderExtensionEffectManager : MonoBehaviour {
    public Shader shaderToModify;

    private Renderer _renderer;

    private void Awake() {
        _renderer = GetComponent<Renderer>();
    }

    public void SetTransparency( float transparency ) {
        if (_renderer.material.HasProperty("_Color")) {
            var color = _renderer.material.GetColor("_Color");

            color.a = transparency;

            _renderer.material.SetColor("_Color", color);
        } else {

#if UNITY_EDITOR
            Debug.Log("Wrong shader was set!");
#endif
        }
    }

    public void SetPropertyArray( string name, List<float> values, ShaderPropertyType propertyType ) {
        switch (propertyType) {
            case ShaderPropertyType.Float:
                _renderer.material.SetFloatArray(name, values);

                break;
        }
    }

    public void SetProperty( string name, float value, ShaderPropertyType propertyType ) {
        if (_renderer.material.HasProperty(name)) {
            switch (propertyType) {
                case ShaderPropertyType.Float:
                    _renderer.material.SetFloat(name, value);

                    break;
            }
        } else {

#if UNITY_EDITOR
            Debug.Log("Wrong shader was set!");
#endif
        }
    }
}
