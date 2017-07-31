using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Valve.VR;

public class Unity_Overlay : MonoBehaviour 
{
	public enum OverlayTrackedDevice 
	{
		None,
		HMD,
		RightHand,
		LeftHand,
		CustomIndex
	}

	[Space(10)]
	public string overlayName = "Unity Overlay";
	public string overlayKey = "unity_overlay";

	public bool isDashboardOverlay = false;
	public bool onlyShowInDashboard = false;

	[Space(10)]

	public bool simulateInEditor = false;

	[Space(10)]

	public Texture overlayTexture;
	public Camera cameraForTexture;
	public Texture thumbNailTexture;

	[Space(10)]
	public bool highQualityRenderTex = true;
	public int renderTexWidthOverride = 0;
	public int renderTexHeightOverride = 0;
	

	[Space(10)]

	public bool autoUpdateOverlay = true;

	[Space(10)]

	public bool isVisible = true;
	public bool highQuality = false;
	public Color colorTint = Color.white;
	[Range(0f, 1f)]
	public float opacity = 1.0f;
	public float widthInMeters = 1.0f;

	[Space(10)]
	public OverlayTrackedDevice deviceToTrack = OverlayTrackedDevice.None;
	public uint customDeviceIndex = 0;
	
	[Space(10)]
	public bool enableSimulatedMouse = false;
	public Vector2 mouseScale = new Vector2(1f, 1f);

	[Space(10)]
	public bool simulateUnityMouseInput = false;
	public GraphicRaycaster canvasGraphicsCaster;


	protected OVR_Handler ovrHandler = OVR_Handler.instance;
	protected OVR_Overlay overlay = new OVR_Overlay();
	
	protected Unity_Overlay_Opts opts = new Unity_Overlay_Opts();

	protected RenderTexture cameraTexture;

	protected Texture _mainTex;

	protected VRTextureBounds_t textureBounds = new VRTextureBounds_t();
	protected HmdVector2_t mouseScale_t = new HmdVector2_t();
	protected OVR_Utils.RigidTransform matrixConverter;

	private HashSet<Selectable> enterTargets = new HashSet<Selectable>();
	private HashSet<Selectable> downTargets = new HashSet<Selectable>();

	public bool mouseDown = false;
	public bool mouseDragging = false;
	public float mouseDownTime = 0f;

	public Vector2 mousePos = new Vector2();

	protected Unity_Overlay_UI_Handler uiHandler = new Unity_Overlay_UI_Handler();

	private float reverseAspect = 0f;

	private bool useChapColor = false;
	private Color lastColor = Color.black;

	public bool lastVisible = false;


	private bool isDashboardOpen = true;

	// Some methods to make UI stuff easier
	public void ToggleEnable()
	{
		gameObject.SetActive(!gameObject.activeSelf);
	}

	public void SetOpacity(float o)
	{
		opacity = o;
	}

	public void SetScale(float s)
	{
		widthInMeters = s;
	}

	public void SetToChaperoneColor(bool setToChapColor)
	{
		useChapColor = setToChapColor;
	}
	public void SetOnlyShowInDashboard(bool enble)
	{
		onlyShowInDashboard = enble;
	}

	// Event Callbac...Err... Delegates.
	void OnVisChange(bool visible)
	{
		isVisible = visible;
		Debug.Log("Is Visible: " + visible);
	}

	void OnDashBoardChange(bool open)
	{
		isDashboardOpen = open;
		Debug.Log("Dashboard Open: " + open);
	}

	void Start () 
	{
		matrixConverter = new OVR_Utils.RigidTransform(transform);

		if(cameraForTexture != null)
		{
			int width = renderTexWidthOverride != 0 ? renderTexWidthOverride : (int) (cameraForTexture.pixelWidth);
			int height = renderTexHeightOverride != 0 ? renderTexHeightOverride : (int) (cameraForTexture.pixelHeight);

			cameraForTexture.enabled = false;
			cameraTexture = new RenderTexture(width, height, 24);

			if(highQualityRenderTex)
			{
				cameraTexture.antiAliasing = 8;
				cameraTexture.filterMode = FilterMode.Trilinear;
			}

			cameraForTexture.targetTexture = cameraTexture;
			overlayTexture = cameraTexture;
		}

		overlay.overlayTextureType = SystemInfo.graphicsDeviceVersion.StartsWith("OpenGL") ? ETextureType.OpenGL : ETextureType.DirectX;
		
		overlay.overlayKey = overlayKey;
		overlay.overlayName = overlayName;

		if(isDashboardOverlay)
			overlay.overlayIsDashboard = true;

		textureBounds.uMin = 0;
		textureBounds.vMin = 1;

		textureBounds.uMax = 1;
		textureBounds.vMax = 0;

		// Testing out some event based stuff
		overlay.onVisibilityChange += OnVisChange;
		ovrHandler.onDashboardChange += OnDashBoardChange;
	}

