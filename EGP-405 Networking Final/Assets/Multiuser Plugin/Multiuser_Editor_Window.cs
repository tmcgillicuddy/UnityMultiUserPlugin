using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class Multiuser_Editor_Window : EditorWindow
{
    string serverpassword;
    string message;
    static List<string> messageStack = new List<string>(); // THIS NEEDS TO BE A NEW DATA TYPE FOR MESSAGES (HOLD USERNAME AND MESSAGE, MAYBE TIME)
    int mode = 0;
    int bottomBuffer = 10, topBuffer = 10;

    [MenuItem("Window/Multiuser Network")]
    static void init()
    {
        Multiuser_Editor_Window window = (Multiuser_Editor_Window)GetWindow(typeof(Multiuser_Editor_Window));
        messageStack.Clear();
        window.Show();
    }
   
    private void OnGUI()
    {


        if (!MultiuserPlugin.mConnected)
        {
            if(mode == 1)
            {
                GUILayout.Label("Server Settings:", EditorStyles.boldLabel);
            }
            else
            {
                GUILayout.Label("Client Settings:", EditorStyles.boldLabel);
            }



            EditorGUILayout.BeginHorizontal();

            GUILayout.Label("Connection Port Number");
            MultiuserPlugin.mPortNum = EditorGUILayout.IntField(MultiuserPlugin.mPortNum);

            EditorGUILayout.EndHorizontal();

            if (mode == 0)
            {
                EditorGUILayout.BeginHorizontal();

                GUILayout.Label("Connection IP");
                MultiuserPlugin.mIP = EditorGUILayout.TextField(MultiuserPlugin.mIP);

                EditorGUILayout.EndHorizontal();
                MultiuserPlugin.toolMode = (MultiuserPlugin.mode)EditorGUILayout.EnumPopup("Mode:", MultiuserPlugin.toolMode);
            }
            else
            {
                EditorGUILayout.BeginHorizontal();

                GUILayout.Label("Server Password (leave blank if public)");
                serverpassword= EditorGUILayout.TextField(serverpassword);

                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Connect"))
            {
                MultiuserPlugin.mConnected = true;
            }

            if (mode == 1)
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Change to Client Setting"))
                {
                    mode = 0;
                }
                EditorGUILayout.EndHorizontal();

            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Change to Server Mode"))
                {
                    mode = 1;
                }
                EditorGUILayout.EndHorizontal();

            }
        }
        else
        {
            //Draw the current stack of messages
            
            string display = "";
            for (int i=0; i < messageStack.Count; ++i)
            {
                display = display + messageStack[i] + "\n";
            }

            GUILayout.Label(display, "textfield");


            EditorGUILayout.BeginHorizontal();
            message = EditorGUILayout.TextField("Message", message);
            if (GUILayout.Button("Send", GUILayout.Width(50)))
            {
                sendMessage();
            }
            EditorGUILayout.EndHorizontal();


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


    void sendMessage()
    {
        messageStack.Add(message);

        //Clean up selection and GUI
        message = null;
        GUI.FocusControl(null);
        Repaint();
    }
}
