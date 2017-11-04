using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Linq;
using System.Runtime.InteropServices;
using System.IO;

/*
  This is used to run certain fetures only the server has access to
  These Include:
  Autosave

*/

[InitializeOnLoad]
public  class ServerUtil {
    static DateTime lastSaveTime = DateTime.Now;
    public static int saveInterval = 2;
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
            EditorSceneManager.SaveScene(newScene, "Assets/Scenes/Autosaved Scenes/" + newSceneName + ".unity", false);
            // saved new scene
        }
    }

    public string[] scenes;
    public static void checkTooManyScenes()
    {
        EditorSceneManager.preventCrossSceneReferences = false;
        Debug.Log("checkTooManyScenes()");
        string path = "Assets/Scenes/Autosaved Scenes/";

        DirectoryInfo levelDirectoryPath = new DirectoryInfo(path);
        FileInfo[] scenesInfo = levelDirectoryPath.GetFiles();

        Queue<string> sceneNames = new Queue<string>();

        EditorSceneManager.SaveOpenScenes();
        Debug.Log(EditorSceneManager.sceneCount);

        int i = 0;
        foreach (FileInfo fi in scenesInfo)
        {
            if (fi.Name.Contains(".meta") == false)
            {
                sceneNames.Enqueue(fi.Name);//.Substring(0, fi.Name.Length - 6)); // add name to queue without .unity extension
                //Debug.Log("Found Scene " + fi.Name + " at " + i);
                i++;
            }
        }

        if (sceneNames.Count > 10)
        {
            while (sceneNames.Count > 10)
            {
                Debug.Log("sceneNames.Count = " + sceneNames.Count);
                string earliestScene = sceneNames.Dequeue();
                Debug.Log(earliestScene);

                Scene scene = EditorSceneManager.GetSceneByName(earliestScene);

                //Debug.Log(EditorSceneManager.CloseScene(scene, true));
                //EditorSceneManager.Destroy(scene);
                EditorSceneManager.OpenScene(path + earliestScene, OpenSceneMode.Single);
                Debug.Log("Active Scene: " + EditorSceneManager.GetActiveScene().name);
                Debug.Log("Deleted Scene: " + EditorSceneManager.CloseScene(SceneManager.GetActiveScene(), true));

                Debug.Log(path + earliestScene + " was Deleted");
            }

        }
    }

    public static String getTimestamp(DateTime val)
    {
        return val.ToString("yyyy.MM.dd HH.mm.ss");
    }
}