	void OnDestroy()
	{
		overlay.DestroyOverlay();
	}

	void OnEnable()
	{
		if(lastVisible)
		{
			isVisible = true;
			overlay.ShowOverlay();
		}
			
	}

	void OnDisable()
	{
		lastVisible = isVisible;
		overlay.HideOverlay();
	}
	
	void Update() 
	{
		if(autoUpdateOverlay)
			UpdateOverlay();
	}

	public void UpdateOverlay()
	{
		if(!ovrHandler.OpenVRConnected)
			return;
		
		if(!overlay.created)
		{
			if(!overlay.CreateOverlay())
			{
				Debug.Log("Failed to create overlay!");
				return;
			}

			isDashboardOpen = ovrHandler.Overlay.IsDashboardVisible();
		}

		if(onlyShowInDashboard && !isDashboardOpen)
		{
			if(isVisible)
				isVisible = false;
		}
		else if (onlyShowInDashboard && isDashboardOpen)
		{
			if(!isVisible)
				isVisible = true;
		} 

		UpdateOpts();

		if(!enabled)
			return;

		DrawOverlayThumbnail();

		if(!isVisible)
			return;		

		if(useChapColor)
		{
			Color chapCol = GetChaperoneColor();
			if(chapCol != lastColor)
			{
				overlay.overlayColor = chapCol;
				lastColor = chapCol;
			}
		}
		else if (lastColor != Color.black)
		{
			overlay.overlayColor = colorTint;
			lastColor = Color.black;
		}

		UpdateTexture();

		if(enableSimulatedMouse)
			UpdateMouse();

		if(enableSimulatedMouse && simulateUnityMouseInput)
				UpdateUnityMouseSim();
	}

	void UpdateMouse()
	{
		if(mouseDown)
			mouseDownTime += Time.deltaTime;

		if(mouseDown && mouseDownTime > 0f)
			mouseDragging = true;

		if(overlay.overlayMouseLeftDown && !mouseDown)
			mouseDown = true;

		if(mouseDown && !overlay.overlayMouseLeftDown)
		{
			mouseDown = false;
			mouseDragging = false;
			mouseDownTime = 0f;
		}

		if(overlayTexture)
		{
			var mPos = overlay.overlayMousePosition;
			if(mPos.x < 0f || mPos.x > 1f || mPos.y < 0f || mPos.y > 1f)
				return;

			mousePos.x = overlayTexture.width * mPos.x;
			mousePos.y = overlayTexture.height * ( 1f - (mPos.y / reverseAspect) );
		}
	}

	void UpdateUnityMouseSim()
	{
		var pd = uiHandler.pD;

		pd.position = mousePos;
		pd.button = PointerEventData.InputButton.Left;

		if(mouseDown && !mouseDragging)
		{
			pd.Reset();
			pd.clickCount = 1;
		}
		else if(mouseDown && mouseDragging)
		{
			pd.clickCount = 0;
			pd.clickTime += mouseDownTime;
			pd.dragging = true;
		}

		var nTargs = uiHandler.GetUITargets(canvasGraphicsCaster, pd);

		uiHandler.EnterTargets(nTargs);

		foreach(Selectable ub in nTargs)
			if(enterTargets.Contains(ub))
				enterTargets.Remove(ub);

		uiHandler.ExitTargets(enterTargets);
		enterTargets = nTargs;

		if(mouseDown)
		{
			if(!mouseDragging)
			{
				foreach(Selectable sel in nTargs)
					downTargets.Add(sel);

				uiHandler.SubmitTargets(downTargets);
				uiHandler.StartDragTargets(downTargets);
				uiHandler.DownTargets(downTargets);
			}
			else
			{
				uiHandler.MoveTargets(downTargets);
				uiHandler.DragTargets(downTargets);
				uiHandler.DownTargets(downTargets);
			}
		}
		else
		{
			uiHandler.UpTargets(downTargets);
			uiHandler.EndDragTargets(downTargets);
			uiHandler.DropTargets(downTargets);

			downTargets.Clear();
		}
	}

