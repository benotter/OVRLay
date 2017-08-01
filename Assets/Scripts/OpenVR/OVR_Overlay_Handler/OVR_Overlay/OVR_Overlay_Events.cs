using System.Collections;
using UnityEngine;
using Valve.VR;

public partial class OVR_Overlay 
{
    public DashboardChange onDashboardChange = delegate(bool open){};
    public FocusChange onFocusChange = delegate(bool hasFocus){};
    public VisibilityChanged onVisibilityChange = delegate(bool visibility){};

    public KeyboardInput onKeyboardInput = delegate(string input){};
    public KeyboardClosed onKeyboardClosed = delegate(){};
    public KeyboardDone onKeyboardDone = delegate(){};

    protected VREvent_t pEvent;
    protected bool PollNextOverlayEvent(ref VREvent_t pEvent)
    {
		if (!OverlayExists)
			return false;

		var size = (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(Valve.VR.VREvent_t));

		return Overlay.PollNextOverlayEvent(_overlayHandle, ref pEvent, size);
    }

    public delegate void DashboardChange(bool open);
    public delegate void FocusChange(bool hasFocus);
    public delegate void VisibilityChanged(bool visibility);

    public delegate void KeyboardInput(string input);
    public delegate void KeyboardClosed();
    public delegate void KeyboardDone();
    
    
    protected virtual void DigestEvent(VREvent_t pEvent)
    {
        EVREventType eventType = (EVREventType) pEvent.eventType;

        switch(eventType)
        {
            case EVREventType.VREvent_MouseMove:
                UpdateMouseData(pEvent.data.mouse);
            break;

            case EVREventType.VREvent_MouseButtonDown:
                UpdateMouseData(pEvent.data.mouse, true);
            break;

            case EVREventType.VREvent_MouseButtonUp:
                UpdateMouseData(pEvent.data.mouse, false);
            break;

            case EVREventType.VREvent_FocusEnter:
                onFocusChange(true);
                _focus = true;
            break;

            case EVREventType.VREvent_FocusLeave:
                onFocusChange(false);
                _focus = false;
            break;

            case EVREventType.VREvent_DashboardActivated:
                onDashboardChange(true);
            break;

            case EVREventType.VREvent_DashboardDeactivated:
                onDashboardChange(false);
            break;

            case EVREventType.VREvent_OverlayShown:
                onVisibilityChange(true);
            break;

            case EVREventType.VREvent_OverlayHidden:
                onVisibilityChange(false);
            break;

            case EVREventType.VREvent_KeyboardCharInput:
                string txt = "";
                var kd = pEvent.data.keyboard;
                byte[] bytes = new byte[]
                {
                    kd.cNewInput0,
                    kd.cNewInput1,
                    kd.cNewInput2,
                    kd.cNewInput3,
                    kd.cNewInput4,
                    kd.cNewInput5,
                    kd.cNewInput6,
                    kd.cNewInput7,
                };
                int len = 0;
                while(bytes[len++] != 0 && len < 7);
                string input = System.Text.Encoding.UTF8.GetString(bytes, 0, len);

                if(_isMinimal)
                    txt = input;
                else
                {
                    System.Text.StringBuilder txtB = new System.Text.StringBuilder(1024);
                    Overlay.GetKeyboardText(txtB, 1024);
                    txt = txtB.ToString();
                }

                onKeyboardInput(txt);
            break;

            case EVREventType.VREvent_KeyboardDone:
                onKeyboardDone();
            break;

            case EVREventType.VREvent_KeyboardClosed:
                onKeyboardClosed();
            break;

            // case EVREventType.VREvent_DashboardActivated:
            // break;

            default:
                // Debug.Log("Overlay - " + overlayName + " - : " + eventType);
            break;
        }
    }
}