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
    //public static int numSavedScenes = EditorSceneManager.loadedSceneCount;
    public const int MAX_SAVED_SCENES = 10;

    public void Update()
    {
        checkTooManyScenes();
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

    public string[] scenes;
    public static void checkTooManyScenes()
    {
        int numSavedScenes = 0;
        /*
        List<string> tmp = new List<string>();
        foreach (UnityEditor.EditorBuildSettingsScene S in UnityEditor.EditorBuildSettings.scenes)
        {
            if (S.enabled)
            {
                string name = S.path.Substring(S.path.LastIndexOf('/') + 1);
                name = name.Substring(0, name.Length - 6);
                tmp.Add(name);
            }
            numSavedScenes++;
        }
        */
        SceneSetup[] tmpScenes = EditorSceneManager.GetSceneManagerSetup();
        string[] tmp = new string[MAX_SAVED_SCENES];
        for (int i = 0; i < tmpScenes.Length; i++)
        {
            Debug.Log("Scene: " + tmpScenes[i].path);
            tmp[i] = tmpScenes[i].path.Substring(tmpScenes[i].path.LastIndexOf('/') + 1);
            numSavedScenes++;
        }

        Debug.Log(numSavedScenes + " saved scenes");

        string earliestScene = tmp[0];
        int j = 0;
        while (earliestScene[j] != ' ')
            j++;
        string earliestSceneTimestamp = earliestScene.Substring(j + 1, earliestScene.Length - 1);
        for (int i = 0; i < numSavedScenes; ++i)
        {
            string currentSceneTimestamp = tmp[i].Substring(j + 1, tmp[i].Length);
            if (Convert.ToDouble(currentSceneTimestamp) < Convert.ToDouble(earliestSceneTimestamp))
                earliestScene = tmp[i];
        }

        EditorSceneManager.UnloadSceneAsync(earliestScene);

        Debug.Log(earliestScene + " was Deleted");
    }

    public static String getTimestamp(DateTime val)
    {
        return val.ToString("yyyy.MM.dd HH.mm.ss");
    }
}
