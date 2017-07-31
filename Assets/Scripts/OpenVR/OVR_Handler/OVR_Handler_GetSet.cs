using System.Collections;
using UnityEngine;
using Valve.VR;

public partial class OVR_Handler 
{
    static private OVR_Handler _instance;
    static public OVR_Handler instance 
    {
        get {
            if(_instance == null)
                _instance = new OVR_Handler();

            return _instance;
        }
    }
    public bool OpenVRConnected { get { return (_VRSystem != null); } }

    private CVRSystem _VRSystem;
    public CVRSystem VRSystem { get { return _VRSystem; } }

    private CVRCompositor _Compositor;
    public CVRCompositor Compositor { get { return _Compositor; } }

    private CVRChaperone _Chaperone;
    public CVRChaperone Chaperone { get { return _Chaperone; } }

    private CVRChaperoneSetup _ChaperoneSetup;
    public CVRChaperoneSetup ChaperoneSetup { get { return _ChaperoneSetup; } }

    private CVROverlay _Overlay;
    public CVROverlay Overlay { get { return _Overlay; } }

    private CVRSettings _Settings;
    public CVRSettings Settings { get { return _Settings; } }

    private CVRApplications _Applications;
    public CVRApplications Applications { get { return _Applications; } }


    private EVRApplicationType _applicationType = EVRApplicationType.VRApplication_Background;
    public EVRApplicationType applicationType { get { return _applicationType; } }
    

    private OVR_Pose_Handler _poseHandler;
    public OVR_Pose_Handler poseHandler 
    { 
        get 
        { 
            if(_poseHandler == null)
                _poseHandler = OVR_Pose_Handler.instance;

            return _poseHandler; 
        }
    }

    private OVR_Overlay_Handler _overlayHandler;
    public OVR_Overlay_Handler overlayHandler 
    { 
        get 
        { 
            if(_overlayHandler == null)
                _overlayHandler = OVR_Overlay_Handler.instance;

            return _overlayHandler; 
        } 
    }
}