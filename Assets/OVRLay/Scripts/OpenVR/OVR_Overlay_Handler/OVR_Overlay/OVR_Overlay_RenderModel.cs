using System;
using System.Collections;
using System.Runtime.InteropServices;

using UnityEngine;
using Valve.VR;

public partial class OVR_Overlay 
{
    public void Test1(string modelPath = "")
    {
        if(modelPath == "")
            modelPath = Application.dataPath + "/RenderModel Tests/test_obj/test_obj.json";

        string rmPath = modelPath;

        Debug.Log("RenderModel Path: \r\n" + rmPath);

        RenderModel_t render = new RenderModel_t();

        IntPtr renderP = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(RenderModel_t)));
        Marshal.StructureToPtr(render, renderP, false);

        EVRRenderModelError error = EVRRenderModelError.None;

        do
        {
            error = RenderModels.LoadRenderModel_Async(rmPath, ref renderP);
        }
        while( (error == EVRRenderModelError.Loading) );
        

        if(error != EVRRenderModelError.None)
            Debug.Log("RenderModel Error: " + error );
    }

    public void Test2(string modelPath) 
    {
        overlayRenderModel = modelPath;
        if(ErrorCheck(error))
            Debug.Log("Trouble Setting Render Model!");
    }
}