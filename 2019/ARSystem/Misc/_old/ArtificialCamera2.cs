using UnityEngine;
using System.Collections;

public class ArtificialCamera2 : MonoBehaviour
{
	void Start()
	{
		Input.gyro.enabled = true;
	}

	void Update()
	{
		Quaternion cameraRotation = new Quaternion(Input.gyro.attitude.x, Input.gyro.attitude.y, -Input.gyro.attitude.z, -Input.gyro.attitude.w);
		this.transform.localRotation = cameraRotation;
	}
}
