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
        string folderName = "Autosaved Scenes ";
        string folderTimestamp = getTimestamp(DateTime.Now, true, false);
        String newTimestamp = getTimestamp(DateTime.Now, true, true);

        if (DateTime.Now >= lastSaveTime)
        {
            Debug.Log(lastSaveTime);
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

            EditorSceneManager.SaveScene(newScene, currentFolderPath + folderName + newSceneName + ".unity", false);
            checkTooManyScenes();
        }
    }

    public static void saveAndSortScenes()
    {
        string folderName = "Autosaved Scenes ";
        string basePath = "Assets/Scenes/";
        string folderTimestamp = getTimestamp(DateTime.Now, true, false);
        String newTimestamp = getTimestamp(DateTime.Now, true, true);

        bool isNewDay = isLastFileOld();

        Debug.Log(lastSaveTime);
        Debug.Log("saveAndSortScenes()::Server side saving");

        // Get the new scene name
        String oldSceneName = EditorSceneManager.GetActiveScene().name;
        Debug.Log("Old scene name: " + oldSceneName);
        int i = 0;
        while (oldSceneName[i] != ' ')
            i++;
        String newSceneName = oldSceneName.Substring(0, i + 1) + newTimestamp;

        Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);

        EditorSceneManager.MergeScenes(EditorSceneManager.GetSceneByName(oldSceneName), newScene);
        // all the scene stuff

        string newFolderName;
        string newPath;
        if (isNewDay)
        {
            Debug.Log("is new day");
            // all the folder stuff
            newFolderName = folderName + folderTimestamp + "/";
            Debug.Log("newFolderName: " + newFolderName);
            newPath = basePath + newFolderName;
            Debug.Log("newPath: " + newPath);

            Debug.Log(AssetDatabase.CreateFolder(basePath, newFolderName));

            Debug.Log("newPath: " + newPath);


            Debug.Log("currentFolderPath + newFolderName + newSceneName: " + currentFolderPath + newFolderName + newSceneName);

            todaysFolder = newPath;
            Debug.Log("Today's folder: " + todaysFolder);

            if (isFolderFull(newPath))
            {
                Debug.Log("Folder is full");

                ++folderIteration;
                newFolderName += " (" + folderIteration + ")";
                Debug.Log(newFolderName);
                todaysFolder = newFolderName;
                AssetDatabase.CreateFolder(basePath, newFolderName);
            }
        }
        else
        {
            newFolderName = todaysFolder;
            Debug.Log("Todays folder: " + todaysFolder);
        }

            EditorSceneManager.SaveScene(newScene, currentFolderPath + newFolderName + newSceneName + ".unity", false);
    }

    public static void checkTooManyScenes()
    {
        EditorSceneManager.preventCrossSceneReferences = false;
        Debug.Log("checkTooManyScenes()");
        String path = "Assets/Scenes/Autosaved Scenes/";

        DirectoryInfo levelDirectoryPath = new DirectoryInfo(path);
        FileInfo[] scenesInfo = levelDirectoryPath.GetFiles();

        Queue<String> sceneNames = new Queue<String>();

       // EditorSceneManager.SaveOpenScenes();
        Debug.Log(EditorSceneManager.sceneCount);

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
                Debug.Log("sceneNames.Count = " + sceneNames.Count);
                string earliestScene = sceneNames.Dequeue();

                Debug.Log(AssetDatabase.DeleteAsset(path + earliestScene));

                Debug.Log(path + earliestScene + " was Deleted");
            }

        }
    }

    public static bool isLastFileOld()
    {
        bool val = false;

        string path = "Assets/Scenes/";

        DirectoryInfo levelDirectoryPath = new DirectoryInfo(path);

        DirectoryInfo[] folderInfo = levelDirectoryPath.GetDirectories();

        foreach (DirectoryInfo d in folderInfo)
        {
            Debug.Log(d.Name);
        }

        if (folderInfo.Length == 0)
            val = true;
        else
        {
            if (folderInfo.Length == 1)
            {
                if (folderInfo[0].Name == "Autosaved Scenes")
                {
                    val = true;
                    return val;
                }
            }
            string lastFolderName = folderInfo[folderInfo.Count() - 1].Name;
            //string lastFolderName = folderInfo[0].Name;
            Debug.Log(lastFolderName);
            int i = 0;
            int numSpaces = 0;
            Debug.Log(lastFolderName.Length);
            while (numSpaces < 2)
            {
                if (lastFolderName[i] == ' ')
                    numSpaces++;
                ++i;
            }

            Debug.Log(lastFolderName.Substring(i));
            if (lastFolderName.Substring(i) == getTimestamp(DateTime.Now, true, false))
                val = false;
            else
                val = true;
        }

        return val;
    }

    public static bool isFolderFull(string path)
    {
        bool val = false;

        DirectoryInfo levelDirectoryPath = new DirectoryInfo(path);
        DirectoryInfo[] folderInfo = levelDirectoryPath.GetDirectories();

        int i = 0;
        foreach (DirectoryInfo d in folderInfo)
        {
            Debug.Log(d.Name);
            if (d.Name.Contains(".meta") == false)
            {
                i++;
            }
        }

        if (i < 10)
            val = false;
        else
            val = true;

        return val;

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
