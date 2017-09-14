using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;

using UnityEngine;
using Valve.VR;

public partial class OVR_Overlay 
{
    public RenderModel_t renderModel;
    public IntPtr renderModelP;

    public RenderModel_TextureMap_t renderModelTexMap;
    public IntPtr renderModelTexMapP;


    public void Test1(string modelPath)
    {
        Debug.Log("RenderModel Path: \r\n" + modelPath);

        if( LoadRenderModel(modelPath) && LoadRenderModelTexture(renderModel, renderModelP) )
            Debug.Log("RenderModel Loaded Succesfully!");
        
    }

    public bool LoadRenderModel(string modelPath)
    {
        renderModel = new RenderModel_t();

        renderModelP = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(RenderModel_t)));
        Marshal.StructureToPtr(renderModel, renderModelP, false);

        EVRRenderModelError error = EVRRenderModelError.None;
        while( (EVRRenderModelError.Loading == (error = RenderModels.LoadRenderModel_Async(modelPath, ref renderModelP))) ){};    

        if(error != EVRRenderModelError.None)
        {
            Debug.Log("RenderModel Error: " + error );
            return false;
        }

        return true;
    }

    public bool LoadRenderModelTexture(RenderModel_t rend, IntPtr rendP)
    {
        renderModelTexMap = new RenderModel_TextureMap_t();

        renderModelTexMapP = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(RenderModel_TextureMap_t)));
        Marshal.StructureToPtr(renderModelTexMap, renderModelTexMapP, false);

        EVRRenderModelError error = EVRRenderModelError.None;
        while(EVRRenderModelError.Loading == (error = RenderModels.LoadTexture_Async(rend.diffuseTextureId, ref renderModelTexMapP))){};

        if(error != EVRRenderModelError.None)
        {
            Debug.Log("Error Loading Render Model Texture: " + error);
            RenderModels.FreeRenderModel(rendP);
            return false;
        }
        
        return true;
    }

    public void Test2(string modelPath)
    {
        overlayRenderModelColor = Color.white;
        overlayRenderModel = modelPath;

        if(ErrorCheck(error))
            Debug.Log("Trouble Setting Render Model!");
    }
}