using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StickerObjectTransformEventArgs : EventArgs {
    public Transform stickerTransform;
    public GameObject outputTextObject;
}

public class Sticker : MonoBehaviour {
    public static EventHandler<StickerObjectTransformEventArgs> OnStickerObjectTransform;
    public GameObject outputText;

    public void OnEditButtonClick() {
        OnStickerObjectTransform(this, new StickerObjectTransformEventArgs() { stickerTransform = gameObject.transform, outputTextObject = outputText });
    }
}
