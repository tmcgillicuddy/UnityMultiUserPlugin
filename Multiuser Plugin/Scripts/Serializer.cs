/*
EGP 405-01 Final Project 12/7/17
Aaron Hamilton
James Smith
Thomas McGillicuddy
“We certify that this work is
entirely our own. The assessor of this project may reproduce this project
and provide copies to other academic staff, and/or communicate a copy of
this project to a plagiarism-checking service, which may retain a copy of the
project on its database.”
*/

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
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

public class Serializer
{
    static int expectedObjs, recievedObjs;

    static List<MarkerFlag>[,] objectMap = new List<MarkerFlag>[100, 100];

    public enum Message
    {
        ID_CONNECTION_REQUEST_ACCEPTED = 16,
        ID_CONNECTION_ATTEMPT_FAILED = 17,
        ID_NEW_INCOMING_CONNECTION = 19,
        ID_NO_FREE_INCOMING_CONNECTIONS = 20,
        ID_DISCONNECTION = 21,
        ID_CONNECTION_LOST = 22,
        CHAT_MESSAGE = 135,
        GO_UPDATE = 136,
        LOADLEVEL = 137,
        LEVELLOADED = 138,
        GO_DELETE = 139,
    }

    public static void init()
    {
        for (int i = 0; i < 100; ++i)
        {
            for (int q = 0; q < 100; ++q)
            {
                objectMap[i, q] = new List<MarkerFlag>();
            }
        }
    }

    public unsafe static void deserializeMessage(char* ser)
    {
        IntPtr care = (IntPtr)ser;
        CharPointer* data = (CharPointer*)care;
        string output = Marshal.PtrToStringAnsi((IntPtr)data->mes);
        switch ((Byte)ser[0])
        {
            case (Byte)Message.ID_CONNECTION_ATTEMPT_FAILED:
                Debug.Log("Failed to connect to server");
                break;
            case (Byte)Message.ID_NEW_INCOMING_CONNECTION:
                MultiuserPlugin.handleChatMessage("A new client is connecting");
                MultiuserPlugin.addClient();

                break;
            case (Byte)Message.ID_CONNECTION_REQUEST_ACCEPTED:
                MultiuserEditor.messageStack.Add("You have connected to the server");
                break;

            case (Byte)Message.ID_NO_FREE_INCOMING_CONNECTIONS:
                Debug.Log("Connection Failed, server is FULL");
                break;
            case (Byte)Message.ID_CONNECTION_LOST:
                MultiuserPlugin.handleChatMessage("Someone lost connection");
                break;
            case (Byte)Message.ID_DISCONNECTION:
                if (MultiuserPlugin.mIsServer)
                {
                    MultiuserPlugin.handleChatMessage("Client has disconnected");
                }
                else
                {
                    MultiuserEditor.messageStack.Add("You have disconnected");
                }
                break;
            case (Byte)Message.LOADLEVEL:
                expectedObjs = deserializeInt(ref output);
                recievedObjs = 0;
                EditorUtility.DisplayProgressBar("Getting Level Data", "", 0);
                break;
            case (Byte)Message.LEVELLOADED:
                ReparentObjects();
                ReparentObjects();
                expectedObjs = -1;
                EditorUtility.ClearProgressBar();

                break;
            case (Byte)Message.GO_DELETE:
                if (MultiuserPlugin.mIsServer)
                {
                    MultiuserPlugin.Echo(Message.GO_DELETE, output);
                }
                deleteGO(output);
                break;
            case (Byte)Message.CHAT_MESSAGE:
                MultiuserPlugin.handleChatMessage(output);
                break;
            case (Byte)Message.GO_UPDATE:
                if (expectedObjs > 0)
                {
                    recievedObjs++;
                    EditorUtility.DisplayProgressBar("Getting Level Data", "Recieved " + recievedObjs, (float)recievedObjs / expectedObjs);
                }
                if (MultiuserPlugin.mIsServer) //If this instance is a server
                {
                    MultiuserPlugin.Echo(Message.GO_UPDATE, output);
                }
                componentSerialize(output);
                //componentSerialize(ser);
                break;
            default:
                int identifier = (Byte)ser[0].GetHashCode();
                Debug.Log("Message with identifier " + identifier.ToString() + " has arrived");
                break;
        }


    }

