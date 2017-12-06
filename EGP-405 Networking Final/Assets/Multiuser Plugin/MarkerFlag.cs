using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class MarkerFlag : MonoBehaviour {
     public string id;
     public bool isModified = false;
     public bool isLocked = false;
     public string parentID;

    public Vector3 ogPos, ogScale;
    public Quaternion ogRot;
    void OnEnable()
    {
        //this.hideFlags = HideFlags.HideInInspector;
    }
    private void OnDrawGizmos()
    {
        if (isLocked)
        {
            Vector3 size = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
            Gizmos.color = new Color(1, 1, 0, 0.75f);
            Gizmos.DrawWireCube(transform.position, size);
        }
    }

    private void OnDestroy()
    {
        MultiuserPlugin.DeleteObject(this);
    }
}
