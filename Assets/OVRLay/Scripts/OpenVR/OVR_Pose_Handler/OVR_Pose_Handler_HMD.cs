using System.Collections;
using UnityEngine;
using Valve.VR;

public partial class OVR_Pose_Handler 
{
    public Vector3 GetEyeTransform(EVREye eye) 
    {
        if(!SysExists)
            return Vector3.zero;

        return (new OVR_Utils.RigidTransform(VRSystem.GetEyeToHeadTransform(eye))).pos;
    }

    public Vector3 GetRightEyeTransform()
    {
        return GetEyeTransform(EVREye.Eye_Right);
    }

    public Vector3 GetLeftEyeTransform()
    {
        return GetEyeTransform(EVREye.Eye_Left);
    }
}