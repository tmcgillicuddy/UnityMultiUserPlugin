/*
EGP 405-01 Final Project 12/7/17
Aaron Hamilton
James Smith
Thomas McGillicuddy
“We certify that this work is
entirely our own. The assessor of this project may reproduce this project
and provide copies to other academic staff, and/or communicate a copy of
this project to a plagiarism-checking service, which may retain a copy of the
project on its database.”
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class MultiuserEditor : EditorWindow
{
    string message;
    public static List<string> messageStack = new List<string>(); // THIS NEEDS TO BE A NEW DATA TYPE FOR MESSAGES (HOLD USERNAME AND MESSAGE, MAYBE TIME)
    int mode = 0;
    public static bool limitAutosave = false;
    public Vector2 scrollPos = Vector2.zero;
    public string nickName = "";
    public static int clientID;

    [MenuItem("Multiuser Editor/Settings")]
    static void init()
    {
        MultiuserEditor window = (MultiuserEditor)GetWindow(typeof(MultiuserEditor),false,"Multiuser Editor Settings");
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
                GUILayout.Label("Server Settings:", EditorStyles.boldLabel);
            else
                GUILayout.Label("Client Settings:", EditorStyles.boldLabel);

            // connection port number
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Connection Port Number");
            mPortNum = EditorGUILayout.IntField(mPortNum);

            if(mode == 1)
            {
                // max connected clients
                GUILayout.Label("Max Connected Clients");
                mMaxClients = EditorGUILayout.IntField(mMaxClients);
            }

            EditorGUILayout.EndHorizontal();

            if (mode == 0)
            {
                // IP to connect to
                EditorGUILayout.BeginHorizontal();

                GUILayout.Label("Connection IP");
                mTargetIP = EditorGUILayout.TextField(mTargetIP);

                EditorGUILayout.EndHorizontal();

                // client nickname
                EditorGUILayout.BeginHorizontal();

                GUILayout.Label("Enter nickname:");
                nickName = EditorGUILayout.TextField(nickName);

                EditorGUILayout.EndHorizontal();

                // toggle edit or view mode
                MultiuserPlugin.toolMode = (MultiuserPlugin.mode)EditorGUILayout.EnumPopup("Mode:", MultiuserPlugin.toolMode);
            }
            else
            {
                // autosave interval
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Autosave interval");
                ServerUtil.saveInterval = EditorGUILayout.IntField(ServerUtil.saveInterval);
                EditorGUILayout.EndHorizontal();

                // nickname 
                EditorGUILayout.BeginHorizontal();

                GUILayout.Label("Enter nickname:");
                nickName = EditorGUILayout.TextField(nickName);

                EditorGUILayout.EndHorizontal();

                // limit autosave to 10 autosaved scenes folder
                limitAutosave = EditorGUILayout.Toggle("Limit Autosave", limitAutosave);
            }

            if (mode == 0) // client
            {
                if (GUILayout.Button("Connect"))
                {
                    // start client
                    MultiuserPlugin.startupClient(mTargetIP, mPortNum);
                    clientID = MultiuserPlugin.clientID;
                    if (nickName == "")
                        nickName = "Client";
                    MultiuserPlugin.objectId = nickName;
                }
            }
            else // server
            {
                // start server
                if (GUILayout.Button("Start Server"))
                {
                    MultiuserPlugin.startupServer(mPortNum, mMaxClients);
                    if (nickName == "")
                        nickName = "Server";
                    MultiuserPlugin.objectId = nickName;
                }
            }

            if (mode == 1) // server
            {
                // change to client mode
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Change to Client Setting"))
                {
                    mode = 0;
                    nickName = "Client";
                }
                EditorGUILayout.EndHorizontal();
            }
            else // client
            {
                // change to server mode
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Change to Server Mode"))
                {
                    mode = 1;
                    nickName = "Server";
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        else
        {
            //Draw the current stack of messages
            string display = "";
            for (int i=0; i < messageStack.Count; ++i)
                display = display + messageStack[i] + "\n";

            // view messages
            EditorGUILayout.BeginHorizontal();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(Screen.width), GUILayout.Height(75));
            EditorGUILayout.TextArea(display, GUILayout.Width(Screen.width), GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndHorizontal();

            // send message
            EditorGUILayout.BeginHorizontal();
            Event e = Event.current;
            bool enterHit = false;
            if (e.keyCode == KeyCode.Return && message != null)
                enterHit = true;
            message = EditorGUILayout.TextField("Message", message);
            if (GUILayout.Button("Send", GUILayout.Width(50)) || enterHit)
            {
                enterHit = false;
                sendMessage();
            }
            EditorGUILayout.EndHorizontal();

            // sync interval
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Sync Interval (0 for realtime)");
            MultiuserPlugin.syncInterval = EditorGUILayout.FloatField(MultiuserPlugin.syncInterval);
            GUILayout.Label("seconds");

            if(MultiuserPlugin.mIsPaused)
                if (GUILayout.Button("Unpause"))
                    MultiuserPlugin.mIsPaused = false;
            else
                if (GUILayout.Button("Pause"))
                    MultiuserPlugin.mIsPaused = true;

            EditorGUILayout.EndHorizontal();

            // Disconnect button
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Disconnect"))
            {
                MultiuserPlugin.Disconnect();
                if (mode == 0)
                    nickName = "Client";
                else
                    nickName = "Server";
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
        string fullMessage = MultiuserPlugin.objectId + ": " + message;

        if (MultiuserPlugin.mIsServer)
            messageStack.Add(fullMessage); // add users own message to the stack

        // send the message over the network
        MultiuserPlugin.SendMessageOverNetwork(fullMessage);

        //Clean up selection and GUI
        message = null;
        GUI.FocusControl(null);
        Repaint();
    }
}