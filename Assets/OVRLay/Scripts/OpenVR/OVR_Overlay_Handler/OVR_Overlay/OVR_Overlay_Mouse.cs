using System.Collections;
using UnityEngine;
using Valve.VR;


public partial class OVR_Overlay 
{
    protected Vector2 _mousePos = new Vector2();
    public Vector2 overlayMousePosition { get { return _mousePos; } }

    protected bool _mouseLeftDown = false;
    public bool overlayMouseLeftDown { get { return _mouseLeftDown; } }

    protected bool _mouseRightDown = false;
    public bool overlayMouseRightDown { get { return _mouseRightDown; } }

    protected void UpdateMouseData(VREvent_Mouse_t mD)
    {
        _mousePos.x = mD.x;
        _mousePos.y = mD.y;
    }
    protected void UpdateMouseData(VREvent_Mouse_t mD, bool state)
    {
        UpdateMouseData(mD);

        switch((EVRMouseButton) mD.button)
        {
            case EVRMouseButton.Left:
                _mouseLeftDown = state;
            break;

            case EVRMouseButton.Right:
                _mouseRightDown = state;
            break;
        }
    }

}