	void UpdateTexture()
	{
		if(!overlay.created)
			return;

		if(cameraForTexture)
			cameraForTexture.Render();

		if(!overlayTexture)
			return;

		DrawOverlayThumbnail();
		
		overlay.overlayWidthInMeters = widthInMeters;
		overlay.overlayTextureBounds = textureBounds;
		overlay.overlayMouseScale = mouseScale_t;

		reverseAspect = (float) overlayTexture.height / (float) overlayTexture.width;
		mouseScale.y = mouseScale_t.v1 = reverseAspect;

		overlay.overlayTexture = overlayTexture;

	}

	void DrawOverlayThumbnail()
	{
		if(isDashboardOverlay && thumbNailTexture)
		{
			overlay.overlayThumbnailTextureBounds = textureBounds;
			overlay.overlayThumbnailTexture = thumbNailTexture;
		}
	}

	void UpdateOpts()
	{
		if(opts.pos != transform.position || opts.rot != transform.rotation)
		{
			matrixConverter.pos = transform.position;
			matrixConverter.rot = transform.rotation;

			overlay.overlayTransform = matrixConverter.ToHmdMatrix34();

			opts.pos = transform.position;
			opts.rot = transform.rotation;
		}

		if( opts.isVisible != isVisible ) 
		{
			overlay.overlayVisible = isVisible;

			opts.isVisible = isVisible;
		}

		if( opts.highQuality != highQuality ) 
		{
			overlay.overlayHighQuality = highQuality;

			opts.highQuality = highQuality;
		}

		if( opts.colorTint != colorTint ) 
		{
			overlay.overlayColor = colorTint;

			opts.colorTint = colorTint;
		}

		if( opts.opacity != opacity ) 
		{
			overlay.overlayAlpha = opacity;

			opts.opacity = opacity;
		}

		if( opts.widthInMeters != widthInMeters ) 
		{
			overlay.overlayWidthInMeters = widthInMeters;

			opts.widthInMeters = widthInMeters;
		}

		if( opts.deviceToTrack != deviceToTrack ) 
		{
			if(deviceToTrack == OverlayTrackedDevice.None)
				overlay.overlayTransformType = VROverlayTransformType.VROverlayTransform_Absolute;
			else
			{
				uint index = 0;
				switch(deviceToTrack)
				{
					case OverlayTrackedDevice.HMD:
						index = ovrHandler.poseHandler.hmdIndex;
					break;

					case OverlayTrackedDevice.RightHand:
						index = ovrHandler.poseHandler.rightIndex;
					break;

					case OverlayTrackedDevice.LeftHand:
						index = ovrHandler.poseHandler.leftIndex;
					break;

					case OverlayTrackedDevice.CustomIndex:
						index = customDeviceIndex;
					break;
				}

				overlay.overlayTransformType = VROverlayTransformType.VROverlayTransform_TrackedDeviceRelative;
				overlay.overlayTransformTrackedDeviceRelativeIndex = index;
			}

			opts.deviceToTrack = deviceToTrack;
		}

		if( opts.customDeviceIndex != customDeviceIndex ) 
		{	
			overlay.overlayTransformTrackedDeviceRelativeIndex = customDeviceIndex;
			opts.customDeviceIndex = customDeviceIndex;
		}

		if( opts.enableSimulatedMouse != enableSimulatedMouse ) 
		{
			overlay.overlayInputMethod = 
				enableSimulatedMouse ? VROverlayInputMethod.Mouse : VROverlayInputMethod.None;

			opts.enableSimulatedMouse = enableSimulatedMouse;
		}

		if( opts.mouseScale != mouseScale ) 
		{
			mouseScale_t.v0 = mouseScale.x;
			mouseScale_t.v1 = mouseScale.y;

			overlay.overlayMouseScale = mouseScale_t;
			opts.mouseScale = mouseScale;
		}
	}

	public Color GetChaperoneColor()
	{
		Color ret = new Color(1,1,1,1);

		if(ovrHandler.Settings == null)
			return ret;

		var collSec = OpenVR.k_pch_CollisionBounds_Section;
		var error = EVRSettingsError.None;

		int r = 255, g = 255, b = 255, a = 255;
		r = ovrHandler.Settings.GetInt32(collSec, OpenVR.k_pch_CollisionBounds_ColorGammaR_Int32, ref error);
		g = ovrHandler.Settings.GetInt32(collSec, OpenVR.k_pch_CollisionBounds_ColorGammaG_Int32, ref error);
		b = ovrHandler.Settings.GetInt32(collSec, OpenVR.k_pch_CollisionBounds_ColorGammaB_Int32, ref error);
		a = ovrHandler.Settings.GetInt32(collSec, OpenVR.k_pch_CollisionBounds_ColorGammaA_Int32, ref error);

		ret.r = (float) r / 255;
		ret.g = (float) g / 255;
		ret.b = (float) b / 255;
		ret.a = (float) a / 255;

		return ret;
	}
}

