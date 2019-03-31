using System;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class Settings : MonoBehaviour {
    public FadeEffectManager fadeEffectManagerScript;

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
        PositionTo.OnTriggerStateApply();

        fadeEffectManagerScript.StartFade(FadeEffectManager.FadeDirection.FadeIn);
    }
}
