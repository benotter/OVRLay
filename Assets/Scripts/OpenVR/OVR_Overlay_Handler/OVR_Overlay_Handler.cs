using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class OVR_Overlay_Handler 
{
    private static OVR_Overlay_Handler _instance;
    public static OVR_Overlay_Handler instance 
    {
        get 
        {
            if(_instance == null)
                _instance = new OVR_Overlay_Handler();
            
            return _instance;
        }
    }

    private HashSet<OVR_Overlay> overlays = new HashSet<OVR_Overlay>();

    public void UpdateOverlays()
    {
        foreach(OVR_Overlay overlay in overlays)
            overlay.UpdateOverlay();
    }

    public void DestroyAllOverlays()
    {
        foreach(OVR_Overlay overlay in overlays)
            overlay.DestroyOverlay();
    }

    public bool RegisterOverlay(OVR_Overlay overlay)
    {
        return overlays.Add(overlay);
    }

    public bool DeregisterOverlay(OVR_Overlay overlay)
    {
        return overlays.Remove(overlay);
    }
}