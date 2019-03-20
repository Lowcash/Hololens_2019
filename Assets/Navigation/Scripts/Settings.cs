using System;
using UnityEngine;

public class Settings : MonoBehaviour {
    public void OnRestartButtonClick() {
        GM.OnRestart(this, new RestartEventArgs());
    }

    public void OnHideSettingsButtonClick() {
        gameObject.SetActive(false);

        Debug.Log("Setting menu was hidden!");
    }
}