    public static string serialize(GameObject obj)
    {
        string serialized = "";//Message.GO_UPDATE.ToString();

        serMarkerFlag serMarker = new serMarkerFlag(); //Put the marker flag info on the string first !!!
        serMarker.flag = obj.GetComponent<MarkerFlag>();

        if (obj.transform.parent == null)
        {
            serMarker.flag.parentID = "__";
        }
        else
        {
            serMarker.flag.parentID = obj.transform.parent.GetComponent<MarkerFlag>().id;
        }

        string flagData = new string(serMarker.toChar());
        serialized += flagData;

        int hashLoc = genHashCode(serMarker.flag.id);
        int xLoc = hashLoc % 10;
        int yLoc = hashLoc % 100;

        //TODO check location if it already is there
        objectMap[xLoc, yLoc].Add(serMarker.flag);

        serialized += obj.name + "|";
        serialized += obj.tag + "|";
        serialized += obj.layer + "|";
        serialized += obj.isStatic + "|";
        Component[] comps;
        comps = obj.GetComponents<Component>();
        for (int i = 0; i < comps.Length; i++)
        {
            if (comps[i] != null)
            {
                if (comps[i].GetType() == typeof(UnityEngine.Transform))
                {
                    UnityEngine.Transform newTransform = comps[i] as UnityEngine.Transform;

                    Transform serNewTransform = new Transform();
                    serNewTransform.pos = newTransform.position;
                    serNewTransform.rot = newTransform.rotation;
                    serNewTransform.scale = newTransform.localScale;
                    string transformString = new string(serNewTransform.toChar());
                    serialized += transformString;

                }
                else if (comps[i].GetType() == typeof(UnityEngine.BoxCollider))
                {
                    UnityEngine.BoxCollider newBoxCollider = comps[i] as UnityEngine.BoxCollider;

                    BoxCollider serNewBoxCollider = new BoxCollider();
                    serNewBoxCollider.center = newBoxCollider.center;
                    serNewBoxCollider.size = newBoxCollider.size;
                    serNewBoxCollider.isTrigger = newBoxCollider.isTrigger;
                    string boxColliderString = new string(serNewBoxCollider.toChar());
                    serialized += boxColliderString;
                }
                else if (comps[i].GetType() == typeof(UnityEngine.SphereCollider))
                {
                    UnityEngine.SphereCollider newSphereCollider = comps[i] as UnityEngine.SphereCollider;

                    SphereCollider serNewSphereCollider = new SphereCollider();
                    serNewSphereCollider.center = newSphereCollider.center;
                    serNewSphereCollider.radius = newSphereCollider.radius;
                    serNewSphereCollider.isTrigger = newSphereCollider.isTrigger;
                    string sphereColliderString = new string(serNewSphereCollider.toChar());
                    serialized += sphereColliderString;
                }
                else if (comps[i].GetType() == typeof(UnityEngine.CapsuleCollider))
                {
                    UnityEngine.CapsuleCollider newCapsuleCollider = comps[i] as UnityEngine.CapsuleCollider;

                    CapsuleCollider serNewCapsuleCollider = new CapsuleCollider();
                    serNewCapsuleCollider.center = newCapsuleCollider.center;
                    serNewCapsuleCollider.radius = newCapsuleCollider.radius;
                    serNewCapsuleCollider.height = newCapsuleCollider.height;
                    serNewCapsuleCollider.directionAxis = newCapsuleCollider.direction;
                    serNewCapsuleCollider.isTrigger = newCapsuleCollider.isTrigger;
                    string capsuleColliderString = new string(serNewCapsuleCollider.toChar());
                    serialized += capsuleColliderString;
                }
                else if (comps[i].GetType() == typeof(UnityEngine.Rigidbody))
                {
                    UnityEngine.Rigidbody newRigidBody = comps[i] as UnityEngine.Rigidbody;

                    RigidBody serNewRigidBody = new RigidBody();
                    serNewRigidBody.mass = newRigidBody.mass;
                    serNewRigidBody.drag = newRigidBody.drag;
                    serNewRigidBody.angularDrag = newRigidBody.angularDrag;
                    serNewRigidBody.interpolate = (int)newRigidBody.interpolation;
                    serNewRigidBody.collisionDetection = newRigidBody.detectCollisions;
                    serNewRigidBody.freeze = (int)newRigidBody.constraints;
                    serNewRigidBody.isKinematic = newRigidBody.isKinematic;
                    serNewRigidBody.useGravity = newRigidBody.useGravity;
                    serNewRigidBody.collisionDetection = newRigidBody.detectCollisions;
                    string rigidBodyString = new string(serNewRigidBody.toChar());
                    serialized += rigidBodyString;
                }
                else if (comps[i].GetType() == typeof(UnityEngine.Camera))
                {
                    UnityEngine.Camera newCamera = comps[i] as UnityEngine.Camera;

                    Camera serNewCamera = new Camera();
                    serNewCamera.clearFlags = (int)newCamera.clearFlags;
                    serNewCamera.background = newCamera.backgroundColor;
                    serNewCamera.cullingMask = newCamera.cullingMask;
                    serNewCamera.projection = newCamera.projectionMatrix.ToString();
                    serNewCamera.near = newCamera.nearClipPlane;
                    serNewCamera.far = newCamera.farClipPlane;
                    serNewCamera.viewportRect = newCamera.rect;
                    serNewCamera.renderingPath = (int)newCamera.renderingPath;
                    serNewCamera.HDR = newCamera.allowHDR;
                    serNewCamera.MSAA = newCamera.allowMSAA;
                    serNewCamera.occlusionCulling = newCamera.useOcclusionCulling;
                    serNewCamera.depth = newCamera.depth;
                    serNewCamera.fov = newCamera.fieldOfView;
                    serNewCamera.targetDisplay = newCamera.targetDisplay;

                    string cameraString = new string(serNewCamera.toChar());
                    serialized += cameraString;
                }
                else if (comps[i].GetType() == typeof(UnityEngine.MeshFilter))
                {
                    //Gather Meshfilter information on current GO
                    UnityEngine.MeshFilter gOMeshFilter = comps[i] as UnityEngine.MeshFilter;
                    Mesh gOMesh = gOMeshFilter.sharedMesh;


                    //Pack data into our meshfilter object
                    MeshFilter meshStruct = new MeshFilter();
                    meshStruct.filePath = AssetDatabase.GetAssetPath(gOMesh);
                    meshStruct.meshName = gOMesh.name;

                    //Convert the data into a string and add it to the overall data stream
                    string sStream = new string(meshStruct.toChar());
                    serialized += sStream;

                }
                else if (comps[i].GetType() == typeof(UnityEngine.MeshRenderer))
                {
                    UnityEngine.MeshRenderer gOMeshRenderer = comps[i] as UnityEngine.MeshRenderer;


                    //Pack data into our MeshRenderer obj
                    MeshRenderer meshStruct = new MeshRenderer();
                    meshStruct.lightProbe = (int)gOMeshRenderer.lightProbeUsage;
                    meshStruct.reflectionProbe = (int)gOMeshRenderer.reflectionProbeUsage;
                    meshStruct.castShadows = (int)gOMeshRenderer.shadowCastingMode;
                    meshStruct.receiveShadows = gOMeshRenderer.receiveShadows;
                    meshStruct.motionVectors = (int)gOMeshRenderer.motionVectorGenerationMode;
                    meshStruct.lightmapStatic = false;

                    Material[] gOMaterials = gOMeshRenderer.sharedMaterials;
                    for (int q = 0; q < gOMaterials.Length; ++q)
                    {
                        string materialPath = "";
                        if (gOMaterials[q] == null || gOMaterials[q].name == "Default-Material")
                        {
                            materialPath = "Default-Material";
                        }
                        else
                        {
                            materialPath = AssetDatabase.GetAssetPath(gOMaterials[q]);
                        }
                        meshStruct.materialFiles.Add(materialPath);
                    }

                    string sStream = new string(meshStruct.toChar());
                    serialized += sStream;
                }
                else if (comps[i].GetType() == typeof(UnityEngine.Light))
                {
                    UnityEngine.Light newLight = comps[i] as UnityEngine.Light;

                    Light serNewLight = new Light();
                    serNewLight.type = (int)newLight.type;
                    serNewLight.shadows = (int)newLight.shadows;
                    serNewLight.mode = (int)newLight.renderMode;
                    serNewLight.cullingMask = newLight.cullingMask;
                    serNewLight.color = newLight.color;
                    serNewLight.intensity = newLight.intensity;
                    serNewLight.cookie = newLight.cookieSize;

                    string lightString = new string(serNewLight.toChar());
                    serialized += lightString;
                }
                else if (comps[i].GetType() == typeof(UnityEngine.GUILayer))
                {
                    UnityEngine.GUILayer newGuiLayer = comps[i] as UnityEngine.GUILayer;

                    string lightString = "guilayer|";
                    serialized += lightString;
                }
                else if (comps[i].GetType() == typeof(UnityEngine.FlareLayer))
                {
                    UnityEngine.FlareLayer newGuiLayer = comps[i] as UnityEngine.FlareLayer;

                    string lightString = "flarelayer|";
                    serialized += lightString;
                }
                else if (comps[i].GetType() == typeof(UnityEngine.AudioListener))
                {
                    UnityEngine.AudioListener newGuiLayer = comps[i] as UnityEngine.AudioListener;

                    string lightString = "audiolistener|";
                    serialized += lightString;
                }

            }
        }

        return serialized;
    }

