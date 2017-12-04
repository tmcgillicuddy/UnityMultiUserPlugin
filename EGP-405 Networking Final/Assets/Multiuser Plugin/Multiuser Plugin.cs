using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Net;


[InitializeOnLoad]
[ExecuteInEditMode]
public class MultiuserPlugin
{
    //Importing DLL functions
    [DllImport("UnityMultiuserPlugin")]
    public static extern int StartServer(string targetIP, int portNum, int maxClients);
    [DllImport("UnityMultiuserPlugin")]
    public static extern int StartClient(string targetIP, int portNum, int maxClients);
    [DllImport("UnityMultiuserPlugin")]
    public static extern unsafe char* GetData();
    [DllImport("UnityMultiuserPlugin")]
    public static extern unsafe int SendData(string data, int length, string ownerIP);
    [DllImport("UnityMultiuserPlugin")]
    public static extern int Shutdown();
    [DllImport("UnityMultiuserPlugin")]
    public static extern unsafe char* GetLastPacketIP();

    //Unity Varibles
    public static bool mConnected, mIsPaused, mIsServer;  //If the system is running;
    public static float syncInterval = 0.5f;   //How often system should sync
    static DateTime lastSyncTime = DateTime.Now;
    public static mode toolMode;
    public static string objectId;
    public static int objCounter = 0;
    public static string serverIP;
    public static int clientID;
    public static bool newMessage;

    struct ConnectedClientInfo  //For storing connected client information
    {
        public string IP;
        public string userName;
        public string ID;
    }


    static List<ConnectedClientInfo> mConnectedClients = new List<ConnectedClientInfo>();

    public enum mode
    {
        EDIT,
        VIEW,
    }

    static MultiuserPlugin()
    {
        EditorApplication.update += Update;
        mConnected = false;
    }

    //Update Loop
    static void Update()
    {

        if (!Application.isPlaying && !mIsPaused)   // Only run the systems when the game is not in play mode and the user hasn't paused the sync system
        {
            //Debug.Log(mConnected);
            if (mConnected)
            {
                if (toolMode == mode.EDIT)
                {
                    editMode();
                }
                else if (toolMode == mode.VIEW)
                {
                    viewMode();
                }
                else if (mIsServer)
                {
                    //ServerUtil.saveScene();
                    // ServerUtil.saveToNewScene();
                }
                checkData();
            }
        }

    }

    static void editMode()
    {
        if (Selection.gameObjects.Length > 0)
        {
            GameObject[] selectedObjects = Selection.gameObjects;

            for (int i = 0; i < selectedObjects.Length; ++i)
            {
                MarkerFlag selectedObjFlags = selectedObjects[i].GetComponent<MarkerFlag>();
                if (selectedObjFlags == null)    //If an object doesn't have the marker flag script on it
                {                                                           //it will be added
                    selectedObjFlags = selectedObjects[i].AddComponent<MarkerFlag>();
                    selectedObjFlags.id = objectId + objCounter.ToString();
                    objCounter++;
                }
                selectedObjFlags.isModified = true;
                selectedObjFlags.isLocked = true;
            }
        }

        //If the system is running AND the sync interval is 0 or if the current time is greater than the last sync time + the sync interval
        if (mConnected && (syncInterval == 0 || DateTime.Now.Minute * 60 + DateTime.Now.Second >=
            (lastSyncTime.Second + syncInterval + lastSyncTime.Minute * 60)))
        {
            Sync();
            lastSyncTime = DateTime.Now;
        }
        else
        {
            //  Debug.Log((DateTime.Now.Minute*60+ DateTime.Now.Second) - (lastSyncTime.Second + syncInterval + lastSyncTime.Minute * 60));
            //  Debug.Log(DateTime.Now.Second);
        }
    }

    static void viewMode()
    {
        Selection.activeObject = null;

    }

    public static void startupServer(int portNum, int maxClients)
    {
        StructScript.init();
        EditorUtility.DisplayProgressBar("Setting up", "", 0);
        objectId = "Server ";
        //Runs through entire scene and setups marker flags
        objCounter = 0;
        GameObject[] allGameobjects = GameObject.FindObjectsOfType<GameObject>();   //Get all gameobjs
        for (int i = 0; i < allGameobjects.Length; ++i)
        {
            MarkerFlag objectFlag = allGameobjects[i].GetComponent<MarkerFlag>();
            if (objectFlag == null)
            {
                objectFlag = allGameobjects[i].AddComponent<MarkerFlag>();
            }

            objectFlag.id = objectId + objCounter;

            StructScript.addToMap(objectFlag);

            objCounter++;
            EditorUtility.DisplayProgressBar("Setting up", allGameobjects[i].name, (float)i / (allGameobjects.Length - 1));

        }

        // get server ip address
        string hostName = System.Net.Dns.GetHostName();
        IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(hostName);
        IPAddress[] addr = ipEntry.AddressList;

        int index = 0;
        foreach (IPAddress i in addr)
        {
            //Debug.Log("Address " + index + ": " + i.ToString() + " ");
            index++;
        }
        serverIP = addr[3].ToString();
       // Debug.Log("Server IP: " + serverIP);


        //Calls plugin function to start server
        StartServer(serverIP, portNum, maxClients);

        mIsServer = true;
        mConnected = true;
        //ServerUtil.forceSave(); //Save the scene to start with

        EditorUtility.ClearProgressBar();

        ServerUtil.saveToNewScene();
        if (Multiuser_Editor_Window.limitAutosave)
            ServerUtil.checkTooManyScenes();

        

    }

