using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class MarkerFlag : MonoBehaviour {
     public string id;
     public bool isModified = true;
     public bool isLocked = true;
     public string parentID;
    void OnEnable()
    {
        //this.hideFlags = HideFlags.HideInInspector;
    }
}
