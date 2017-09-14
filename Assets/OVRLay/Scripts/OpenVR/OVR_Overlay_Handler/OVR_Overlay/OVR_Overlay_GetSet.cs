using System.Collections;
using UnityEngine;
using Valve.VR;

public partial class OVR_Overlay 
{
    protected OVR_Handler OVR { get { return OVR_Handler.instance; } }
    public bool OverlayExists 
    { 
        get 
        {
            return OVR.Overlay != null;
        } 
    }
    protected CVROverlay Overlay { get { return OVR.Overlay; } }
    protected CVRRenderModels RenderModels { get { return OVR.RenderModels; } }

    protected bool _overlayIsDashboard = false;
    public bool overlayIsDashboard 
    {
        get { return _overlayIsDashboard; }
        set { _overlayIsDashboard = value; }
    }

    protected bool validHandle
    {
        get 
        {
            return _overlayHandle != OpenVR.k_ulOverlayHandleInvalid;
        } 
    }

    protected ulong _overlayHandle = OpenVR.k_ulOverlayHandleInvalid;
    public ulong overlayHandle { get { return _overlayHandle; } }

    private ulong _overlayThumbnailHandle = OpenVR.k_ulOverlayHandleInvalid;
    private ulong overlayThumbnailHandle { get { return _overlayThumbnailHandle; } }

    protected string _overlayName = "OpenVR Overlay";
    public string overlayName 
    {
        get { return _overlayName; }
        set { _overlayName = value; }
    }

    protected string _overlayKey = "open_vr_overlay";
    public string overlayKey 
    {
        get {return _overlayKey; }
        set { _overlayKey = value; }
    }
    
    protected bool _created = false;
    public bool created { get { return _created; } }
    

    protected bool _focus = false;
    public bool focus { get { return _focus; } }


    protected bool _overlayHighQuality = false;
    public bool overlayHighQuality 
    {
        get 
        {
            if(OverlayExists && validHandle)
                _overlayHighQuality = ( Overlay.GetHighQualityOverlay() == _overlayHandle );
            
            return _overlayHighQuality;
        }
        set 
        {
            _overlayHighQuality = value;

            if(value && OverlayExists && validHandle)
                error = Overlay.SetHighQualityOverlay(_overlayHandle);
        }
    }

    protected Color _overlayColor = Color.white;
    public Color overlayColor 
    {
        get 
        {
            if(OverlayExists && validHandle)
            {
                float r = 0f, g = 0f, b = 0f;

                error = Overlay.GetOverlayColor(_overlayHandle, ref r, ref g, ref b);

                _overlayColor.r = r;
                _overlayColor.g = g;
                _overlayColor.b = b;
            }

            return _overlayColor;
        }
        set 
        {
            _overlayColor = value;

            if(OverlayExists && validHandle)
                error = Overlay.SetOverlayColor(_overlayHandle, _overlayColor.r, _overlayColor.g, _overlayColor.b);
        }
    }

    protected float _overlayAlpha = 1f;
    public float overlayAlpha 
    {
        get 
        { 
            if(OverlayExists && validHandle)
                error = Overlay.GetOverlayAlpha(_overlayHandle, ref _overlayAlpha);

            return _overlayAlpha;
        }

        set 
        {
            _overlayAlpha = value;

            if(OverlayExists && validHandle)
                error = Overlay.SetOverlayAlpha(_overlayHandle, _overlayAlpha);
        }
    }

    protected float _overlayWidthInMeters = 0f;
    public float overlayWidthInMeters 
    {
        get 
        {
            if(OverlayExists && validHandle)
                error = Overlay.GetOverlayWidthInMeters(_overlayHandle, ref _overlayWidthInMeters);

            return _overlayWidthInMeters;
        }
        set 
        {
            _overlayWidthInMeters = value;

            if(OverlayExists && validHandle)
                error = Overlay.SetOverlayWidthInMeters(_overlayHandle, value);
        }
    }

    // Skipping overlayAutoCurveDistanceRangeInMeters

    protected VRTextureBounds_t _overlayTextureBounds = new VRTextureBounds_t();
    public VRTextureBounds_t overlayTextureBounds 
    {
        get 
        {
            if(OverlayExists && validHandle)
                error = Overlay.GetOverlayTextureBounds(_overlayHandle, ref _overlayTextureBounds);
                
            return _overlayTextureBounds;
        }
        set 
        {
            _overlayTextureBounds = value;

            if(OverlayExists && validHandle)
                error = Overlay.SetOverlayTextureBounds(_overlayHandle, ref _overlayTextureBounds);
        }
    }

    protected VRTextureBounds_t _overlayThumbnailTextureBounds = new VRTextureBounds_t();
    public VRTextureBounds_t overlayThumbnailTextureBounds 
    {
        get 
        {
            if(OverlayExists && validHandle)
                error = Overlay.GetOverlayTextureBounds(_overlayThumbnailHandle, ref _overlayThumbnailTextureBounds);
                
            return _overlayThumbnailTextureBounds;
        }
        set 
        {
            _overlayThumbnailTextureBounds = value;

            if(OverlayExists && validHandle)
                error = Overlay.SetOverlayTextureBounds(_overlayThumbnailHandle, ref _overlayThumbnailTextureBounds);
        }
    }

