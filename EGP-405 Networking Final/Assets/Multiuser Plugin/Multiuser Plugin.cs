using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public class MultiuserPlugin
{
    public static bool mConnected, mIsPaused;  //If the system is running;
    public static int mPortNum = 6666, maxConnectedClients;      //Which port to connect through
    public static string mIP = "127.07.04"; //Which IP to connect to
    public static float syncInterval = 0f;   //How often system should sync
    static DateTime lastSyncTime = DateTime.Now;
    public static mode toolMode;
    public static string clientID;
    public static int objCounter = 0;

    public enum mode
    {
        EDIT,
        VIEW
    }

    static MultiuserPlugin()
    {
        EditorApplication.update += Update;
        mConnected = false;

    }

    //Update Loop
    static void Update()
    {
        if (!Application.isPlaying && !mIsPaused)
        {
            if (toolMode == mode.EDIT)
            {
                editMode();
            }
            else if (toolMode == mode.VIEW)
            {
                viewMode();
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
        //Name all current gameobjs on server side

        //Start server with given information
    }

    public static void startupClient()
    {
        //Clear current scene and prepare to recieve level data from server

        //Request connection
    }

    public static void Sync()   //Sends out the data of the "modified" objects
    {
            GameObject[] allGameobjects = GameObject.FindObjectsOfType<GameObject>();
            Debug.Log("Syncing");

            for (int i = 0; i < allGameobjects.Length; ++i) //Checks All objects in scene and 
            {
                MarkerFlag objectFlag = allGameobjects[i].GetComponent<MarkerFlag>();
                if (objectFlag == null)    //If an object doesn't have the marker flag script on it
                {                                                           //it will be added. This happens when a client makes a new obj
                    objectFlag = allGameobjects[i].AddComponent<MarkerFlag>();
                    objectFlag.name = clientID + objCounter.ToString(); //Make a uniquie name for the client so that other objects can get confused by it
                    objCounter++;
                }

                if (objectFlag.isModified)    //If this object's marker flag has been modified
                {
                    Component[] allComponenets = allGameobjects[i].GetComponents<Component>();
                    for (int j = 0; j < allComponenets.Length; ++j)    //Checks the marked gameobject's componentss
                    {
                        //CALL SERIALIZE DATA STUFF
                        //SEND THAT DATA VIA PLUGIN
                    }
                    objectFlag.isModified = false;
                }

                if (!Selection.Contains(allGameobjects[i]))
                {
                    objectFlag.isLocked = false;
                }

            }
        }

    public static void ReceiveData(/*char[]*/)  //Called by C plugin to tell Unity to read in some data
    {
        //CONNECTION REQUEST STATUS
        //DESERIALIZE MESSAGE
        //DESERIALIZE OTHER FEATURE
        //DESERIALIZE GAMEOBJ DATA
    }

    public static void testSerialize()
    {
        Debug.Log("Testing selected obj(s)");
        if(Selection.gameObjects.Length > 0)
        {
            GameObject[] testObjs = Selection.gameObjects;
            for (int i=0; i < testObjs.Length; ++i)
            {
                //CALL THE SERLIAZE FUNCTION FOR GAMEOBJECT[i]

                //IMMEDIARTLY DESERIALIZE IT

            }
        }
    }

 }

