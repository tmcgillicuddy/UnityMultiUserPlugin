# Multiuser Plugin For Unity Editor

### Description

This plugin can be used with the Unity Editor to give multiple users the ability to edit a given scene simultaneously. Changes made by users are shared with each other and are updated in the local version of the scene in realtime.

It uses Raknet to transmit and receive data, which is then translated in the Unity Editor into updates for the various gameobjects in the scene. One instance of Unity functions as the Server that controls the scene, the messages being relayed, and other various authoritative functions.

This repo includes an example unity project, the compiled plugin with relevant unity editor scripts, and the source for the plugin to see how they talk to each other. 

This project was a joint final project for our Networking for Games course in the Fall of 2017. The main contributors to the project are Aaron William Hamilton, Thomas Ota-McGillicuddy, and James Smith.

## Dependecies
The Multiuser Plugin uses these libraries:

 * [Raknet](http://www.jenkinssoftware.com/) - Networking API
 * [Unity Editor 2017.4](https://unity3d.com/) - Unity Scenes

## Using Plugin:

Drag and drop the [Multieuser Plugin](https://github.com/tmcgillicuddy/UnityMultiUserPlugin/tree/develop/Multiuser%20Plugin) folder into your Assets Folder.

Open the project as you would any other Unity Project

Under the "Multiuser Editor" tab, select "Settings"

Select either to act as the Server or a Client

### As Server
Set the desired port, max number of clients that can connect, how often the scenes should autosave,
the name of the server, and whether you want to limit the number of autosaves or not.

Click "Start Server" and you're good!

### As Client
Set the server's target port, the IP of the server, a nickname for your client, and which edit mode
you should run in.

Click "Connect" and you're good!