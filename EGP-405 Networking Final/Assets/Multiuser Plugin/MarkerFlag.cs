using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class MarkerFlag : MonoBehaviour {
     public string id;
     public bool isModified = false; //Marks if the object needs to be sent over the network
     public bool isHeld = false; //Marks if YOU'RE holding the object
     public bool isLocked = false; //Marks if an object is held by another client
     public string parentID;

    public Vector3 ogPos, ogScale;
    public Quaternion ogRot;
    void OnEnable()
    {
      //  this.hideFlags = HideFlags.HideInInspector;
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
