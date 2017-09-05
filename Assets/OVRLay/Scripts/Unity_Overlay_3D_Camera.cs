using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unity_Overlay_3D_Camera : MonoBehaviour 
{
	public bool autoUpdate = true;

	[Space(10)]

	public GameObject targetObject;
	
	[Space(10)]
	public Unity_Overlay overlay;
	public MeshRenderer debugRenderer;

	public int textureSize = 200;
	public float focalDistance = 0.5f;

	[Space(10)]

	public Unity_SteamVR_Handler handler;
	public Transform hmd;
	
	[Space(10)]

	public Camera rightEye;
	public Camera leftEye;

	[Space(10)]

	public Vector2 rightScale = Vector2.one;
	public Vector2 rightOffset = Vector2.zero;

	public Vector2 leftScale = Vector2.one;
	public Vector2 leftOffset = Vector2.zero;

	[Space(10)]

	public RenderTexture outputTex;

	private RenderTexture rightTex;
	private RenderTexture leftTex;

	private uint prefWidth = 0;
	private uint prefHeight = 0;

	private bool debugSet = false;

	void Update() 
	{
		if(autoUpdate)
			UpdateObject();
	}

	public void UpdateObject() 
	{
		if(!handler.connectedToSteam)
			return;
		
		if(!overlay.overlay.overlayFlag_SideBySide_Parallel)
			overlay.overlay.overlayFlag_SideBySide_Parallel = true;

		if(!debugSet && outputTex)
		{
			debugRenderer.material.mainTexture = outputTex;
			debugSet = true;
		}

		var distO = focalDistance / Vector3.Distance(targetObject.transform.position, hmd.transform.position);

		rightEye.transform.localPosition = handler.poseHandler.GetRightEyeTransform() * distO;
		leftEye.transform.localPosition = handler.poseHandler.GetLeftEyeTransform() * distO;

		UpdateTextures();
		UpdatePosition();
	}

	public void UpdatePosition() 
	{
		var head = (hmd.transform.position - targetObject.transform.position).normalized;
		var newPos = head * focalDistance;	

		// transform.position = Vector3.MoveTowards(targetObject.transform.position, hmd.transform.position, focalDistance);
		// transform.position = hmd.transform.position;
		transform.position = newPos + targetObject.transform.position;

		transform.LookAt(targetObject.transform);

		var hmdR = hmd.transform.eulerAngles;
		transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, hmdR.z);

		overlay.transform.position = targetObject.transform.position;
		var lookP = 2 * overlay.transform.position - hmd.transform.position;
		overlay.transform.LookAt(lookP);

		var oEA = overlay.transform.eulerAngles;
		oEA.z = hmdR.z;
		overlay.transform.eulerAngles = oEA;
	}

	public void UpdateTextures() 
	{
		if(!outputTex)
			CreateTextures();

		if(rightEye.targetTexture != outputTex)
			rightEye.targetTexture = outputTex;
		
		if(leftEye.targetTexture!= outputTex)
			leftEye.targetTexture = outputTex;

		rightEye.Render();
		leftEye.Render();

		overlay.overlayTexture = outputTex;
	}

	public void CreateTextures() 
	{
		if(!handler.connectedToSteam)
			return;

		outputTex = new RenderTexture(textureSize * 2, textureSize, 32);
	}
}
