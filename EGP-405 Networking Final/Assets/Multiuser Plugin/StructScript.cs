﻿using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct CharPointer
{
    public byte mId;
    public fixed char mes[512];
}

public unsafe struct StraightCharPointer //No mId so all stuffed content can be read out at once
{
    public fixed char mes[512];
}

public class StructScript {

    static MarkerFlag[,] objectMap = new MarkerFlag[100,100];

    enum Message //TODO: Add all the regular message types that we want to be ready for
    {
        ID_CONNECTION_REQUEST_ACCEPTED = 17,
        ID_CONNECTION_ATTEMPT_FAILED = 18,
        ID_NEW_INCOMING_CONNECTION = 19,
        ID_NO_FREE_INCOMING_CONNECTIONS = 20,
        ID_DISCONNECTION = 21,
        CHAT_MESSAGE = 135,
        GO_UPDATE = 136,

    }

    public static string serialize(GameObject obj)
    {
        string serialized = "";//Message.GO_UPDATE.ToString();


        serMarkerFlag markTemp = new serMarkerFlag(); //Put the marker flag info on the string first !!!
        markTemp.flag = obj.GetComponent<MarkerFlag>();
        string flagData = new string(markTemp.toChar());
        serialized += flagData;


        int hashLoc = genHashCode(markTemp.flag.id);
        int xLoc = hashLoc % 10;
        int yLoc = hashLoc % 100;
        //Debug.Log(xLoc +" "+ yLoc);

        objectMap[xLoc, yLoc] = markTemp.flag;

        serialized += obj.name + "/";
        serialized += obj.tag + "/";
        serialized += obj.layer + "/";
        serialized += obj.isStatic + "/";
       // Debug.Log("Checking");
        Component[] comps;
        comps = obj.GetComponents<Component>();
       // Debug.Log(comps.Length);
        for(int i = 0; i < comps.Length; i++)
        {
            if (comps[i].GetType() == typeof(UnityEngine.Transform))
            {
                //        Debug.Log("Has Transform");
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
                //   Debug.Log("Has Box Collider");
                UnityEngine.BoxCollider temp = comps[i] as UnityEngine.BoxCollider;
                BoxCollider serTemp = new BoxCollider();
                serTemp.center = temp.center;
                serTemp.size = temp.size;
                serTemp.isTrigger = temp.isTrigger;
                string tempString = new string(serTemp.toChar());
                serialized += tempString;
            }
            else if (comps[i].GetType() == typeof(UnityEngine.SphereCollider))
            {
                //    Debug.Log("Has Sphere Collider");
                UnityEngine.SphereCollider temp = comps[i] as UnityEngine.SphereCollider;
                SphereCollider serTemp = new SphereCollider();
                serTemp.center = temp.center;
                serTemp.radius = temp.radius;
                serTemp.isTrigger = temp.isTrigger;
                string tempString = new string(serTemp.toChar());
                serialized += tempString;
            }
            else if (comps[i].GetType() == typeof(UnityEngine.CapsuleCollider))
            {
                //  Debug.Log("Has Capsule Collider");
                UnityEngine.CapsuleCollider temp = comps[i] as UnityEngine.CapsuleCollider;
                CapsuleCollider serTemp = new CapsuleCollider();
                serTemp.center = temp.center;
                serTemp.radius = temp.radius;
                serTemp.height = temp.height;
                serTemp.directionAxis = temp.direction;
                serTemp.isTrigger = temp.isTrigger;
                string tempString = new string(serTemp.toChar());
                serialized += tempString;
            }
            else if (comps[i].GetType() == typeof(UnityEngine.Rigidbody))
            {
                //  Debug.Log("Has Rigidbody");
                UnityEngine.Rigidbody temp = comps[i] as UnityEngine.Rigidbody;
                RigidBody serTemp = new RigidBody();
                serTemp.mass = temp.mass;
                serTemp.drag = temp.drag;
                serTemp.angularDrag = temp.angularDrag;
                serTemp.interpolate = (int)temp.interpolation;
                serTemp.collisionDetection = temp.detectCollisions;
                serTemp.freeze = (int)temp.constraints;
                serTemp.isKinematic = temp.isKinematic;
                serTemp.useGravity = temp.useGravity;
                serTemp.collisionDetection = temp.detectCollisions;
                string tempString = new string(serTemp.toChar());
                serialized += tempString;
            }
            else if (comps[i].GetType() == typeof(UnityEngine.Camera))
            {
                //    Debug.Log("Has Camera");
            }
            else if (comps[i].GetType() == typeof(UnityEngine.MeshFilter))
            {
                //   Debug.Log("Has Mesh Filter");
            }
            else
            {
                //    Debug.Log(comps[i].GetType());
            }
        }
        return serialized;
    }

