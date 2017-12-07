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

    private void OnDrawGizmos()
    {
        if (isLocked)
        {
            Renderer[] childRenderer = GetComponentsInChildren<Renderer>();
            Bounds tempBounds = getBounds(this.gameObject);

            Gizmos.color = new Color(1, 1, 0, 0.75f);
            Gizmos.DrawWireCube(tempBounds.center, tempBounds.size);
        }
        
    }
    Bounds getBounds(GameObject objeto)
    {
        Bounds bounds;
        bounds = getRenderBounds(objeto);
        if (bounds.extents.x == 0)
        {
            bounds = new Bounds(objeto.transform.position, Vector3.zero);
            Renderer[] childRenderers = GetComponentsInChildren<Renderer>();
            foreach (Renderer child in childRenderers)
            {
                if (child)
                {
                    bounds.Encapsulate(child.bounds);
                }
                else
                {
                    bounds.Encapsulate(getBounds(child.gameObject));
                }
            }
        }
        return bounds;
    }
    Bounds getRenderBounds(GameObject objeto)
    {
        Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
        Renderer render = objeto.GetComponent<Renderer>();
        if (render != null)
        {
            return render.bounds;
        }
        return bounds;
    }
    void OnEnable()
    {
        hideFlags = HideFlags.HideInInspector;
       
    }
    private void OnDestroy()
    {
        MultiuserPlugin.DeleteObject(this);
    }
}