    public static void componentSerialize(string ser)
    {
        GameObject gameObject = null;

        MarkerFlag objMarker = deserializeMarkerFlag(ref ser);

        int hashLoc = genHashCode(objMarker.id);

        int xLoc = hashLoc % 10;
        int yLoc = hashLoc % 100;

        MarkerFlag thisFlag = findInList(objMarker.id,xLoc,yLoc);

        if (thisFlag == null) //Make a new game object with given flag if you need to
        {
            gameObject = new GameObject();
            thisFlag = gameObject.AddComponent<MarkerFlag>();
        }
        else
        {
            gameObject = thisFlag.gameObject;
        }

        thisFlag.id = objMarker.id;
        thisFlag.parentID = objMarker.parentID;
        thisFlag.isLocked = objMarker.isLocked;
        if (thisFlag.parentID != "_")
        {
            int parentHash = genHashCode(thisFlag.parentID);
            int xParent = parentHash % 10;
            int yParent = parentHash % 100;
            MarkerFlag parentFlag = findInList(thisFlag.parentID, xParent, yParent);
            if (parentFlag != null)
            {
                gameObject.transform.SetParent(parentFlag.gameObject.transform);
            }
            else
            {
                gameObject.transform.SetParent(null);
            }
        }
        else
        {
            gameObject.transform.SetParent(null);
        }

        gameObject.name = deserializeString(ref ser);
        gameObject.tag = deserializeString(ref ser);
        gameObject.layer = deserializeInt(ref ser);
        gameObject.isStatic = deserializeBool(ref ser);
        while (ser.Length > 0)
        {
            string tag = deserializeString(ref ser); //Identifies the component type

            if (tag == "transform")
            {
                UnityEngine.Transform trans = gameObject.transform;
                trans.position = deserializeVector3(ref ser);
                trans.rotation = deserializeQuaternion(ref ser);
                trans.localScale = deserializeVector3(ref ser);

                if (expectedObjs > -1)
                {
                    thisFlag.ogPos = trans.position;
                    thisFlag.ogRot = trans.rotation;
                    thisFlag.ogScale = trans.localScale;
                }
            }
            else if (tag == "boxCollider")
            {
                UnityEngine.BoxCollider col = gameObject.GetComponent<UnityEngine.BoxCollider>();
                if (col == null)
                {
                    col = gameObject.AddComponent<UnityEngine.BoxCollider>();
                }
                col.center = deserializeVector3(ref ser);
                col.size = deserializeVector3(ref ser);
                col.isTrigger = deserializeBool(ref ser);
            }
            else if (tag == "sphereCollider")
            {
                UnityEngine.SphereCollider col = gameObject.GetComponent<UnityEngine.SphereCollider>();
                if (col == null)
                {
                    col = gameObject.AddComponent<UnityEngine.SphereCollider>();
                }
                col.center = deserializeVector3(ref ser);
                col.radius = deserializeFloat(ref ser);
                col.isTrigger = deserializeBool(ref ser);
            }
            else if (tag == "capsuleCollider")
            {
                UnityEngine.CapsuleCollider col = gameObject.GetComponent<UnityEngine.CapsuleCollider>();
                if (col == null)
                {
                    col = gameObject.AddComponent<UnityEngine.CapsuleCollider>();
                }
                col.center = deserializeVector3(ref ser);
                col.radius = deserializeFloat(ref ser);
                col.height = deserializeFloat(ref ser);
                col.direction = deserializeInt(ref ser);
                col.isTrigger = deserializeBool(ref ser);
            }
            else if (tag == "rigidbody")
            {
                UnityEngine.Rigidbody col = gameObject.GetComponent<UnityEngine.Rigidbody>();
                if (col == null)
                {
                    col = gameObject.AddComponent<UnityEngine.Rigidbody>();
                }
                col.mass = deserializeFloat(ref ser);
                col.drag = deserializeFloat(ref ser);
                col.angularDrag = deserializeFloat(ref ser);
                col.interpolation = (RigidbodyInterpolation)deserializeInt(ref ser);
                col.constraints = (RigidbodyConstraints)deserializeInt(ref ser);
                col.useGravity = deserializeBool(ref ser);
                col.isKinematic = deserializeBool(ref ser);
                col.detectCollisions = deserializeBool(ref ser);
            }
            else if (tag == "camera")
            {
                UnityEngine.Camera cam = gameObject.GetComponent<UnityEngine.Camera>();
                if (cam == null)
                {
                    cam = gameObject.AddComponent<UnityEngine.Camera>();
                }
                cam.clearFlags = (CameraClearFlags)deserializeInt(ref ser);
                cam.backgroundColor = deserializeColor(ref ser);
                cam.cullingMask = deserializeInt(ref ser);
                cam.nearClipPlane = deserializeFloat(ref ser);
                cam.farClipPlane = deserializeFloat(ref ser);
                cam.rect = deserializeRect(ref ser);
                cam.renderingPath = (RenderingPath)deserializeInt(ref ser);
                cam.allowHDR = deserializeBool(ref ser);
                cam.allowMSAA = deserializeBool(ref ser);
                cam.useOcclusionCulling = deserializeBool(ref ser);
                cam.depth = deserializeFloat(ref ser);
                cam.fieldOfView = deserializeFloat(ref ser);
                cam.targetDisplay = deserializeInt(ref ser);
            }
            else if (tag == "light")
            {
                UnityEngine.Light li = gameObject.GetComponent<UnityEngine.Light>();
                if (li == null)
                {
                    li = gameObject.AddComponent<UnityEngine.Light>();
                }
                li.type = (LightType)deserializeInt(ref ser);
                li.shadows = (LightShadows)deserializeInt(ref ser);
                li.renderMode = (LightRenderMode)deserializeInt(ref ser);
                li.cullingMask = deserializeInt(ref ser);
                li.color = deserializeColor(ref ser);
                li.intensity = deserializeFloat(ref ser);
                li.cookieSize = deserializeFloat(ref ser);
            }
            else if (tag == "meshfilter")
            {

                UnityEngine.MeshFilter meshFilter = gameObject.GetComponent<UnityEngine.MeshFilter>();
                if (meshFilter == null)
                {
                    meshFilter = gameObject.AddComponent<UnityEngine.MeshFilter>();
                }
                string filePath = deserializeString(ref ser);
                string meshName = deserializeString(ref ser);

                UnityEngine.Object[] assets = AssetDatabase.LoadAllAssetsAtPath(filePath);
                for (int x = 0; x < assets.Length; ++x)
                {
                    if (assets[x].name == meshName)
                    {
                        gameObject.GetComponent<UnityEngine.MeshFilter>().mesh = assets[x] as UnityEngine.Mesh;
                        break;
                    }
                }

            }
            else if (tag == "meshRenderer")
            {
                UnityEngine.MeshRenderer gOMeshRenderer = gameObject.GetComponent<UnityEngine.MeshRenderer>();
                if (gOMeshRenderer == null)
                {
                    gOMeshRenderer = gameObject.AddComponent<UnityEngine.MeshRenderer>();
                }

                gOMeshRenderer.lightProbeUsage = (UnityEngine.Rendering.LightProbeUsage)deserializeInt(ref ser);
                gOMeshRenderer.reflectionProbeUsage = (UnityEngine.Rendering.ReflectionProbeUsage)deserializeInt(ref ser);
                gOMeshRenderer.shadowCastingMode = (UnityEngine.Rendering.ShadowCastingMode)deserializeInt(ref ser);
                gOMeshRenderer.receiveShadows = deserializeBool(ref ser);
                gOMeshRenderer.motionVectorGenerationMode = (UnityEngine.MotionVectorGenerationMode)deserializeInt(ref ser);
                //Light map static junk
                deserializeBool(ref ser);

                string materialsList = deserializeString(ref ser);
                List<Material> renderMaterials = new List<Material>();
                if (materialsList.Length > 1)
                {
                    while (materialsList != "")
                    {
                        int length = materialsList.IndexOf(",");
                        if (length > 0)
                        {
                            string ret = materialsList.Substring(0, length);
                            materialsList = materialsList.Remove(0, length + 1);
                            Material newMat = null;
                            if (ret == "Default-Material" || ret == "" || ret == "Resources/unity_builtin_extra")
                            {
                                newMat = AssetDatabase.GetBuiltinExtraResource<Material>("Default-Diffuse.mat");
                            }
                            else
                            {
                                newMat = (Material)AssetDatabase.LoadAssetAtPath(ret, typeof(Material));
                            }

                            renderMaterials.Add(newMat);

                        }
                    }
                    if (renderMaterials.Count > 0)
                    {
                        gOMeshRenderer.GetComponent<Renderer>().materials = renderMaterials.ToArray();
                    }
                }
            }
            else if (tag == "guilayer")
            {
                UnityEngine.GUILayer gOGuiLayer = gameObject.GetComponent<GUILayer>();
                if (gOGuiLayer == null)
                {
                    gOGuiLayer = gameObject.AddComponent<GUILayer>();
                }
            }
            else if (tag == "flarelayer")
            {
                UnityEngine.FlareLayer gOFlareLayer = gameObject.GetComponent<FlareLayer>();
                if (gOFlareLayer == null)
                {
                    gOFlareLayer = gameObject.AddComponent<FlareLayer>();
                }
            }
            else if (tag == "audiolistener")
            {
                UnityEngine.AudioListener gOAudioListener = gameObject.GetComponent<AudioListener>();
                if (gOAudioListener == null)
                {
                    gOAudioListener = gameObject.AddComponent<AudioListener>();
                }
            }
            else
            {
                Debug.Log("Unkown Componenet Type " + tag);
            }
        }
        addToMap(thisFlag);
    }
    static int genHashCode(string id)
    {
        const int primeNum = 31;
        int hashCode = 0;
        for (int i = 0; i < id.Length; ++i)
        {
            hashCode += id[i].GetHashCode();
        }
        return hashCode * primeNum;
    }

