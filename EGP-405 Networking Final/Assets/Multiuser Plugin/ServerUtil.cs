using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.Runtime.InteropServices;

/*
 * This is used to run certain fetures only the server has access to
  These Include:
  Autosave

*/
[InitializeOnLoad]
public class ServerUtil {
    DateTime lastSaveTime = DateTime.Now;
    public int saveInterval = 2;

    public void saveScene()
    {
        if(DateTime.Now.Minute>= lastSaveTime.Minute + saveInterval)
        {
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            lastSaveTime = DateTime.Now;
            Debug.Log("Scene Last Saved: " + lastSaveTime);
        }
    }
}
