using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RenderingMode { Default, Opaque = 0, TransparentCutout, Transparent, PremultipliedTransparent, Additive, Custom }

public class ShaderExtensionEffectManager : MonoBehaviour {
    public Shader shaderToModify;

    [Header("Change materials")]
    public Material defaultMaterial;
    public Material switchToMaterial;

    private Renderer _renderer;

    private RenderingMode defaultRenderingMode;

    private void Awake() {
        _renderer = GetComponent<Renderer>();

        if (_renderer.material.HasProperty("_Mode")) {
            defaultRenderingMode = (RenderingMode)_renderer.material.GetInt("_Mode");
        }
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

    public void ChangeRenderingModeTo( RenderingMode renderingMode ) {
        if (_renderer.material.HasProperty("_Mode")) {
            if (renderingMode == RenderingMode.Default) {
                _renderer.material = defaultMaterial;

                //_renderer.material.SetInt("_Mode", (int)defaultRenderingMode);
                //_renderer.material.SetFloat("_RenderQueueOverride", -1.0f);
            } else {
                _renderer.material = switchToMaterial;
                //_renderer.material.SetInt("_Mode", (int)renderingMode);
                //_renderer.material.SetFloat("_RenderQueueOverride", 0.0f);
            }
        }
    }
}
