using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDepthControll : MonoBehaviour
{

	public Material depthMat;
	
	// Use this for initialization
	void Start ()
	{
		var cam = GetComponent<Camera>();
		cam.depthTextureMode = DepthTextureMode.Depth;
	}

	private void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		Graphics.Blit(src,dest, depthMat);
	}
}
