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
    public static int folderIteration = 0;
    public static int saveInterval = 2;
    public const int MAX_SAVED_SCENES = 10;
    public static string currentFolderPath = "Assets/Scenes/";
    public static bool newDay = false;
    public static string todaysFolder;


    public void Update()
    {
        //checkTooManyScenes();
        //saveToNewScene();
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
        string folderName = "Autosaved Scenes";
        String newTimestamp = getTimestamp(DateTime.Now, true, true);

        string newSceneName, oldSceneName;
        Scene newScene;

        if (DateTime.Now >= lastSaveTime)
        {
            // create the new scene
            // if we are not currently in a scene
            if (EditorSceneManager.GetActiveScene().name == "")
            {
                newSceneName = "Test Scene " + newTimestamp;
            
                // create new scene
                newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            }
            // if we are currently in a scene
            else
            {
                // Get the new scene name
                oldSceneName = EditorSceneManager.GetActiveScene().name;
                int i = 0;
                while (oldSceneName[i] != ' ')
                    i++;
                newSceneName = oldSceneName.Substring(0, i + 1) + newTimestamp;
                // got the new scene name

                // create new scene
                newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);

                // copy everything from old scene into new scene
                EditorSceneManager.MergeScenes(EditorSceneManager.GetSceneByName(oldSceneName), newScene);
                // copied everything from old scene into new scene
            }

            string savedScene = currentFolderPath + folderName + "/" + newSceneName + ".unity";
            EditorSceneManager.SaveScene(newScene, savedScene, false);
            Debug.Log(savedScene + " was Saved");
        }
    }

    public static void checkTooManyScenes()
    {
        EditorSceneManager.preventCrossSceneReferences = false;
        String path = "Assets/Scenes/Autosaved Scenes/";

        DirectoryInfo levelDirectoryPath = new DirectoryInfo(path);
        FileInfo[] scenesInfo = levelDirectoryPath.GetFiles();

        Queue<String> sceneNames = new Queue<String>();

        int i = 0;
        foreach (FileInfo fi in scenesInfo)
        {
            if (fi.Name.Contains(".meta") == false)
            {
                sceneNames.Enqueue(fi.Name);
                i++;
            }
        }

        if (sceneNames.Count > 10)
        {
            while (sceneNames.Count > 10)
            {
                string earliestScene = sceneNames.Dequeue();

                AssetDatabase.DeleteAsset(path + earliestScene);
                Debug.Log(path + earliestScene + " was Deleted");
            }

        }
    }

    public static String getTimestamp(DateTime val, bool date, bool time)
    {
        if (date && !time)
            return val.ToString("yyyy.MM.dd");
        else if (!date && time)
            return val.ToString("HH.mm.ss");
        else
            return val.ToString("yyyy.MM.dd HH.mm.ss");
    }
}
