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

    public void OnDeleteButtonClick() {
        OnStickerObjectDelete(this, new StickerObjectTransformEventArgs() { stickerObject = gameObject, stickerTransform = gameObject.transform, outputTextObject = outputText });
    }

    public void OnEditButtonClick() {
        OnStickerObjectTransform(this, new StickerObjectTransformEventArgs() { stickerObject = gameObject, stickerTransform = gameObject.transform, outputTextObject = outputText });
    }
}
