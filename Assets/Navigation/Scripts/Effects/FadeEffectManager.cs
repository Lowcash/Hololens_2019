using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeEffectManager : MonoBehaviour {
    public enum FadeDirection { FadeIn, FadeOut }

    private float _alphaValue = 1;

    private FadeDirection _nextFade = FadeDirection.FadeOut;

    private Coroutine _fadeInCoroutine;
    private Coroutine _fadeOutCoroutine;

    private readonly List<Renderer> _allChildRenderers = new List<Renderer>();

    private void Awake() {
        Settings.OnClickHideSettings += StartFade;
    }

    private void Start() {
        var allChildren = LayerHelper.FindObjectsInLayer(gameObject, LayerName.UI);

        foreach (var child in allChildren) {
            Renderer renderer;

            if ((renderer = child.GetComponent<Renderer>()) != null) {
                _allChildRenderers.Add(renderer);
            }
        }
    }

    private void StartFade( object sender, EventArgs e ) {
        if (_nextFade == FadeDirection.FadeOut) {
            if (_fadeInCoroutine != null) { StopCoroutine(_fadeInCoroutine); }

            _fadeOutCoroutine = StartCoroutine(FadeOut());

            _nextFade = FadeDirection.FadeIn;
        } else if (_nextFade == FadeDirection.FadeIn) {
            if (_fadeOutCoroutine != null) { StopCoroutine(_fadeOutCoroutine); }

            _fadeInCoroutine = StartCoroutine(FadeIn());

            _nextFade = FadeDirection.FadeOut;
        }
    }

    private IEnumerator FadeIn() {
        for (; _alphaValue <= 1; _alphaValue += 0.05f) {
            foreach (var renderer in _allChildRenderers) {
                var color = renderer.material.color;

                color.a = _alphaValue;

                renderer.material.color = color;
            }

            yield return new WaitForSeconds(0.05f);
        }
    }

    private IEnumerator FadeOut() {
        for (; _alphaValue >= -0.05f; _alphaValue -= 0.05f) {
            foreach (var renderer in _allChildRenderers) {
                var color = renderer.material.color;

                color.a = _alphaValue;

                renderer.material.color = color;
            }

            yield return new WaitForSeconds(0.05f);
        }
    }
}
