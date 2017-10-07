using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructScript {

    public char[] serialize(GameObject obj)
    {
        Debug.Log("Checking");
        Component[] comps;
        comps = obj.GetComponents<Component>();
        Debug.Log(comps.Length);
        for(int i = 0; i < comps.Length; i++)
        {
            if(comps[i].GetType() == typeof(UnityEngine.Transform))
            {
                Debug.Log("Has Transform");
            }
            else if (comps[i].GetType() == typeof(UnityEngine.Rigidbody))
            {
                Debug.Log("Has Rigidbody");
            }
        }
        return null;
    }

}

public class serializedComponent
{

    public serializedComponent() { }
    enum typeID
    {
        TRANSFORM,
        BOXCOLLIDER,
        SPHERECOLLIDER,
        CAPSULECOLLIDER,
        RIGIDBODY,
        CAMERA,
        MESHFILTER
    }
    typeID id;

}


//These structs are in unity engine by default, keeping them just in case.
/*struct Vector3
{
    float x, y, z;
}

struct Quaternion
{
    float rotX, rotY, rotZ, rotW;
}

struct Color
{
    float r, g, b, a;
}*/

public class Transform : serializedComponent
{
    Vector3 pos;
    Quaternion rot;
    Vector3 scale;
}

public class BoxCollider : serializedComponent
{
    Vector3 center;
    Vector3 size;
    bool isTrigger;
}

public class SphereCollider : serializedComponent
{
    Vector3 center;
    float radius;
    bool isTrigger;
}

public class CapsuleCollider : serializedComponent
{
    Vector3 center;
    float radius, height;
    enum directionAxis
    {
        X_AXIS,
        Y_AXIS,
        Z_AXIS
    }
    bool isTrigger;
}

public class RigidBody : serializedComponent
{
    float mass, drag, angularDrag;
    bool useGravity, isKinematic;
    enum interpolate
    {
        NONE,
        INTERPOLATE,
        EXTRAPOLATE
    }
    enum collisionDetection
    {
        DISCRETE,
        CONTINUOUS,
        CONTINUOUS_DYNAMIC
    }
    Vector3 freezepos, freezeRot;
}

public class Camera : serializedComponent
{
    enum clearFlags
    {
        SKYBOX,
        SOLID_COLOR,
        DEPTH_ONLY,
        DONT_CLEAR
    }
    Color background;
    bool[] cullingMask = new bool[5];
    enum projection
    {
        PERSPECTIVE,
        ORTHOGRAPHIC
    }
    float[] clippingPlanes = new float[2];
    Vector4 viewportRect;
    enum renderingPath
    {
        USE_GRAPHICS_SETTINGS,
        FORWARD,
        DEFERRED,
        LEGACY_VERTEX_LIT,
        LEGACY_DEFERRED
    }

    bool HDR, MSAA, occlusionCulling;

    float depth, fov;

     enum targetDisplay
    {
        DISPLAY_1,
        DISPLAY_2,
        DISPLAY_3,
        DISPLAY_4,
        DISPLAY_5,
        DISPLAY_6,
        DISPLAY_7,
        DISPLAY_8
    }
}

public class MeshFilter : serializedComponent
{
    string fileName;
}


