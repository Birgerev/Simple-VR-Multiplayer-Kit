Thank you so much for purchasing our asset!

If you ever need support, feel free to reach out over at our
Discord Server: https://discord.gg/KbFKA5Z6Sr
We'd love if you'd share your games there as well!


BEFORE YOU START
- Install following packages: "XR Interaction Toolkit", "OpenXR Plugin", "Mirror"
- Set up your OpenXR input devices in Project Settings
- Make sure you set up "Player" & "Grabbable" layers like in "physics layers.png". They must have same layer ids


FOR USING MULTIPLAYER
- by default, port 630 is used (you can change this in Multiplayer Manager).
- make sure you Port Forward port 630 to your pc, so players outside your network can connect
- Open port 630 in Windows Firewall
- If you still can't connect, make sure you have your network type set to Private (not Public)


HOW TO SET UP YOUR OWN GRABBABLE OBJECTS
- Set up your grabbable as normal
- If you want it to be synced over network, you must have the "NetworkIdentity" component
- Use components like "NetworkTransform" & "NetworkRigidbody" to sync position, velocity etc
- Authority etc is handled automatically through our Multiplayer XR Rig
- Read more on networking over at: https://mirror-networking.gitbook.io/docs/
- Have a look at "Physics Cube", as it is the simplest example
