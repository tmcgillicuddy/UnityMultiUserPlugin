using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class Multiuser_Editor_Window : EditorWindow
{
    [MenuItem("Window/Multiuser Network")]
    static void init()
    {
        Multiuser_Editor_Window window = (Multiuser_Editor_Window)GetWindow(typeof(Multiuser_Editor_Window));
        window.Show();
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();

        GUILayout.Label("Connection Port Number");
        MultiuserPlugin.mPortNum = EditorGUILayout.IntField(MultiuserPlugin.mPortNum);

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        GUILayout.Label("Connection IP");
        MultiuserPlugin.mIP = EditorGUILayout.TextField(MultiuserPlugin.mIP);

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        GUILayout.Label("Sync Interval (0 for realtime)");
        MultiuserPlugin.syncInterval = EditorGUILayout.FloatField(MultiuserPlugin.syncInterval);
        GUILayout.Label("seconds");
        EditorGUILayout.EndHorizontal();

        MultiuserPlugin.mConnected = GUILayout.Toggle(MultiuserPlugin.mConnected, "Run");

        if(GUILayout.Button("Manual Sync"))
        {
            MultiuserPlugin.Sync();
        }
    }
}
