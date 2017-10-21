using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Runtime.InteropServices;

/*
  This is used to run certain fetures only the server has access to
  These Include:
  Autosave

*/

[InitializeOnLoad]
public  class ServerUtil {
    static DateTime lastSaveTime = DateTime.Now;
    public static int saveInterval = 2;

    public void Update()
    {
        saveToNewScene();
    }

    public static void forceSave()
    {
        Debug.Log("Server side saving");
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        lastSaveTime = DateTime.Now;
        Debug.Log("Scene Last Saved: " + lastSaveTime);
    }

    public static void saveScene()
    {
        if(DateTime.Now.Minute>= lastSaveTime.Minute + saveInterval)
        {
            Debug.Log("Server side saving");
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            lastSaveTime = DateTime.Now;
            Debug.Log("Scene Last Saved: " + lastSaveTime);
        }
    }

    public static void saveToNewScene()
    {
        // TODO: if there are more than x (probably gonna be 10) scenes
        // Begin deleting oldest scene upon saving of new scene
        String newTimestamp = getTimestamp(DateTime.Now);
        if (DateTime.Now >= lastSaveTime)
        {
            Debug.Log("saveToNewScene()::Server side saving");

            // Get the new scene name
            String oldSceneName = EditorSceneManager.GetActiveScene().name;
            int i = 0;
            while (oldSceneName[i] != ' ')
                i++;
            String newSceneName = oldSceneName.Substring(0, i + 1) + newTimestamp;
            // got the new scene name

            // create a new scene 
            Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
            // created a new scene

            // copy everything from old scene into new scene
            EditorSceneManager.MergeScenes(EditorSceneManager.GetSceneByName(oldSceneName), newScene);
            // copied everything from old scene into new scene

            // save new scene
            EditorSceneManager.SaveScene(newScene, "Assets/Scenes/" + newSceneName + ".unity");
            // saved new scene
        }
    }

    public static String getTimestamp(DateTime val)
    {
        return val.ToString("yyyy.MM.dd HH.mm.ss");
    }
}
