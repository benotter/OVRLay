using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unity_Overlay_3D_Object : MonoBehaviour 
{
	public GameObject hmd;
	public GameObject targetObject;
	public Unity_Overlay overlay;

	[Space(10)]

	public MeshRenderer debugRenderer;
	
	[Space(10)]

	public Unity_Overlay_3D_Camera cameraRig;

	private bool debugSet = false;

	void Start() 
	{

	}

	void Update() 
	{
		overlay.transform.position = targetObject.transform.position;
		overlay.transform.LookAt(2 * overlay.transform.position - hmd.transform.position);

		cameraRig.UpdateObject();

		overlay.overlayTexture = cameraRig.outputTex;

		if(!debugSet && cameraRig.outputTex)
		{
			debugRenderer.material.mainTexture = cameraRig.outputTex;
			debugSet = true;
		}

		if(!overlay.overlay.overlayFlag_SideBySide_Parallel)
			overlay.overlay.overlayFlag_SideBySide_Parallel = true;

	}
}
