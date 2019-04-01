using System;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class Settings : MonoBehaviour {
    public FadeEffectManager fadeEffectManagerScript;
    public PositionTo positionToScript;

    private void Awake() {
        CustomInputControl.OnMenuButtonClicked += OnMenuButtonClicked;
    }

    private void OnRestartButtonClick() {
        GM.OnRestart();
    }

    private void OnHideSettingsButtonClick() {
        fadeEffectManagerScript.StartFade(FadeEffectManager.FadeDirection.FadeOut);
    }

    private void OnMenuButtonClicked(InputHand inputHand) {
        if (fadeEffectManagerScript.GetFadeState() == FadeEffectManager.FadeDirection.FadeOut) {
            positionToScript.ApplyPositioning();

            fadeEffectManagerScript.StartFade(FadeEffectManager.FadeDirection.FadeIn);
        }
    }
}
