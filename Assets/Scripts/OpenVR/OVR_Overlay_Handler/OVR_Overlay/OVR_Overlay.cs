using System.Collections;
using UnityEngine;
using Valve.VR;

public partial class OVR_Overlay 
{
    public bool UpdateCurrentOverlay()
    {
        overlayHighQuality = _overlayHighQuality;
        overlayColor = _overlayColor;
        overlayAlpha = _overlayAlpha;
        overlayWidthInMeters = _overlayWidthInMeters;
        overlayTextureBounds = _overlayTextureBounds;

        overlayTransformType = _overlayTransformType;
        overlayTransform = _overlayTransform;
        overlayTransformAbsoluteTrackingOrigin = _overlayTransformAbsoluteTrackingOrigin;
        overlayTransformTrackedDeviceRelativeIndex = _overlayTransformTrackedDeviceRelativeIndex;

        overlayInputMethod = _overlayInputMethod;
        overlayVisible = _overlayVisible;

        overlayTexture = _overlayTexture;

        return !ErrorCheck(error);
    }

    public bool HideOverlay() 
    {
        overlayVisible = false;
        return !ErrorCheck(error);
    }
    public bool ShowOverlay() 
    {
        overlayVisible = true;
        return !ErrorCheck(error);
    }

    public bool ClearOverlayTexture()
    {
        if(OverlayExists && validHandle)
            error = Overlay.ClearOverlayTexture(_overlayHandle);
        
        return !ErrorCheck(error);
    }

    public bool ClearOverlayThumbnailTexture()
    {
        if(OverlayExists && validHandle && overlayIsDashboard)
            error = Overlay.ClearOverlayTexture(_overlayThumbnailHandle);

        return !ErrorCheck(error);
    }

    private bool _isMinimal = false;
    public bool OpenKeyboard(string dscrp = "", string fillTxt = "", bool minimal = false)
    {
        _isMinimal = minimal;
        
        if(OverlayExists)
            error = Overlay.ShowKeyboard(0, 0, dscrp, 256, fillTxt, minimal, 0);

        return !ErrorCheck(error);
    }

    public void CloseKeyboard()
    {
        if(OverlayExists)
            Overlay.HideKeyboard();
    }

    protected EVROverlayError error;
    protected bool ErrorCheck(EVROverlayError error)
    {
        bool err = (error != EVROverlayError.None);

        if(err)
            Debug.Log("Error: " + Overlay.GetOverlayErrorNameFromEnum(error));

        return err;
    }

    public OVR_Overlay()
    {
        OVR_Overlay_Handler.instance.RegisterOverlay(this);
    }

    ~OVR_Overlay()
    {
        OVR_Overlay_Handler.instance.DeregisterOverlay(this);
        DestroyOverlay();
    }

    public virtual bool CreateOverlay()
    {
        if(!OverlayExists)
            return ( _created = false );

        if(_overlayIsDashboard)
            error = Overlay.CreateDashboardOverlay(_overlayKey, _overlayName, ref _overlayHandle, ref _overlayThumbnailHandle);
        else
            error = Overlay.CreateOverlay(_overlayKey, _overlayName, ref _overlayHandle);

        bool allGood = !ErrorCheck(error);

        return ( _created = allGood );
    }

    public void UpdateOverlay() 
    {
        while(PollNextOverlayEvent(ref pEvent))
            DigestEvent(pEvent);
    }

    public bool DestroyOverlay()
    {
        if(!_created || !OverlayExists || !validHandle)
            return true;   

        error = Overlay.DestroyOverlay(_overlayHandle);
        _created = false;

        return _created;
    }
}
