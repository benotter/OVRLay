using System.Collections;
using UnityEngine;
using Valve.VR;

public partial class OVR_Handler 
{
    public OpenVRChange onOpenVRChange = delegate(bool connected){};
    public StandbyChange onStandbyChange = delegate(bool inStandbyMode){};
    public DashboardChange onDashboardChange = delegate(bool open){};
    public ChaperoneChange onChaperoneChange = delegate(){};

    private bool PollNextEvent(ref VREvent_t pEvent)
    {
        if(VRSystem == null)
            return false;

		var size = (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(Valve.VR.VREvent_t));
		return VRSystem.PollNextEvent(ref pEvent, size);
    }

    public delegate void OpenVRChange(bool connected);
    public delegate void StandbyChange(bool inStandbyMode);
    public delegate void DashboardChange(bool open);

    public delegate void ChaperoneChange();

    private void DigestEvent(VREvent_t pEvent) 
    {
        EVREventType type = (EVREventType) pEvent.eventType;
        switch(type)
        {
            case EVREventType.VREvent_Quit:
                Debug.Log("VR - QUIT - EVENT");
                onOpenVRChange(false);
            break;
            
            case EVREventType.VREvent_DashboardActivated:
                onDashboardChange(true);
            break;
            case EVREventType.VREvent_DashboardDeactivated:
                onDashboardChange(false);
            break;

            case EVREventType.VREvent_EnterStandbyMode:
                onStandbyChange(true);
            break;
            case EVREventType.VREvent_LeaveStandbyMode:
                onStandbyChange(false);
            break;

            case EVREventType.VREvent_ChaperoneSettingsHaveChanged:
                onChaperoneChange();
            break;
        }

        onVREvent.Invoke(pEvent);
    }
}