    private float _overlayTexelAspect = 1f;
    public float overlayTexelAspect 
    {
        get 
        {
            if(OverlayExists && validHandle)
                error = Overlay.GetOverlayTexelAspect(_overlayHandle, ref _overlayTexelAspect);

            return _overlayTexelAspect;
        }

        set 
        {
            _overlayTexelAspect = value;
            if(OverlayExists && validHandle)
                error = Overlay.SetOverlayTexelAspect(_overlayHandle, _overlayTexelAspect);
        }
    }

    private uint [] _overlaySize = new uint[2] {0, 0};
    public uint [] overlaySize 
    {
        get 
        {
            uint width = _overlaySize[0];
            uint height = _overlaySize[1];
            
            
            if(OverlayExists && validHandle)
                error = Overlay.GetOverlayImageData(_overlayHandle, System.IntPtr.Zero, 0, ref width, ref height);
            
            _overlaySize[0] = width;
            _overlaySize[1] = height;

            return _overlaySize;
        }
    }


    protected ETrackingUniverseOrigin _overlayTransformAbsoluteTrackingOrigin = ETrackingUniverseOrigin.TrackingUniverseStanding;
    public ETrackingUniverseOrigin overlayTransformAbsoluteTrackingOrigin 
    {
        get { return _overlayTransformAbsoluteTrackingOrigin; }
        set { _overlayTransformAbsoluteTrackingOrigin = value; }
    }

    protected VROverlayTransformType _overlayTransformType = VROverlayTransformType.VROverlayTransform_Absolute;
    public VROverlayTransformType overlayTransformType 
    {
        get 
        {
            if(OverlayExists && validHandle)
                error = Overlay.GetOverlayTransformType(_overlayHandle, ref _overlayTransformType);

            return _overlayTransformType;
        }
        set 
        {
            _overlayTransformType = value;
        }
    }

    protected uint _overlayTransformTrackedDeviceRelativeIndex = OpenVR.k_unTrackedDeviceIndexInvalid;
    public uint overlayTransformTrackedDeviceRelativeIndex 
    {
        get { return _overlayTransformTrackedDeviceRelativeIndex; }
        set { 
                _overlayTransformTrackedDeviceRelativeIndex = value;
                overlayTransform = _overlayTransform;
            }
    }
    protected HmdMatrix34_t _overlayTransform;
    public HmdMatrix34_t overlayTransform 
    {
        get 
        {
            if(OverlayExists && validHandle)
            {
                VROverlayTransformType type = _overlayTransformType;
                switch(type)
                {
                    default:
                    case VROverlayTransformType.VROverlayTransform_Absolute:

                        error = Overlay.GetOverlayTransformAbsolute(
                            _overlayHandle, 
                            ref _overlayTransformAbsoluteTrackingOrigin, 
                            ref _overlayTransform);

                    break;

                    case VROverlayTransformType.VROverlayTransform_TrackedDeviceRelative:

                        error = Overlay.GetOverlayTransformTrackedDeviceRelative(
                            _overlayHandle,
                            ref _overlayTransformTrackedDeviceRelativeIndex,
                            ref _overlayTransform);

                    break;
                }
            }
            
            return _overlayTransform;
        }

        set 
        {
            _overlayTransform = value;

            if(OverlayExists && validHandle)
            {
                VROverlayTransformType type = _overlayTransformType;
                switch(type)
                {
                    default:
                    case VROverlayTransformType.VROverlayTransform_Absolute:

                        error = Overlay.SetOverlayTransformAbsolute(
                            _overlayHandle, 
                            _overlayTransformAbsoluteTrackingOrigin, 
                            ref _overlayTransform);

                    break;

                    case VROverlayTransformType.VROverlayTransform_TrackedDeviceRelative:

                        error = Overlay.SetOverlayTransformTrackedDeviceRelative(
                            _overlayHandle,
                            _overlayTransformTrackedDeviceRelativeIndex,
                            ref _overlayTransform);

                    break;
                }
            }
        }
    }

    protected bool _overlayVisible = false;
    public bool overlayVisible 
    {
        get 
        {
            if(OverlayExists && validHandle)
                _overlayVisible = Overlay.IsOverlayVisible(_overlayHandle);
            
            return _overlayVisible;
        }

        set 
        {
            _overlayVisible = value;

            if(OverlayExists && validHandle)
                if(value)
                {
                    error = Overlay.ShowOverlay(_overlayHandle);
                    if(_overlayIsDashboard)
                        Overlay.ShowOverlay(_overlayThumbnailHandle);
                }
                    
                else
                    error = Overlay.HideOverlay(_overlayHandle);
        }
    }