    public unsafe static void deserializeMessage(char* ser)
    {
        IntPtr care = (IntPtr)ser;
        CharPointer* data = (CharPointer*)care;
        string output = Marshal.PtrToStringAnsi((IntPtr)data->mes);

        switch ((Byte)ser[0])
        {
            case ((Byte)Message.ID_CONNECTION_ATTEMPT_FAILED):
                Debug.Log("Failed to connect to server");
                break;
            case ((Byte)Message.ID_NEW_INCOMING_CONNECTION):
                Debug.Log("A new client is connecting");
                MultiuserPlugin.addClient();
                
                break;
            case ((Byte)Message.ID_CONNECTION_REQUEST_ACCEPTED):
                Debug.Log("You have connected to the server");
                break;

            case ((Byte)Message.ID_NO_FREE_INCOMING_CONNECTIONS):
                Debug.Log("Connection Failed, server is FULL");
                break;
            case ((Byte)Message.GO_UPDATE):
                Debug.Log("Game Object Received");
                componentSerialize(output);
                Debug.Log(ser[0]);
                //componentSerialize(ser);
                break;
            case ((Byte)Message.ID_DISCONNECTION):
                if(MultiuserPlugin.mIsServer)
                {
                    MultiuserPlugin.deleteClient();
                    Debug.Log("Client has disconnected");
                    //TODO: remove this client from client list
                }
                else
                {
                    Debug.Log("You have disconnected");
                }
                break;
            default:
                Debug.Log(output);
                int identifier = ser[0].GetHashCode();
                Debug.Log("Message with identifier " + identifier.ToString() + " has arrived");
                break;
        }
        
       
    }

    static int genHashCode(string id)
    {
        const int primeNum = 31;
        int temp = 0;
        for (int i = 0; i < id.Length; ++i)
        {
            temp += id[i].GetHashCode();
        }
        return temp*primeNum;
    }

    public static void componentSerialize(string ser)
    {
        Debug.Log(ser);
        GameObject temp = null;

        MarkerFlag objMarker = deserializeMarkerFlag(ref ser);

        int hashLoc = genHashCode(objMarker.id);

        int xLoc = hashLoc % 10;
        int yLoc = hashLoc % 100;

        MarkerFlag thisFlag = objectMap[xLoc, yLoc];

        if(thisFlag == null) //Make a new game object with given flag if you need to
        {
            temp = new GameObject();
            thisFlag = temp.AddComponent<MarkerFlag>();
            thisFlag.id = objMarker.id;
            thisFlag.parentID = objMarker.parentID;
        }
        else
        {
            temp = thisFlag.gameObject;
        }

        temp.name = deserializeString(ref ser);
        temp.tag = deserializeString(ref ser);
        temp.layer = deserializeInt(ref ser);
        temp.isStatic = deserializeBool(ref ser);
        Debug.Log(ser);
        while (ser.Length > 0)
        {
            string tag = deserializeString(ref ser);

            if (tag == "transform")
            {
                UnityEngine.Transform trans = temp.transform;
                trans.position = deserializeVector3(ref ser);
                trans.rotation = deserializeQuaternion(ref ser);
                trans.localScale = deserializeVector3(ref ser);
            }
            else if(tag == "boxCollider")
            {
                UnityEngine.BoxCollider col = temp.AddComponent<UnityEngine.BoxCollider>();
                col.center = deserializeVector3(ref ser);
                col.size = deserializeVector3(ref ser);
                col.isTrigger = deserializeBool(ref ser);
            }
            else if(tag == "sphereCollider")
            {
                UnityEngine.SphereCollider col = temp.AddComponent<UnityEngine.SphereCollider>();
                col.center = deserializeVector3(ref ser);
                col.radius = deserializeFloat(ref ser);
                col.isTrigger = deserializeBool(ref ser);
            }
            else if (tag == "capsuleCollider")
            {
                UnityEngine.CapsuleCollider col = temp.AddComponent<UnityEngine.CapsuleCollider>();
                col.center = deserializeVector3(ref ser);
                col.radius = deserializeFloat(ref ser);
                col.height = deserializeFloat(ref ser);
                col.direction = deserializeInt(ref ser);
                col.isTrigger = deserializeBool(ref ser);
            }
            else if (tag == "rigidbody")
            {
                UnityEngine.Rigidbody col = temp.AddComponent<UnityEngine.Rigidbody>();
                col.mass = deserializeFloat(ref ser);
                col.drag = deserializeFloat(ref ser);
                col.angularDrag = deserializeFloat(ref ser);
                col.interpolation = (RigidbodyInterpolation)deserializeInt(ref ser);
                col.constraints = (RigidbodyConstraints)deserializeInt(ref ser);
                col.useGravity = deserializeBool(ref ser);
                col.isKinematic = deserializeBool(ref ser);
                col.detectCollisions = deserializeBool(ref ser);
            }

        }
        addToMap(thisFlag);
    }

