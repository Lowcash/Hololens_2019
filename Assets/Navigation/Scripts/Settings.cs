using System;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Examples.InteractiveElements;
using HoloToolkit.Unity.UX;

public class Settings : MonoBehaviour {
    public enum SettingLayerState { MAIN, DISTORTION, PULSE }

    [Header("Fade effects")]
    public FadeEffectManager fullPanelFade;
    public FadeEffectManager mainPanelFade;
    public FadeEffectManager distorsionPanelFade;
    public FadeEffectManager pulsePanelFade;

    [Header("Positioning")]
    public PositionTo positionToScript;

    [Header("Distortion")]
    public GameObject distortionObject;
    public SliderGestureControl strengthSliderControl;
    public SliderGestureControl timeSliderControl;

    [Header("Pulsing")]
    public GameObject pulsingObject;
    public InteractiveToggle speed1;
    public InteractiveToggle speed2;
    public InteractiveToggle speed3;
    public InteractiveToggle speed4;

    [Header("Resize")]
    public GameObject boundingBoxRigObject;

    private bool _isResizeMode = false;

    private GameObject _appBar;

    private DistortionManager _distortionManager;
    private PulsingEffectManager _pulsingEffectManager;
    private SettingLayerState _actualSettingLayerState = SettingLayerState.MAIN;

    private void Awake() {
        CustomInputControl.OnMenuButtonClicked += OnMenuButtonClicked;
    }

    private void Start() {
        if (distortionObject = GameObject.FindGameObjectWithTag("DistortionEffect"))
            _distortionManager = distortionObject.GetComponent<DistortionManager>();

        if (pulsingObject = GameObject.FindGameObjectWithTag("PulsingEffect"))
            _pulsingEffectManager = pulsingObject.GetComponent<PulsingEffectManager>();
    }

    private void OnRestartButtonClick() {
        GM.OnRestart();
    }
    public void OnStickerButtonClick() {
        GM.OnStickerGenerate();
    }

    #region Fade
    private void OnHideSettingsButtonClick() {
        fullPanelFade.StartFade(FadeEffectManager.FadeDirection.FadeOut);
    }

    private void OnMenuButtonClicked( InputHand inputHand ) {
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

    public void OnResizeButtonClick() {
        if (!_isResizeMode) {
            boundingBoxRigObject.transform.SetParent(transform.parent);
            gameObject.transform.SetParent(boundingBoxRigObject.transform);

            boundingBoxRigObject.SetActive(true);

            boundingBoxRigObject.GetComponent<BoundingBoxRig>().boxInstance.Target.GetComponent<BoundingBoxRig>().Activate();

            _isResizeMode = true;
        } else {
            boundingBoxRigObject.SetActive(false);

            gameObject.transform.SetParent(boundingBoxRigObject.transform.parent);
            boundingBoxRigObject.transform.SetParent(gameObject.transform);

            boundingBoxRigObject.GetComponent<BoundingBoxRig>().boxInstance.Target.GetComponent<BoundingBoxRig>().Deactivate();

            _isResizeMode = false;
        }
    }
    #endregion

    #region Distortion
    public void OnDistortionSet( bool isSet ) {
        distortionObject.SetActive(isSet);
    }

    public void OnDistortionStrengthChange() {
        _distortionManager.SetDistortionStrength((int)strengthSliderControl.SliderValue);
    }

    public void OnDistortionTimeChange() {
        _distortionManager.SetDistortionTime((int)timeSliderControl.SliderValue);
    }
    #endregion

    #region Pulsing
    public void OnPulsingSet( bool isSet ) {
        _pulsingEffectManager.PulsingEffectSet(isSet);
    }

    public void OnPulsingSpeedSet( int value ) {
        speed1.SetSelection(false);
        speed2.SetSelection(false);
        speed3.SetSelection(false);
        speed4.SetSelection(false);

        switch (value) {
            case 1: speed1.SetSelection(true); break;
            case 2: speed2.SetSelection(true); break;
            case 3: speed3.SetSelection(true); break;
            case 4: speed4.SetSelection(true); break;
        }

        _pulsingEffectManager.PulsingSpeedSet(value);
    }
    #endregion
}
