using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour {
    public void OnHideSettingsClick()
    {
        gameObject.SetActive(false);

        Debug.Log("Setting menu was hidden!");
    }
}