    public static void startupClient(string targetIP, int portNum)
    {
        StructScript.init();

        //Clears any gameobjects from the current scene //TODO: (might change to just open new scene)
        objCounter = 0;
        GameObject[] allGameobjects = GameObject.FindObjectsOfType<GameObject>();   //Get all gameobjs
        for (int i = 0; i < allGameobjects.Length; ++i)
        {
            //TODO: Destroy the objects
        }

        //TODO: Start client with given port num, targetIP and password
        StartClient(targetIP, portNum, 0);

        mIsServer = false;
        mConnected = true;
    }

    public static void Sync()   //Sends out the data of the "modified" objects
    {
        GameObject[] allGameobjects = GameObject.FindObjectsOfType<GameObject>();
        //Debug.Log("Syncing");

        for (int i = 0; i < allGameobjects.Length; ++i) //Checks All objects in scene and 
        {
            MarkerFlag objectFlag = allGameobjects[i].GetComponent<MarkerFlag>();   //TODO: Potentially expensive might change

            if (objectFlag == null)    //If an object doesn't have the marker flag script on it
            {                          //it will be added. This happens when a new object has been made

                objectFlag = allGameobjects[i].AddComponent<MarkerFlag>();
                objectFlag.name = objectId + objCounter; //Make a uniquie name for the client so that other objects can't get confused by it
                objCounter++;
            }

            if (objectFlag.isModified)    //If this object's marker flag has been modified
            {
                string temp = StructScript.serialize(allGameobjects[i]);

                if (!mIsServer)
                {
                    //   Debug.Log("Test Sending to server"); 
                    SendData(temp, temp.Length, "");
                }
                else
                {
                    //  Debug.Log("Test Broadcasting");

                    for (int j = 0; j < mConnectedClients.Count; ++j)
                    {
                       // Debug.Log(mConnectedClients[j].IP);
                        if (mConnectedClients[j].IP != "")
                        {
                            SendData(temp, temp.Length, mConnectedClients[j].IP);
                        }
                    }
                }
                objectFlag.isModified = false;
            }

            if (!Selection.Contains(allGameobjects[i]))
            {
                objectFlag.isLocked = false;
            }

        }
    }
    unsafe struct MyStringStruct
    {
        public int id;
        public fixed char pseudoString[512];
    }
    static unsafe void checkData()  //Checks the plugin network loop for a packet
    {
        //char* data = null;
        char* data = GetData();
        // Debug.Log(data[0]);
        if (data == null)
        {
            //Debug.Log(temp);
            return;
        }
        StructScript.deserializeMessage(data);

    }

    public static unsafe void addClient()
    {
        char* ip = GetLastPacketIP();
        IntPtr careTwo = (IntPtr)ip;
        StraightCharPointer* dataTwo = (StraightCharPointer*)careTwo;
        //string start = Marshal.PtrToStringAnsi((IntPtr)dataTwo->mId);
        string newIP = Marshal.PtrToStringAnsi((IntPtr)dataTwo->mes);
        Debug.Log(newIP);
        ConnectedClientInfo newClient = new ConnectedClientInfo();
        newClient.IP = newIP;
        mConnectedClients.Add(newClient);

        //Send a data buffer of all the objects currently in the scene to the newly connected client
        GameObject[] allGameobjects = GameObject.FindObjectsOfType<GameObject>();   //Get all gameobjs
        for (int i=0; i < allGameobjects.Length; ++i)
        {
            string temp = StructScript.serialize(allGameobjects[i]);
            SendData(temp, temp.Length, newIP);
        }

        for (int i = 0; i < allGameobjects.Length; ++i)
        {
            string temp = StructScript.serialize(allGameobjects[i]);
            SendData(temp, temp.Length, newIP);
        }

    }

    public static void SendMessageOverNetwork(string message)
    {
        switch (mIsServer)
        {
            case true:
                {
                    foreach (ConnectedClientInfo c in mConnectedClients)
                    {
                        SendData(message, message.Length, c.IP);
                    }
                    break;
                }

            case false:
                {
                    SendData(message, message.Length, serverIP);
                    break;
                }
        }
    }

    public static void Disconnect()
    {
        Shutdown();
        mConnected = false;
    }
}

