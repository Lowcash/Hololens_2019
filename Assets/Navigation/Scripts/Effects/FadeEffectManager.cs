using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeEffectManager : MonoBehaviour {
    public enum FadeDirection { FadeIn, FadeOut }

    public float speedOfTransition = 0.05f;

    private float _alphaValue = 1.0f;

    private Coroutine _fadeInCoroutine;
    private Coroutine _fadeOutCoroutine;

    private FadeDirection _nextFade = FadeDirection.FadeOut;

    private readonly List<Color> _childrenDefaultColors = new List<Color>();
    private readonly List<Text> _childrenTexts = new List<Text>();
    private readonly List<Renderer> _childrenRenderers = new List<Renderer>();
    private readonly List<ShaderExtensionEffectManager> _childrenShaderExtensionManagers = new List<ShaderExtensionEffectManager>();

    private void Start() {
        var allChildren = LayerHelper.FindObjectsInLayer(gameObject, LayerName.UI);

        foreach (var child in allChildren) {
            Renderer renderer;

            if ((renderer = child.GetComponent<Renderer>()) != null) {
                _childrenRenderers.Add(renderer);
                _childrenDefaultColors.Add(renderer.material.color);

                ShaderExtensionEffectManager shaderExtensionEffectManager;

                if ((shaderExtensionEffectManager = child.GetComponent<ShaderExtensionEffectManager>()) != null) {
                    _childrenShaderExtensionManagers.Add(shaderExtensionEffectManager);
                }
            }

            Text text;

            if ((text = child.GetComponent<Text>()) != null)
                _childrenTexts.Add(text);
        }
    }

    public void StartFade() {
        StartFade(_nextFade);
    }

    public void StartFade( FadeDirection fadeDirectionState ) {
        if (fadeDirectionState == FadeDirection.FadeOut) {
            if (_fadeInCoroutine != null) { StopCoroutine(_fadeInCoroutine); }

            _fadeOutCoroutine = StartCoroutine(FadeOut());

            _nextFade = FadeDirection.FadeIn;
        } else if (fadeDirectionState == FadeDirection.FadeIn) {
            if (!gameObject.activeSelf) {
                gameObject.SetActive(true);

                if (_fadeOutCoroutine != null) { StopCoroutine(_fadeOutCoroutine); }

                _fadeInCoroutine = StartCoroutine(FadeIn());

                _nextFade = FadeDirection.FadeOut;
            }
        }
    }

    public FadeDirection GetFadeState() {
        return _nextFade == FadeDirection.FadeIn ? FadeDirection.FadeOut : FadeDirection.FadeIn;
    }

    private IEnumerator FadeIn() {
        gameObject.SetActive(true);

        for (; _alphaValue <= 1; _alphaValue += speedOfTransition) {
            for (int i = 0; i < _childrenRenderers.Count; i++) {
                var color = _childrenRenderers[i].material.color;

                if (_alphaValue < _childrenDefaultColors[i].a && color.a < 1) {
                    color.a = _alphaValue;

                    _childrenRenderers[i].material.color = color;
                }
            }

            for (int i = 0; i < _childrenTexts.Count; i++) {
                var color = _childrenTexts[i].color;

                if (color.a < 1) {
                    color.a = _alphaValue;

                    _childrenTexts[i].color = color;
                }
            }

            yield return new WaitForSeconds(0.05f);
        }

        _childrenShaderExtensionManagers.ForEach(m => m.ChangeRenderingModeTo(RenderingMode.Default));
    }

    private IEnumerator FadeOut() {
        _childrenShaderExtensionManagers.ForEach(m => m.ChangeRenderingModeTo(RenderingMode.Transparent));

        for (; _alphaValue >= -speedOfTransition; _alphaValue -= speedOfTransition) {
            for (int i = 0; i < _childrenRenderers.Count; i++) {
                var color = _childrenRenderers[i].material.color;

                if (_alphaValue < _childrenDefaultColors[i].a && color.a > 0) {
                    color.a = _alphaValue;

                    _childrenRenderers[i].material.color = color;
                }
            }

            for (int i = 0; i < _childrenTexts.Count; i++) {
                var color = _childrenTexts[i].color;

                if (color.a > 0) {
                    color.a = _alphaValue;

                    _childrenTexts[i].color = color;
                }
            }

            yield return new WaitForSeconds(0.05f);
        }

        gameObject.SetActive(false);
    }
}