    protected VROverlayInputMethod _overlayInputMethod = VROverlayInputMethod.None;
    public VROverlayInputMethod overlayInputMethod 
    {
        get 
        {
            if(OverlayExists && validHandle)
                error = Overlay.GetOverlayInputMethod(_overlayHandle, ref _overlayInputMethod);
            
            return _overlayInputMethod;
        }

        set 
        {
            _overlayInputMethod = value;

            if(OverlayExists && validHandle)
                error = Overlay.SetOverlayInputMethod(_overlayHandle, _overlayInputMethod);
        }
    }

    protected HmdVector2_t _overlayMouseScale = new HmdVector2_t();
    public HmdVector2_t overlayMouseScale 
    {
        get 
        {
            if(OverlayExists && validHandle)
                error = Overlay.GetOverlayMouseScale(_overlayHandle, ref _overlayMouseScale);

            return _overlayMouseScale;
        }

        set 
        {
            _overlayMouseScale.v0 = value.v0;
            _overlayMouseScale.v1 = value.v1;

            if(OverlayExists && validHandle)
                error = Overlay.SetOverlayMouseScale(_overlayHandle, ref _overlayMouseScale);
        }
    }

    protected ETextureType _overlayTextureType;
    public ETextureType overlayTextureType 
    { 
        get { return _overlayTextureType; } 
        set { _overlayTextureType = value; }
    }

    protected Texture _overlayTexture;
    protected Texture_t _overlayTexture_t = new Texture_t();
    public Texture overlayTexture 
    {
        set 
        {   
            if(!value)
                return;
                
            _overlayTexture = value;

            _overlayTexture_t.handle = _overlayTexture.GetNativeTexturePtr();
            _overlayTexture_t.eType = overlayTextureType;
            _overlayTexture_t.eColorSpace = EColorSpace.Auto;

            if(OverlayExists && validHandle)
                error = Overlay.SetOverlayTexture(_overlayHandle, ref _overlayTexture_t);
        }
    }

    private Texture _overlayThumbnailTexture;
    protected Texture_t _overlayThumbnailTexture_t = new Texture_t();
    public Texture overlayThumbnailTexture 
    {
        set 
        {
            _overlayThumbnailTexture = value;

            _overlayThumbnailTexture_t.handle = _overlayThumbnailTexture.GetNativeTexturePtr();
            _overlayThumbnailTexture_t.eType = overlayTextureType;
            _overlayThumbnailTexture_t.eColorSpace = EColorSpace.Auto;

            if(OverlayExists && overlayIsDashboard)
                error = Overlay.SetOverlayTexture(_overlayThumbnailHandle, ref _overlayThumbnailTexture_t);
        }
    }

    private HmdColor_t _overlayRenderModelColor = new HmdColor_t();
    public Color overlayRenderModelColor 
    {
        get 
        {
            Color c = new Color();

            c.r = _overlayRenderModelColor.r;
            c.g = _overlayRenderModelColor.g;
            c.b = _overlayRenderModelColor.b;
            c.a = _overlayRenderModelColor.a;

            return c;
        }
        set 
        {
            _overlayRenderModelColor.r = value.r;
            _overlayRenderModelColor.g = value.g;
            _overlayRenderModelColor.b = value.b;
            _overlayRenderModelColor.a = value.a;
        }
    }

    private string _overlayRenderModel = "";
    public string overlayRenderModel 
    {
        get 
        {
            return _overlayRenderModel;
        }
        set 
        {
            if(OverlayExists && validHandle)
                error = Overlay.SetOverlayRenderModel(_overlayHandle, value, ref _overlayRenderModelColor);
               
            _overlayRenderModel = value;
        }
    }

    // Overlay Flags

    public bool GetFlag(VROverlayFlags flag) 
    {
        if(!OverlayExists || !validHandle)
            return false;

        bool flagged = false;

        error = Overlay.GetOverlayFlag(_overlayHandle, flag, ref flagged);

        return flagged;
    }

    public void SetFlag(VROverlayFlags flag, bool val)
    {
        if(OverlayExists && validHandle)
            error = Overlay.SetOverlayFlag(_overlayHandle, flag, val);
    }

    public bool overlayFlag_Curved
    {
        get { return GetFlag(VROverlayFlags.Curved); }
        set { SetFlag(VROverlayFlags.Curved, value); }
    }

    public bool overlayFlag_ShowScrollWheel
    {
        get { return GetFlag(VROverlayFlags.ShowTouchPadScrollWheel); }
        set { SetFlag(VROverlayFlags.ShowTouchPadScrollWheel, value); }
    }

    public bool overlayFlag_SideBySide_Crossed
    {
        get { return GetFlag(VROverlayFlags.SideBySide_Crossed); }
        set { SetFlag(VROverlayFlags.SideBySide_Crossed, value); }
    }

    public bool overlayFlag_SideBySide_Parallel
    {
        get { return GetFlag(VROverlayFlags.SideBySide_Parallel); }
        set { SetFlag(VROverlayFlags.SideBySide_Parallel, value); }
    }
}