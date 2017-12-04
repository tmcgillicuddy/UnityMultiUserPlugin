﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class Multiuser_Editor_Window : EditorWindow
{
    string serverpassword;
    string message;
    public static List<string> messageStack = new List<string>(); // THIS NEEDS TO BE A NEW DATA TYPE FOR MESSAGES (HOLD USERNAME AND MESSAGE, MAYBE TIME)
    int mode = 0;
    //int bottomBuffer = 10, topBuffer = 10;
    public static bool limitAutosave = false;
    public Vector2 scrollPos = Vector2.zero;
    public string nickName;
    public static int clientID;

    [MenuItem("Window/Multiuser Network")]
    static void init()
    {
        Multiuser_Editor_Window window = (Multiuser_Editor_Window)GetWindow(typeof(Multiuser_Editor_Window));
        messageStack.Clear();
        window.Show();
    }
    int mPortNum = 6666, mMaxClients = 10;
    string mTargetIP = "127.0.0.1";
    private void OnGUI()
    {
        if (!MultiuserPlugin.mConnected)
        {
            messageStack.Clear();
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
            mPortNum = EditorGUILayout.IntField(mPortNum);

            if(mode == 1)
            {
                GUILayout.Label("Max Connected Clients");
                mMaxClients = EditorGUILayout.IntField(mMaxClients);
            }

            EditorGUILayout.EndHorizontal();

            if (mode == 0)
            {
                EditorGUILayout.BeginHorizontal();

                GUILayout.Label("Connection IP");
                mTargetIP = EditorGUILayout.TextField(mTargetIP);

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();

                GUILayout.Label("Enter nickname:");
                nickName = EditorGUILayout.TextField(nickName);

                EditorGUILayout.EndHorizontal();
                MultiuserPlugin.toolMode = (MultiuserPlugin.mode)EditorGUILayout.EnumPopup("Mode:", MultiuserPlugin.toolMode);
            }
            else
            {
                EditorGUILayout.BeginHorizontal();

                GUILayout.Label("Server Password (leave blank if public)");
                serverpassword= EditorGUILayout.TextField(serverpassword);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Autosave interval");
                ServerUtil.saveInterval = EditorGUILayout.IntField(ServerUtil.saveInterval);
                EditorGUILayout.EndHorizontal();


                EditorGUILayout.BeginHorizontal();

                GUILayout.Label("Enter nickname:");
                nickName = EditorGUILayout.TextField(nickName);

                EditorGUILayout.EndHorizontal();

                limitAutosave = EditorGUILayout.Toggle("Limit Autosave", limitAutosave);
            }


            if (mode == 0) // client
            {
                if (GUILayout.Button("Connect"))
                {
                    //CALL CONNECT TO SERVER FUNCTION HERE
                    MultiuserPlugin.startupClient(mTargetIP, mPortNum);
                    clientID = MultiuserPlugin.clientID;
                }
            }
            else // server
            {
                if (GUILayout.Button("Start Server"))
                {
                    //CALL START SERVER FUNCTION HERE
                    MultiuserPlugin.startupServer(mPortNum, mMaxClients);
                }
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

            EditorGUILayout.BeginHorizontal();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(Screen.width), GUILayout.Height(75));//, GUILayout.ExpandHeight(false));
            //EditorGUILayout.LabelField(display, "textfield");//, GUILayout.Height(75));
            EditorGUILayout.TextArea(display, GUILayout.Width(Screen.width), GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndHorizontal();

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
                MultiuserPlugin.Disconnect();
            }

            if (GUILayout.Button("Manual Sync"))
            {
                MultiuserPlugin.Sync();
            }
            EditorGUILayout.EndHorizontal();

        }
    }

    private void Update()
    {
        if (MultiuserPlugin.newMessage)
        {
            Repaint();
            MultiuserPlugin.newMessage = false;
        }
    }

    void sendMessage()
    {
        string fullMessage = nickName + ": " + message; // full message is "nickname: message"

        if (MultiuserPlugin.mIsServer)
            messageStack.Add(fullMessage); // add users own message to the stack

        // send the message over the network
        MultiuserPlugin.SendMessageOverNetwork(fullMessage);

        MultiuserPlugin.SendMessageOverNetwork(message);

        //Clean up selection and GUI
        message = null;
        GUI.FocusControl(null);
        Repaint();
    }
}