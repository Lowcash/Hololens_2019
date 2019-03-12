using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;
using HoloToolkit.Unity.SpatialMapping;


public class Settings : MonoBehaviour {
    private TapToPlace tapToPlaceScript;
    private Interpolator interpolatorScript;

    private void Awake()
    {
        tapToPlaceScript = GetComponent<TapToPlace>();
        interpolatorScript = GetComponent<Interpolator>();
    }

    public void OnHideSettingsClick()
    {
        gameObject.SetActive(false);
        
        Debug.Log("Setting menu was hidden!");
    }
}