    public static void deleteGO(string info) //Function used to delete a gameobject
    {
        string gOId = deserializeString(ref info);

        int hashLoc = genHashCode(gOId);

        int xLoc = hashLoc % 10;
        int yLoc = hashLoc % 100;

        MarkerFlag thisFlag = null;

        thisFlag = findInList(gOId, xLoc, yLoc);

        if (thisFlag != null)
        {
            MonoBehaviour.DestroyImmediate(thisFlag.gameObject);
        }
    }

    public static void ReparentObjects() //Used to reparent all objects, used when server has sent over all game objects
    {
        GameObject[] allGameobjects = GameObject.FindObjectsOfType<GameObject>();   //Get all gameobjs
        EditorUtility.DisplayProgressBar("Reparenting Objects", "", 0);

        for (int i = 0; i < allGameobjects.Length; ++i)
        {
            MarkerFlag currentFlag = allGameobjects[i].GetComponent<MarkerFlag>();
            if (currentFlag.parentID != null && currentFlag.parentID != "__")
            {
                int parentHash = genHashCode(currentFlag.parentID);
                int xParent = parentHash % 10;
                int yParent = parentHash % 100;
                MarkerFlag parentFlag = findInList(currentFlag.parentID, xParent, yParent);
                if (parentFlag != null)
                {
                    allGameobjects[i].transform.SetParent(parentFlag.gameObject.transform, false); //Parent the object

                    allGameobjects[i].transform.position = currentFlag.ogPos; //Reapply the local values because they have changed with the parent
                    allGameobjects[i].transform.localScale = currentFlag.ogScale;
                    allGameobjects[i].transform.rotation = currentFlag.ogRot;
                }
                else
                {
                    allGameobjects[i].transform.SetParent(null);
                }
            }


            EditorUtility.DisplayProgressBar("Getting Level Data", allGameobjects[i].name, (float)i / allGameobjects.Length);

        }
        Lightmapping.giWorkflowMode = Lightmapping.GIWorkflowMode.Iterative;
        EditorUtility.ClearProgressBar();


    }

