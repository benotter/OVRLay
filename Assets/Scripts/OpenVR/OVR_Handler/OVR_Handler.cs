using System.Collections;
using UnityEngine;
using Valve.VR;

public partial class OVR_Handler : System.IDisposable
{
    public void UpdateAll()
    {
        while(PollNextEvent(ref pEvent))
            DigestEvent(pEvent);
        
        poseHandler.UpdatePoses();
        overlayHandler.UpdateOverlays();
    }

    private EVRInitError error = EVRInitError.None;
    private VREvent_t pEvent = new VREvent_t();

    public bool StartupOpenVR()
    {
        _VRSystem = OpenVR.Init(ref error, _applicationType);

        bool result = !ErrorCheck(error);
        
        if(result)
        {
            GetOpenVRExistingInterfaces();
            onOpenVRChange.Invoke(true);
        }
        else
            ShutDownOpenVR(); // GetOpenVRInterfaces();

        return result;
    }
    public void GetOpenVRExistingInterfaces()
    {
        _Compositor = OpenVR.Compositor;
        _Chaperone = OpenVR.Chaperone;
        _ChaperoneSetup = OpenVR.ChaperoneSetup;
        _Overlay = OpenVR.Overlay;
        _Settings = OpenVR.Settings;
        _Applications = OpenVR.Applications;
    }

    public bool ShutDownOpenVR()
    {
        _VRSystem = null;

        _Compositor = null;
        _Chaperone = null;
        _ChaperoneSetup = null;
        _Overlay = null;
        _Settings = null;
        _Applications = null;

        overlayHandler.DestroyAllOverlays();
        OpenVR.Shutdown();

        return false;
    }

    private bool ErrorCheck(EVRInitError error)
    {
        bool err = (error != EVRInitError.None);

        if(err)
            Debug.Log("VR Error: " + OpenVR.GetStringForHmdError(error));

        return err;
    }

    ~OVR_Handler()
    {
        Dispose();
    }

    public void Dispose()
    {
        ShutDownOpenVR();
        _instance = null;
    }

    public void SafeDispose()
    {
        if(_instance != null)
            return;
        _instance = null;
    }
}