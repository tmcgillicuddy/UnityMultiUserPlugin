using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public class MultiuserPlugin
{
    public static bool mConnected;  //If the system is running;
    public static int mPortNum = 6666;      //Which port to connect through
    public static string mIP = "127.07.04"; //Which IP to connect to
    public static float syncInterval = 0f;   //How often system should sync
    static DateTime lastSyncTime = DateTime.Now;

    enum supportedTypes
    {
        Transform,
        RigidBody
    }

    static MultiuserPlugin()
    {
        EditorApplication.update += Update;
        mConnected = false;

    }

    //Update Loop
    static void Update()
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
        if (mConnected && (syncInterval == 0 ||DateTime.Now.Minute*60+ DateTime.Now.Second >= 
            (lastSyncTime.Second+syncInterval+ lastSyncTime.Minute*60)))
        {
           
            Sync();
            lastSyncTime = DateTime.Now;
            //EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
        else
        {
          //  Debug.Log((DateTime.Now.Minute*60+ DateTime.Now.Second) - (lastSyncTime.Second + syncInterval + lastSyncTime.Minute * 60));
            //  Debug.Log(DateTime.Now.Second);
        }
    }

    public static void Sync()
    {
        GameObject[] allGameobjects = GameObject.FindObjectsOfType<GameObject>();
        Debug.Log("Syncing");
        for (int i = 0; i < allGameobjects.Length; ++i) //Checks All objects in scene and 
        {
            if (allGameobjects[i].GetComponent<MarkerFlag>() == null)    //If an object doesn't have the marker flag script on it
            {                                                           //it will be added
                allGameobjects[i].AddComponent<MarkerFlag>();
            }

            MarkerFlag objectFlag = allGameobjects[i].GetComponent<MarkerFlag>();

            if (objectFlag.isModified)    //If this object's marker flag has been modified
            {
                Component[] allComponenets = allGameobjects[i].GetComponents<Component>();
                for (int j = 0; j < allComponenets.Length; ++j)    //Checks the marked gameobject's componentss
                {
                    //THIS IS WHERE WE WILL COMPILE DATA TO SEND
                }
                objectFlag.isModified = false;
            }

            if (!Selection.Contains(allGameobjects[i]))
            {
                objectFlag.isLocked = false;
            }

        }
    }

}
