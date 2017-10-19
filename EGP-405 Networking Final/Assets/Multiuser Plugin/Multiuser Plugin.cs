﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.Runtime.InteropServices;
 

[InitializeOnLoad]
[ExecuteInEditMode]
public class MultiuserPlugin
{
    //Importing DLL functions
    [DllImport("UnityMultiuserPlugin")]
    public static extern int Startup();
    [DllImport("UnityMultiuserPlugin")]
    public static extern int StartServer(int maxClients, int portNum);
    [DllImport("UnityMultiuserPlugin")]
    public static extern char GetStrBufOut(int index);
    [DllImport("UnityMultiuserPlugin")]
    public static extern char StartClient(string targetIP, int portNum);
    [DllImport("UnityMultiuserPlugin")]
    public static extern int UpdateNetworking();

    public static bool mConnected, mIsPaused, mIsServer;  //If the system is running;
    public static int mPortNum = 6666, maxConnectedClients = 10;      //Which port to connect through
    public static string mIP = "127.07.04"; //Which IP to connect to
    public static float syncInterval = 0f;   //How often system should sync
    static DateTime lastSyncTime = DateTime.Now;
    public static mode toolMode;
    public static string objectId;
    public static int objCounter = 0;

    public enum mode
    {
        EDIT,
        VIEW,
    }

    static MultiuserPlugin()
    {
        EditorApplication.update += Update;
        Startup();
        mConnected = false;
    }

    //Update Loop
    static void Update()
    {
        if (!Application.isPlaying && !mIsPaused)   // Only run the systems when the game is not in play mode and the user hasn't paused the sync system
        {
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
                else if(mIsServer)
                {
                    ServerUtil.saveScene();

                }
                UpdateNetworking();
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

    public static void startupServer()
    {
        objectId = "Server ";
        //Runs through entire scene and setups marker flags
        objCounter = 0;
        GameObject[] allGameobjects = GameObject.FindObjectsOfType<GameObject>();   //Get all gameobjs
        for(int i =0; i < allGameobjects.Length; ++i)
        {
            MarkerFlag objectFlag = allGameobjects[i].GetComponent<MarkerFlag>();
            if(objectFlag == null)
            {
                objectFlag = allGameobjects[i].AddComponent<MarkerFlag>();                
            }

            objectFlag.id = objectId + objCounter;

            objCounter++;
        }

        //Calls plugin function to start server
        StartServer(maxConnectedClients, mPortNum); //TODO: Add password varible

        mIsServer = true;
        mConnected = true;
        ServerUtil.forceSave(); //Save the scene to start with
    }

    public static void startupClient()
    {
        //Clears any gameobjects from the current scene //TODO: (might change to just open new scene)
        objCounter = 0;
        GameObject[] allGameobjects = GameObject.FindObjectsOfType<GameObject>();   //Get all gameobjs
        for (int i = 0; i < allGameobjects.Length; ++i)
        {
            //TODO: Destroy the objects
        }

        //TODO: Start client with given port num, targetIP and password
        StartClient(mIP, mPortNum);

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
                     //TODO: CALL SERIALIZE DATA STUFF
                     //TODO: SEND THAT DATA VIA PLUGIN
                    objectFlag.isModified = false;
                }

                if (!Selection.Contains(allGameobjects[i]))
                {
                    objectFlag.isLocked = false;
                }

            }
        }

    public static void ReceiveGOData(/*char[]*/)  //Called by C plugin to tell Unity to read in some new gameobject data
    {
        //TODO: DESERIALIZE GAMEOBJ DATA
    }

    public static void ReceiveMessageData(/*char[]*/)   //Called by C plugin to tell unity to receive some message data
    {
        //TODO: Deserialize the message data
    }

    public static void ReceiveIncomingConnection(/*char[]*/)    //Called by C plugin to tell unity that new connection is incoming
    {
        //TODO: Send handshake message, which will have the ENTIRE scene data from the server
    }


    public static void testSerialize()
    {
        Debug.Log("Testing selected obj(s)");
        if(Selection.gameObjects.Length > 0)
        {
            GameObject[] testObjs = Selection.gameObjects;
            for (int i=0; i < testObjs.Length; ++i)
            {
                //TODO: CALL THE SERLIAZE FUNCTION FOR GAMEOBJECT[i]

                //TODO: IMMEDIARTLY DESERIALIZE IT

            }
        }
    }

    private string GetDllOut()
    {
        string ret = "";
        for (int it = 0; it < 128; it++)
        {
            char val = GetStrBufOut(it);
            if (val == '\0')
            {
                it = 500;
            }
            else
            {
                ret += val;
            }
        }
        return ret;
    }

}

