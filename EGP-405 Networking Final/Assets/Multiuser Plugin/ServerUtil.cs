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
public class ServerUtil
{
    public static int saveInterval = 2;
    public const int MAX_SAVED_SCENES = 10;
    public static string currentFolderPath = "Assets/Scenes/";


    public void Update()
    {
    }

    public static void saveToNewScene()
    {
        string folderName = "Autosaved Scenes";
        string newTimestamp = "TS-" + getTimestamp(DateTime.Now, true, true);

        int tsLength = newTimestamp.Length;

        string newSceneName, oldSceneName;
        Scene newScene;

        int length = EditorSceneManager.GetActiveScene().name.IndexOf("TS-"); // delimiter
                                                                              // create the new scene
                                                                              // if we are not currently in a scene

        if (EditorSceneManager.GetActiveScene().name == "") // if we are not in a scene
        {
            newSceneName = "Autosaved Scene" + newTimestamp;
            oldSceneName = "";

            newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        }

        else // if we are in a scene
        {
            if (length < 0) // if the scene name does not contain the delimiter
            {
                newSceneName = "Test Scene " + newTimestamp;

                oldSceneName = EditorSceneManager.GetActiveScene().name;
            }
            // if there is an open scene
            else
            {
                // Get the new scene name
                oldSceneName = EditorSceneManager.GetActiveScene().name;

                newSceneName = oldSceneName.Substring(0, length - 1) + " " + newTimestamp;
                // got the new scene name
            }
            // create new scene
            newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);

            // copy everything from old scene into new scene
            EditorSceneManager.MergeScenes(EditorSceneManager.GetSceneByName(oldSceneName), newScene);
            // copied everything from old scene into new scene
        }

        string savedScene = currentFolderPath + folderName + "/" + newSceneName + ".unity";
        EditorSceneManager.SaveScene(newScene, savedScene, false);
    }

    public static void checkTooManyScenes()
    {
        string path;
        if (EditorSceneManager.GetActiveScene().name == "") // if we are not in a scene
            path = "Assets/Scenes/Autosaved Scenes/"; // path to autosaved scenes
        else // if we are
        {
            // get file the scene is in
            int lim = EditorSceneManager.GetActiveScene().name.Length;
            path = EditorSceneManager.GetActiveScene().path.Substring(0, lim - 2); // take scene name out of path
        }

        DirectoryInfo levelDirectoryPath = new DirectoryInfo(path); // asset information at path
        FileInfo[] scenesInfo = levelDirectoryPath.GetFiles();  // array of files at the path

        Queue<String> sceneNames = new Queue<String>(); // queue of names in the file

        int i = 0;
        foreach (FileInfo fi in scenesInfo)
        {
            if (fi.Name.Contains(".meta") == false) // filter out .meta files
            {
                sceneNames.Enqueue(fi.Name); // add appropriate file names into the queue
                i++;
            }
        }

        if (sceneNames.Count > 10) // if there are more than 10 scenes saved
        {
            while (sceneNames.Count > 10) // run loop while there are more than 10 scenes in folder
            {
                string earliestScene = sceneNames.Dequeue(); // name of earliest scene in the file
                AssetDatabase.DeleteAsset(path + earliestScene); // delete the scene
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