public struct Unity_Overlay_Opts 
{
	public Vector3 pos;
	public Quaternion rot;
	public bool isVisible;
	public bool highQuality;
	public Color colorTint;
	public float opacity;
	public float widthInMeters;

	public Unity_Overlay.OverlayTrackedDevice deviceToTrack;
	public uint customDeviceIndex;

	public bool enableSimulatedMouse;
	public Vector2 mouseScale;
	
}

public class Unity_Overlay_UI_Handler 
{
	public Camera cam;
	public PointerEventData pD = new PointerEventData(EventSystem.current);
	public AxisEventData aD = new AxisEventData(EventSystem.current);

	public HashSet<Selectable> GetUITargets(GraphicRaycaster gRay, PointerEventData pD)
	{
		if(cam != gRay.eventCamera)
			cam = gRay.eventCamera;

		aD.Reset();
		aD.moveVector = (this.pD.position - pD.position);

		float x1 = this.pD.position.x,
			  x2 = pD.position.x,
			  y1 = this.pD.position.y,
			  y2 = pD.position.y;

		float xDiff = x1 - x2;
		float yDiff = y1 - y2;

		MoveDirection dir = MoveDirection.None;

		if(xDiff > yDiff)
			if(xDiff > 0f)
				dir = MoveDirection.Right;
			else if(xDiff < 0f)
				dir = MoveDirection.Left;
		else if (yDiff > xDiff)
			if(yDiff > 0f)
				dir = MoveDirection.Up;
			else if(yDiff < 0f)
				dir = MoveDirection.Down;

		aD.moveDir = dir;

		var ray = cam.ScreenPointToRay(pD.position);
		Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);

		List<RaycastResult> hits = new List<RaycastResult>();
		HashSet<Selectable> uibT = new HashSet<Selectable>();

		gRay.Raycast(pD, hits);

		if(hits.Count > 0)
			pD.pointerCurrentRaycast = pD.pointerPressRaycast = hits[0];

		for(int i = 0; i < hits.Count; i++)
		{
			var go = hits[i].gameObject;
			Selectable u = GOGetter(go);
			
			if(u)
				uibT.Add(u);
		}

		this.pD = pD;

		return uibT;
	}
	
	public Selectable GOGetter(GameObject go, bool tryPar = false)
	{
		Selectable sel = go.GetComponentInParent<Selectable>();

		return sel;
	}

	public void EnterTargets(HashSet<Selectable> t)
	{
		foreach(Selectable b in t)
			ExecuteEvents.Execute(b.gameObject, pD, ExecuteEvents.pointerEnterHandler);
	}

	public void ExitTargets(HashSet<Selectable> t)
	{
		foreach(Selectable b in t)
			ExecuteEvents.Execute(b.gameObject, pD, ExecuteEvents.pointerExitHandler);
	}

	public void DownTargets(HashSet<Selectable> t)
	{
		foreach(Selectable b in t) 
			ExecuteEvents.Execute(b.gameObject, pD, ExecuteEvents.pointerDownHandler);
	}

	public void UpTargets(HashSet<Selectable> t)
	{
		foreach(Selectable b in t)
			ExecuteEvents.Execute(b.gameObject, pD, ExecuteEvents.pointerUpHandler);
	}

	public void SubmitTargets(HashSet<Selectable> t)
	{
		foreach(Selectable b in t)
			ExecuteEvents.Execute(b.gameObject, pD, ExecuteEvents.submitHandler);
	}

	public void StartDragTargets(HashSet<Selectable> t)
	{
		foreach(Selectable b in t)
			ExecuteEvents.Execute(b.gameObject, pD, ExecuteEvents.beginDragHandler);
	}

	public void DragTargets(HashSet<Selectable> t)
	{
		foreach(Selectable b in t)
			ExecuteEvents.Execute(b.gameObject, pD, ExecuteEvents.dragHandler);
	}

	public void MoveTargets(HashSet<Selectable> t)
	{
		foreach(Selectable b in t)
			ExecuteEvents.Execute(b.gameObject, aD, ExecuteEvents.moveHandler);
	}

	public void EndDragTargets(HashSet<Selectable> t)
	{
		foreach(Selectable b in t)
			ExecuteEvents.Execute(b.gameObject, pD, ExecuteEvents.endDragHandler);
	}

	public void DropTargets(HashSet<Selectable> t)
	{
		foreach(Selectable b in t)
			ExecuteEvents.Execute(b.gameObject, pD, ExecuteEvents.dropHandler);
	}


}