    public static void addToMap(MarkerFlag flag) 
    {
        int hashCode = genHashCode(flag.id); 
        int xLoc = hashCode % 10;
        int yLoc = hashCode % 100;

        if (!objectMap[xLoc,yLoc].Contains(flag)) //The flag doesn't exsist in the map anywhere
        {
            objectMap[xLoc, yLoc].Add(flag);
        }
        //Otherwise, don't bother adding it
    }

    public static MarkerFlag findInList(string id, int x, int y) //Searches a map location for a specific ID (good for really complex scenes)
    {
        for (int i = 0; i < objectMap[x, y].Count; ++i)
        {
            if (objectMap[x, y][i].id == id)
            {
                return objectMap[x, y][i];
            }
        }

        return null;
    }

    //Functions to return various types of information for interpreting the given string information
    public static MarkerFlag deserializeMarkerFlag(ref string ser)
    {
        MarkerFlag markerFlag = new MarkerFlag();
        markerFlag.id = deserializeString(ref ser);
        markerFlag.parentID = deserializeString(ref ser);
        markerFlag.isLocked = deserializeBool(ref ser);
        return markerFlag;
    }

    public static Rect deserializeRect(ref string ser)
    {
        Rect rec = new Rect();
        int length = ser.IndexOf("|");
        rec.x = float.Parse(ser.Substring(0, length));
        ser = ser.Remove(0, length + 1);
        length = ser.IndexOf("|");
        rec.y = float.Parse(ser.Substring(0, length));
        ser = ser.Remove(0, length + 1);
        length = ser.IndexOf("|");
        rec.width = float.Parse(ser.Substring(0, length));
        ser = ser.Remove(0, length + 1);
        length = ser.IndexOf("|");
        rec.height = float.Parse(ser.Substring(0, length));
        ser = ser.Remove(0, length + 1);
        return rec;
    }

