using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class Multiuser_Editor_Window : EditorWindow
{

    bool test;


    [MenuItem("Window/Multiuser Network")]
    static void init()
    {
        Multiuser_Editor_Window window = (Multiuser_Editor_Window)GetWindow(typeof(Multiuser_Editor_Window));
        window.Show();
    }

    private void OnGUI()
    {

        if (!MultiuserPlugin.mConnected)
        {
            EditorGUILayout.BeginHorizontal();

            GUILayout.Label("Connection Port Number");
            MultiuserPlugin.mPortNum = EditorGUILayout.IntField(MultiuserPlugin.mPortNum);

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            GUILayout.Label("Connection IP");
            MultiuserPlugin.mIP = EditorGUILayout.TextField(MultiuserPlugin.mIP);

            EditorGUILayout.EndHorizontal();

            MultiuserPlugin.toolMode = (MultiuserPlugin.mode)EditorGUILayout.EnumPopup("Mode:", MultiuserPlugin.toolMode);

            if (GUILayout.Button("Connect"))
            {
                MultiuserPlugin.mConnected = true;
            }
        }
        else
        {
            EditorGUILayout.BeginHorizontal();

            GUILayout.Label("Sync Interval (0 for realtime)");
            MultiuserPlugin.syncInterval = EditorGUILayout.FloatField(MultiuserPlugin.syncInterval);
            GUILayout.Label("seconds");

            if(MultiuserPlugin.mIsPaused)
            {
                if (GUILayout.Button("Unpause"))
                {
                    MultiuserPlugin.mIsPaused = false;
                }
            }
            else
            {
                if (GUILayout.Button("Pause"))
                {
                    MultiuserPlugin.mIsPaused = true;
                }
            }
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Disconnect"))
            {
                MultiuserPlugin.mConnected = false;
            }

            if (GUILayout.Button("Manual Sync"))
            {
                MultiuserPlugin.Sync();
            }
            EditorGUILayout.EndHorizontal();

        }
    }
}
