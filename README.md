# OVRLay

A(n)? (Hopefully) Easy-to-use toolkit for developing OpenVR Overlays with Unity Engine!

---

What is OVRLay?
=

OVRLay is a small toolkit for creating Overlays for Open/Steam VR,

It provides the neccassary OpenVR API's, Plugins, and Unity Prefabs to enable a sort of drag-and-drop ease of use.

## Features
- Implements and Supports most OpenVR Overlay Features / Settings / Flags!

- Create and Manipulate Both In-Game, and Dashboard Menu Overlays!

- Support for the lastest Stable Release of Unity (2017.1)!

- Does not use Unity's Built-In OpenVR support, but has its own OpenVR Handler that deals with getting a connection to SteamVR, getting the HMD and Right/Left controller positions, and updating / handling Overlay Relevant OpenVR events!

- Has drop-in support for interaction with unmodified UnityUI, by simulating mouse screen cords via a in-scene camera! (Just make sure to position camera to look at WorldSpace UI!)

Basic Usage (Non - Technical)
===
The easiest method I've used, so far, is:

**1.  Set up a 'rig', or Container GameObject.** This GameObject has any elements you need and an orthographic camera positioned just so that it would render out the exact 2D image you want as an Overlay.

**2. Drag a Unity_SteamVR_Handler prefab into your scene (anywhere will do, as long as its active).** This prefab helps setup and handle SteamVR, and should have a few GameObjects representing the VR HMD, and right/left controllers as children. When Connected to SteamVR, these objects should match a players position in their play space 1:1 from 0,0,0.

**3. Drag a Unity_Overlay into your Scene and position where you would want it a players VR playspace, and set its Camera For Texture property to the camera from your Rig in step 1, and fill out its overlay Name and Key to be unique!** 
If you have 'Auto Update' enabled on both the Unity_SteamVR_Handler and Unity_Overlay objects, it should automatically handle the creation of a render texture, and rendering out a new texture for SteamVR!

That should get you the most basic form of a working SteamVR Overlay, check out the code, settings, and experiment to get a basic feel for how it works!

Advanced Usage (Technical)
===

## Coming Soon! 

But I'm not gonna be that guy, and leave you in the dark. 

The **OVR_Handler** class found in it's folder/files, and **OVR_Overlay / OVR_Overlay_Handler** classes in their folder/files, contain everything you would need to skip the prefabs, and spin your own Unity Interaction Handling while just using some easier-for-unity abstractions for SteamVR stuff.

Specifically, the OVR_Overlay class is just an abstractor that 'contains' an overlay via keeping track of its handle, and using a bunch of getters/setters to translate API calls. Just makes dev a lot faster then constantly keeping track of and calling Overlay.GetSetMaOverlaysSetting(_MaHandle,_myValues, ref _StoredVals) style things all over the place, lol.