    public static Color deserializeColor(ref string ser)
    {
        Color col;
        int length = ser.IndexOf("|");
        col.r = float.Parse(ser.Substring(0, length));
        ser = ser.Remove(0, length + 1);
        length = ser.IndexOf("|");
        col.g = float.Parse(ser.Substring(0, length));
        ser = ser.Remove(0, length + 1);
        length = ser.IndexOf("|");
        col.b = float.Parse(ser.Substring(0, length));
        ser = ser.Remove(0, length + 1);
        length = ser.IndexOf("|");
        col.a = float.Parse(ser.Substring(0, length));
        ser = ser.Remove(0, length + 1);
        return col;
    }

    public static int deserializeInt(ref string ser)
    {
        int length = ser.IndexOf("|");
        int ret = int.Parse(ser.Substring(0, length));
        ser = ser.Remove(0, length + 1);
        return ret;
    }

    public static string deserializeString(ref string ser)
    {
        int length = ser.IndexOf("|");
        string ret = ser.Substring(0, length);
        ser = ser.Remove(0, length + 1);
        return ret;
    }

    public static float deserializeFloat(ref string ser)
    {
        int length = ser.IndexOf("|");
        float ret = float.Parse(ser.Substring(0, length));
        ser = ser.Remove(0, length + 1);
        return ret;
    }

