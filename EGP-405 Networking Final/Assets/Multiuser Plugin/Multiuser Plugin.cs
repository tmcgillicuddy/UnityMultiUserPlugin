using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public class MultiuserPlugin
{
    public static bool mConnected;
    public static int mPortNum = 6666;
    public static string mIP = "127.07.04";

    static MultiuserPlugin()
    {
        EditorApplication.update += Update;
        mConnected = false;

    }

    //Update Loop
    static void Update()
    {
        if(mConnected)
        {
            Debug.Log("Updating");
        }
    }
}
