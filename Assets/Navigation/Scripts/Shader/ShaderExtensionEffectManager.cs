using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderExtensionEffectManager : MonoBehaviour {
    private Renderer _renderer;

    public Shader shaderToModify;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
    }

    public void SetTransparency(float transparency)
    {
        if (_renderer.material.HasProperty("_Color"))
        {
            Color color = _renderer.material.GetColor("_Color");

            color.a = transparency;

            _renderer.material.SetColor("_Color", color);
        }
        else
        {

#if UNITY_EDITOR
            Debug.Log("Wrong shader was set!");
#endif
        }
    }
}
