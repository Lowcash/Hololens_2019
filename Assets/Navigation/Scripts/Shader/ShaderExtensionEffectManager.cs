using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RenderingMode { Default, Opaque = 0, TransparentCutout, Transparent, PremultipliedTransparent, Additive, Custom }

public enum MaterialType { Material, SharedMaterial }

public class ShaderExtensionEffectManager : MonoBehaviour {
    public Shader shaderToModify;

    [Header("Change materials")]
    public MaterialType materialType = MaterialType.Material;

    public string renderModePropertyName;

    public Material defaultMaterial;
    public Material switchToMaterial;

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

    public void ChangeRenderingModeTo( RenderingMode renderingMode ) {
        if (_renderer.material.HasProperty(renderModePropertyName)) {
            switch (materialType) {
                case MaterialType.Material: {
                    _renderer.material = (renderingMode == RenderingMode.Default) ? defaultMaterial : switchToMaterial;
                }

                break;

                case MaterialType.SharedMaterial: {
                    _renderer.sharedMaterial = (renderingMode == RenderingMode.Default) ? defaultMaterial : switchToMaterial;
                }

                break;
            }
        }
    }
}
