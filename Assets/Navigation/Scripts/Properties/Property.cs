using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Property : MonoBehaviour {
    public bool IsEnable { get { return enabled; } set { enabled = value; } }

    public virtual void Init()
    {
        IsEnable = false;
    }
}