    public static bool deserializeBool(ref string ser)
    {
        int length = ser.IndexOf("|");
        bool ret = bool.Parse(ser.Substring(0, length));
        ser = ser.Remove(0, length + 1);
        return ret;
    }

    public static Vector3 deserializeVector3(ref string ser)
    {
        Vector3 vec;
        int length = ser.IndexOf("|");
        vec.x = float.Parse(ser.Substring(0, length));
        ser = ser.Remove(0, length + 1);
        length = ser.IndexOf("|");
        vec.y = float.Parse(ser.Substring(0, length));
        ser = ser.Remove(0, length + 1);
        length = ser.IndexOf("|");
        vec.z = float.Parse(ser.Substring(0, length));
        ser = ser.Remove(0, length + 1);
        return vec;
    }

    public static Quaternion deserializeQuaternion(ref string ser)
    {
        Quaternion vec;
        int length = ser.IndexOf("|");
        vec.x = float.Parse(ser.Substring(0, length));
        ser = ser.Remove(0, length + 1);
        length = ser.IndexOf("|");
        vec.y = float.Parse(ser.Substring(0, length));
        ser = ser.Remove(0, length + 1);
        length = ser.IndexOf("|");
        vec.z = float.Parse(ser.Substring(0, length));
        ser = ser.Remove(0, length + 1);
        length = ser.IndexOf("|");
        vec.w = float.Parse(ser.Substring(0, length));
        ser = ser.Remove(0, length + 1);
        return vec;
    }

   
}

public class serializedComponent
{
    public serializedComponent() { }

    public virtual char[] toChar() { return null; }

}

public class serMarkerFlag : serializedComponent
{
    public MarkerFlag flag;

    override public char[] toChar()
    {
        string charString = "";
        charString += flag.id + "|" + flag.parentID + "|";
        if(flag.isHeld)
        {
            charString += true + "|";
        }
        else
        {
            charString += false + "|";

        }
        return charString.ToCharArray();
    }

}

public class Transform : serializedComponent
{
    public Vector3 pos;
    public Quaternion rot;
    public Vector3 scale;
    override public char[] toChar()
    {
        string transformString = "transform|";
        transformString += pos.x + "|";
        transformString += pos.y + "|";
        transformString += pos.z + "|";
        transformString += rot.x + "|";
        transformString += rot.y + "|";
        transformString += rot.z + "|";
        transformString += rot.w + "|";
        transformString += scale.x + "|";
        transformString += scale.y + "|";
        transformString += scale.z + "|";
        return transformString.ToCharArray();
    }
}

public class BoxCollider : serializedComponent
{
    public Vector3 center;
    public Vector3 size;
    public bool isTrigger;
    override public char[] toChar()
    {
        string boxColliderString = "boxCollider|";
        boxColliderString += center.x + "|";
        boxColliderString += center.y + "|";
        boxColliderString += center.z + "|";
        boxColliderString += size.x + "|";
        boxColliderString += size.y + "|";
        boxColliderString += size.z + "|";
        boxColliderString += isTrigger + "|";
        return boxColliderString.ToCharArray();
    }
}

