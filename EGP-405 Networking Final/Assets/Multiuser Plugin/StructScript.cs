using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StructScript {

    enum Message //TODO: Add all the regular message types that we want to be ready for
    {
        ID_CONNECTION_REQUEST_ACCEPTED = 16,
        ID_CONNECTION_ATTEMPT_FAILED = 17,
        ID_NEW_INCOMING_CONNECTION = 1043,
        ID_NO_FREE_INCOMING_CONNECTIONS = 20,
        CHAT_MESSAGE = 135,
        GO_UPDATE = 136,
    }

    public string serialize(GameObject obj)
    {
        string serialized = "g";
        Debug.Log("Checking");
        Component[] comps;
        comps = obj.GetComponents<Component>();
        Debug.Log(comps.Length);
        for(int i = 0; i < comps.Length; i++)
        {
            if (comps[i].GetType() == typeof(UnityEngine.Transform))
            {
                Debug.Log("Has Transform");
                UnityEngine.Transform temp = comps[i] as UnityEngine.Transform;
                Transform serTemp = new Transform();
                serTemp.pos = temp.position;
                serTemp.rot = temp.rotation;
                serTemp.scale = temp.localScale; //Look here for scaling issues
                string tempString = new string(serTemp.toChar());
                serialized += tempString;
                
            }
            else if (comps[i].GetType() == typeof(UnityEngine.BoxCollider))
            {
                Debug.Log("Has Box Collider");
            }
            else if (comps[i].GetType() == typeof(UnityEngine.SphereCollider))
            {
                Debug.Log("Has Shpere Collider");
            }
            else if (comps[i].GetType() == typeof(UnityEngine.CapsuleCollider))
            {
                Debug.Log("Has Capsule Collider");
            }
            else if (comps[i].GetType() == typeof(UnityEngine.Rigidbody))
            {
                Debug.Log("Has Rigidbody");
            }
            else if (comps[i].GetType() == typeof(UnityEngine.Camera))
            {
                Debug.Log("Has Camera");
            }
            else if (comps[i].GetType() == typeof(UnityEngine.MeshFilter))
            {
                Debug.Log("Has Mesh Filter");
            }
            else
                Debug.Log(comps[i].GetType());
        }
        return serialized;
    }

    public static unsafe void deserialize(char* ser)
    {
        Debug.Log((Message)ser[0]);
        switch ((Message)ser[0])
        {
            case Message.CHAT_MESSAGE:
                Debug.Log("New Message Recieved");
                //TODO: put message on the message stack for chat system
                break;
            case Message.ID_CONNECTION_ATTEMPT_FAILED:
                Debug.Log("Failed to connect to server");
                break;
            case Message.ID_NEW_INCOMING_CONNECTION:
                Debug.Log("A new client is connecting");
                break;
            case Message.ID_CONNECTION_REQUEST_ACCEPTED:
                Debug.Log("You have connected to the server");
                break;
            case Message.ID_NO_FREE_INCOMING_CONNECTIONS:
                Debug.Log("Connection Failed, server is FULL");
                break;
            case Message.GO_UPDATE:
                Debug.Log("New Gameobject update recieved");
                break;
            default:
                Debug.Log("Message with identifier " + ser[0] + " has arrived");
                break;
        }
        
       
    }

    public static Vector3 deserializeVector3(ref string ser)
    {
        Vector3 vec;
        int length = ser.IndexOf("/");
        vec.x = float.Parse(ser.Substring(0, length));
        ser = ser.Remove(0, length+1);
        length = ser.IndexOf("/");
        vec.y = float.Parse(ser.Substring(0, length));
        ser = ser.Remove(0, length+1);
        length = ser.IndexOf("/");
        vec.z = float.Parse(ser.Substring(0, length));
        ser = ser.Remove(0, length+1);
        return vec;
    }

    public static Quaternion deserializeQuaternion(ref string ser)
    {
        Quaternion vec;
        int length = ser.IndexOf("/");
        vec.x = float.Parse(ser.Substring(0, length));
        ser = ser.Remove(0, length+1);
        length = ser.IndexOf("/");
        vec.y = float.Parse(ser.Substring(0, length));
        ser = ser.Remove(0, length+1);
        length = ser.IndexOf("/");
        vec.z = float.Parse(ser.Substring(0, length));
        ser = ser.Remove(0, length+1);
        length = ser.IndexOf("/");
        vec.w = float.Parse(ser.Substring(0, length));
        ser = ser.Remove(0, length+1);
        return vec;
    }

    public string vecToString(Vector3 vec)
    {
        string temp = "";
        temp += vec.x;
        temp += vec.y;
        temp += vec.z;
        return temp;
    }

    public string quatToString(Quaternion qua)
    {
        string temp = "";
        temp += qua.x;
        temp += qua.y;
        temp += qua.z;
        temp += qua.w;
        return temp;
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

    public virtual char[] toChar() { return null; }

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
    public Vector3 pos;
    public Quaternion rot;
    public Vector3 scale;
    public virtual char[] toChar()
    {
        string temp = "transform/";
        temp += pos.x + "/";
        temp += pos.y + "/";
        temp += pos.z + "/";
        temp += rot.x + "/";
        temp += rot.y + "/";
        temp += rot.z + "/";
        temp += rot.w + "/";
        temp += scale.x + "/";
        temp += scale.y + "/";
        temp += scale.z + "/";
        return temp.ToCharArray();
    }
}

public class BoxCollider : serializedComponent
{
    Vector3 center;
    Vector3 size;
    bool isTrigger;
    public virtual char[] toChar()
    {
        string temp = "transform/";
        temp += center.x + "/";
        temp += center.y + "/";
        temp += center.z + "/";
        temp += size.x + "/";
        temp += size.y + "/";
        temp += size.z + "/";
        temp += isTrigger + "/";
        return temp.ToCharArray();
    }
}

/*public class SphereCollider : serializedComponent
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
}*/

