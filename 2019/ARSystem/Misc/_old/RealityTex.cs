using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RealityTex : MonoBehaviour
{
	WebCamTexture webCamTexture;
	bool camAvailable;

	public RawImage renderer;

	// Use this for initialization
	private void Start()
	{		
		webCamTexture = new WebCamTexture(WebCamTexture.devices[0].name, Screen.width, Screen.height);
		renderer.material.mainTexture = webCamTexture;
		webCamTexture.Play();

		camAvailable = true;
	}

	private void Update()
	{
		if (!camAvailable)
		{
			return;
		}

		float ratio = (float)webCamTexture.width / (float)webCamTexture.height;


		float scaleY = webCamTexture.videoVerticallyMirrored ? -1f : 1f;
		transform.localScale = new Vector3(1, scaleY, 1);


		int orient = -webCamTexture.videoRotationAngle;
		renderer.transform.localEulerAngles = new Vector3(0, 0, orient);

	}
}