public class SphereCollider : serializedComponent
{
    public Vector3 center;
    public float radius;
    public bool isTrigger;
    override public char[] toChar()
    {
        string sphereColliderString = "sphereCollider|";
        sphereColliderString += center.x + "|";
        sphereColliderString += center.y + "|";
        sphereColliderString += center.z + "|";
        sphereColliderString += radius + "|";
        sphereColliderString += isTrigger + "|";
        return sphereColliderString.ToCharArray();
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
        string capsuleColliderString = "capsuleCollider|";
        capsuleColliderString += center.x + "|";
        capsuleColliderString += center.y + "|";
        capsuleColliderString += center.z + "|";
        capsuleColliderString += radius + "|";
        capsuleColliderString += height + "|";
        capsuleColliderString += directionAxis + "|";
        capsuleColliderString += isTrigger + "|";
        return capsuleColliderString.ToCharArray();
    }
}

public class RigidBody : serializedComponent
{
    public float mass, drag, angularDrag;
    public int interpolate, freeze;
    public bool useGravity, isKinematic, collisionDetection;
    override public char[] toChar()
    {
        string rigidBodyString = "rigidbody|";
        rigidBodyString += mass + "|";
        rigidBodyString += drag + "|";
        rigidBodyString += angularDrag + "|";
        rigidBodyString += interpolate + "|";
        rigidBodyString += freeze + "|";
        rigidBodyString += useGravity + "|";
        rigidBodyString += isKinematic + "|";
        rigidBodyString += collisionDetection + "|";
        return rigidBodyString.ToCharArray();
    }
}

public class MeshFilter : serializedComponent
{
    public string filePath;

    public string meshName;

    override public char[] toChar()
    {
        string meshFilterString = "meshfilter|";
        meshFilterString += filePath + "|" + meshName + "|";
        return meshFilterString.ToCharArray();
    }
}

public class MeshRenderer : serializedComponent
{
    public int lightProbe;

    public int reflectionProbe;

    //MISSING Anchor Override

    public int castShadows;

    public bool receiveShadows;

    public int motionVectors;

    public bool lightmapStatic;

    //MISSING lightmap settings and uv charting control

    public List<string> materialFiles = new List<string>();
    override public char[] toChar()
    {
        string meshRendererString = "meshRenderer|";
        meshRendererString += lightProbe.ToString() + "|";
        meshRendererString += reflectionProbe.ToString() + "|";
        meshRendererString += castShadows.ToString() + "|";
        meshRendererString += receiveShadows + "|";
        meshRendererString += motionVectors.ToString() + "|";
        meshRendererString += lightmapStatic + "|";

        for (int i = 0; i < materialFiles.Count; ++i)
        {
            meshRendererString += materialFiles[i] + ","; //Use a comma because it cannot be used by file reader/writer
        }
        meshRendererString += "|";
        return meshRendererString.ToCharArray();
    }

}

public class Camera : serializedComponent
{
    public int clearFlags;
    public Color background;
    public int cullingMask;
    public string projection;
    public float near, far;
    public Rect viewportRect;
    public int renderingPath;
    public bool HDR, MSAA, occlusionCulling;
    public float depth, fov;
    public int targetDisplay;

    override public char[] toChar()
    {
        string cameraString = "camera|";
        cameraString += clearFlags.ToString() + "|";
        cameraString += background.r + "|";
        cameraString += background.g + "|";
        cameraString += background.b + "|";
        cameraString += background.a + "|";
        cameraString += cullingMask.ToString() + "|";
        cameraString += near.ToString() + "|";
        cameraString += far.ToString() + "|";
        cameraString += viewportRect.x + "|";
        cameraString += viewportRect.y + "|";
        cameraString += viewportRect.width + "|";
        cameraString += viewportRect.height + "|";
        cameraString += renderingPath.ToString() + "|";
        cameraString += HDR.ToString() + "|";
        cameraString += MSAA.ToString() + "|";
        cameraString += occlusionCulling.ToString() + "|";
        cameraString += depth.ToString() + "|";
        cameraString += fov.ToString() + "|";
        cameraString += targetDisplay.ToString() + "|";
        return cameraString.ToCharArray();
    }
}

public class Light : serializedComponent
{
    public int type, shadows, mode, cullingMask;
    public Color color;
    public float intensity, cookie;

    override public char[] toChar()
    {
        string lightString = "light|";
        lightString += type.ToString() + "|";
        lightString += shadows.ToString() + "|";
        lightString += mode.ToString() + "|";
        lightString += cullingMask.ToString() + "|";
        lightString += color.r + "|";
        lightString += color.g + "|";
        lightString += color.b + "|";
        lightString += color.a + "|";
        lightString += intensity + "|";
        lightString += cookie + "|";
        return lightString.ToCharArray();
    }
}
