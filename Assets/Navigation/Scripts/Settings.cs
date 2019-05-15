using System;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class Settings : MonoBehaviour {
    public enum SettingLayerState { MAIN, DISTORTION, PULSE }

    [Header("Fade effects")]
    public FadeEffectManager fullPanelFade;
    public FadeEffectManager mainPanelFade;
    public FadeEffectManager distorsionPanelFade;
    public FadeEffectManager pulsePanelFade;

    [Header("Positioning")]
    public PositionTo positionToScript;

    private SettingLayerState _actualSettingLayerState = SettingLayerState.MAIN;

    private void Awake() {
        CustomInputControl.OnMenuButtonClicked += OnMenuButtonClicked;
    }

    private void OnRestartButtonClick() {
        GM.OnRestart();
    }

    private void OnHideSettingsButtonClick() {
        fullPanelFade.StartFade(FadeEffectManager.FadeDirection.FadeOut);
    }

    private void OnMenuButtonClicked(InputHand inputHand) {
        if (fullPanelFade.GetFadeState() == FadeEffectManager.FadeDirection.FadeOut) {
            positionToScript.ApplyPositioning();

            fullPanelFade.StartFade(FadeEffectManager.FadeDirection.FadeIn);
        }
    }

    public void OnDistorsionButtonClick() {
        mainPanelFade.StartFade(FadeEffectManager.FadeDirection.FadeOut);
        distorsionPanelFade.StartFade(FadeEffectManager.FadeDirection.FadeIn);

        _actualSettingLayerState = SettingLayerState.DISTORTION;
    }

    public void OnPulseButtonClick() {
        mainPanelFade.StartFade(FadeEffectManager.FadeDirection.FadeOut);
        pulsePanelFade.StartFade(FadeEffectManager.FadeDirection.FadeIn);

        _actualSettingLayerState = SettingLayerState.PULSE;
    }

    public void OnBackButtonClick() {
        mainPanelFade.StartFade(FadeEffectManager.FadeDirection.FadeIn);

        switch (_actualSettingLayerState) {
            case SettingLayerState.DISTORTION:
                distorsionPanelFade.StartFade(FadeEffectManager.FadeDirection.FadeOut);
            break;
            case SettingLayerState.PULSE:
                pulsePanelFade.StartFade(FadeEffectManager.FadeDirection.FadeOut);
            break;
        }
    }
}
