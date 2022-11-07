using UnityEngine;
using System.Collections;

public class ArtificialCamera : MonoBehaviour
{
	void Start()
	{
		Input.gyro.enabled = true;
	}

	void Update()
	{
		Quaternion rotation = new Quaternion();
		Vector3 angles = Input.gyro.attitude.eulerAngles;
		rotation.eulerAngles = new Vector3(-angles.x, -angles.y, angles.z);
		transform.rotation = rotation;
	}
}