    public static void addToMap(MarkerFlag flag)
    {
        Debug.Log("Adding to map");
        int hashCode = genHashCode(flag.id); //TODO Need to do an overwrite check
        int xLoc = hashCode % 10;
        int yLoc = hashCode % 100;
        objectMap[xLoc,yLoc] = flag;
    }

    public static MarkerFlag deserializeMarkerFlag(ref string ser)
    {
        MarkerFlag temp = new MarkerFlag();
        //string trash = deserializeString(ref ser);
        temp.id = deserializeString(ref ser);
        return temp;
    }

    public static int deserializeInt(ref string ser)
    {
        int length = ser.IndexOf("/");
        int ret = int.Parse(ser.Substring(0, length));
        ser = ser.Remove(0, length + 1);
        return ret;
    }

    public static string deserializeString(ref string ser)
    {
        int length = ser.IndexOf("/");
        string ret = ser.Substring(0, length);
        ser = ser.Remove(0, length + 1);
        return ret;
    }

    public static float deserializeFloat(ref string ser)
    {
        int length = ser.IndexOf("/");
        float ret = float.Parse(ser.Substring(0, length));
        ser = ser.Remove(0, length + 1);
        return ret;
    }

    public static bool deserializeBool(ref string ser)
    {
        int length = ser.IndexOf("/");
        bool ret = bool.Parse(ser.Substring(0, length));
        ser = ser.Remove(0, length + 1);
        return ret;
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
    /*enum typeID
    {
        TRANSFORM,
        BOXCOLLIDER,
        SPHERECOLLIDER,
        CAPSULECOLLIDER,
        RIGIDBODY,
        CAMERA,
        MESHFILTER
    }
    typeID id;*/

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

public class serMarkerFlag : serializedComponent
{
    public MarkerFlag flag;

    override public char[] toChar()
    {
        string temp = "markerFlag/";
        temp += flag.id + "/";
        return temp.ToCharArray();
    }
}

public class Transform : serializedComponent
{
    public Vector3 pos;
    public Quaternion rot;
    public Vector3 scale;
    override public char[] toChar()
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
    public Vector3 center;
    public Vector3 size;
    public bool isTrigger;
    override public char[] toChar()
    {
        string temp = "boxCollider/";
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

public class SphereCollider : serializedComponent
{
    public Vector3 center;
    public float radius;
    public bool isTrigger;
    override public char[] toChar()
    {
        string temp = "sphereCollider/";
        temp += center.x + "/";
        temp += center.y + "/";
        temp += center.z + "/";
        temp += radius + "/";
        temp += isTrigger + "/";
        return temp.ToCharArray();
    }
}

public class CapsuleCollider : serializedComponent
{
    public Vector3 center;
    public float radius, height;
    public int directionAxis;
    public bool isTrigger;
    override public char[] toChar()
    {
        string temp = "capsuleCollider/";
        temp += center.x + "/";
        temp += center.y + "/";
        temp += center.z + "/";
        temp += radius + "/";
        temp += height + "/";
        temp += directionAxis + "/";
        temp += isTrigger + "/";
        return temp.ToCharArray();
    }
}

public class RigidBody : serializedComponent
{
    public float mass, drag, angularDrag;
    public int interpolate, freeze;
    public bool useGravity, isKinematic, collisionDetection;
    override public char[] toChar()
    {
        string temp = "rigidbody/";
        temp += mass + "/";
        temp += drag + "/";
        temp += angularDrag + "/";
        temp += interpolate + "/";
        temp += freeze + "/";
        temp += useGravity + "/";
        temp += isKinematic + "/";
        temp += collisionDetection + "/";
        return temp.ToCharArray();
    }
}

/*public class Camera : serializedComponent
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

