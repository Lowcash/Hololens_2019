using HoloToolkit.Examples.InteractiveElements;
using HoloToolkit.Unity.UX;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StickerObjectTransformEventArgs : EventArgs {
    public GameObject stickerObject;
    public Transform stickerTransform;
    public GameObject outputTextObject;
}

public class Sticker : MonoBehaviour {
    public static EventHandler<StickerObjectTransformEventArgs> OnStickerObjectTransform;
    public static EventHandler<StickerObjectTransformEventArgs> OnStickerObjectDelete;
    public GameObject outputText;

    public enum StickerLayerState { MAIN, COLOR }

    [Header("Fade effects")]
    public FadeEffectManager mainPanelFade;
    public FadeEffectManager colorPanelFade;

    [Header("Color changing")]
    public SliderGestureControl redSliderControl;
    public SliderGestureControl greenSliderControl;
    public SliderGestureControl blueSliderControl;

    [Header("Resize")]
    public GameObject boundingBoxRigObject;

    public List<Renderer> renderers = new List<Renderer>();

    private bool _isResizeMode = false;

    private void Start() {
        redSliderControl.SetSliderValue(255.0f);
        greenSliderControl.SetSliderValue(255.0f);
        blueSliderControl.SetSliderValue(255.0f);

        colorPanelFade.gameObject.SetActive(false);
    }

    public void OnDeleteButtonClick() {
        OnStickerObjectDelete(this, new StickerObjectTransformEventArgs() { stickerObject = gameObject, stickerTransform = gameObject.transform, outputTextObject = outputText });
    }

    public void OnEditButtonClick() {
        OnStickerObjectTransform(this, new StickerObjectTransformEventArgs() { stickerObject = gameObject, stickerTransform = gameObject.transform, outputTextObject = outputText });
    }

    public void OnChangeModeButtonClick() {
        colorPanelFade.StartFade(FadeEffectManager.FadeDirection.FadeIn);

        mainPanelFade.StartFade(FadeEffectManager.FadeDirection.FadeOut);
    }

    public void OnResizeButtonClick() {
        if (!_isResizeMode) {
            boundingBoxRigObject.transform.SetParent(transform.parent);
            gameObject.transform.SetParent(boundingBoxRigObject.transform);

            boundingBoxRigObject.GetComponent<BoundingBoxRig>().boxInstance.Target.GetComponent<BoundingBoxRig>().Activate();

            _isResizeMode = true;
        } else {
            gameObject.transform.SetParent(boundingBoxRigObject.transform.parent);
            boundingBoxRigObject.transform.SetParent(gameObject.transform);

            boundingBoxRigObject.GetComponent<BoundingBoxRig>().boxInstance.Target.GetComponent<BoundingBoxRig>().Deactivate();

            _isResizeMode = false;
        }
    }

    public void OnBackButtonClick() {
        mainPanelFade.StartFade(FadeEffectManager.FadeDirection.FadeIn);

        colorPanelFade.StartFade(FadeEffectManager.FadeDirection.FadeOut);
    }

    public void OnColorSliderChange() {
        renderers.ForEach(r => r.material.SetColor("_Color", new Color(
            redSliderControl.SliderValue / 255.0f,
            greenSliderControl.SliderValue / 255.0f,
            blueSliderControl.SliderValue / 255.0f,
            0.2f)
            )
        );
    }
}
