using System;
using UnityEngine;

public class Settings : MonoBehaviour {
    public static EventHandler<EventArgs> OnClickHideSettings;

    public void OnRestartButtonClick() {
        GM.OnRestart(this, new RestartEventArgs());
    }

    public void OnHideSettingsButtonClick() {
        OnClickHideSettings(this, EventArgs.Empty);
    }
}
