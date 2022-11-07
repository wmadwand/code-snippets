using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DTools;
using UnityEngine.UI;
using Vuforia;
using System;

namespace ARSystem
{
	public enum CameraType
	{
		CameraARBack = 0,
		CameraARFront = 10,
		Camera3D = 20,
		CameraMain = 30
	}

	public class ARCamera : IARController
	{
		#region Variables
		public bool IsInitiated { get; private set; }
		public bool IsActive { get; private set; }

		private CameraType currentCameraType;

		private Dictionary<CameraType, GameObject> cameras;

		private VuforiaBehaviour sceneCamera;

		private const string CameraARFilePath = "ARSystem/Cameras/ARCamera";
		private const string Camera3DFilePath = "ARSystem/Cameras/3DCamera";
		#endregion

		//---------------------------------------------------------

		public ARCamera()
		{
			cameras = new Dictionary<CameraType, GameObject>();
		}

		public void Init(StageType stage, Action callback)
		{
			if (IsInitiated)
			{
				Debug.Log("ARCamera has been already initiated.");
				return;
			}

			ARMode.Instance.StartCoroutine(InitRoutine(stage, callback));
		}

		public void Run(Action callback = null)
		{
			if (!IsInitiated)
			{
				Debug.Log("ARCamera cannot be ran unless it's initiated.");
				return;
			}

			Activate();

			this.IsActive = true;
		}

		public void Stop(Action callback = null)
		{
			if (!IsInitiated)
			{
				Debug.Log("ARCamera cannot be stopped unless it's initiated.");
				return;
			}

			Deactivate();

			this.IsInitiated = false;
			this.IsActive = false;
		}

		public void ChangeWorldCenterMode()
		{
			//sceneCamera. .SetWorldCenterMode(VuforiaARController.WorldCenterMode.SPECIFIC_TARGET);
		}

		//---------------------------------------------------------

		private IEnumerator InitRoutine(StageType stageType, Action callback)
		{
			CameraType cameraType = CameraType.CameraARBack;
			string filePath = "";

			switch (stageType)
			{
				case StageType.Arena3D:
					{
						cameraType = CameraType.Camera3D;
						filePath = Camera3DFilePath;
					}
					break;
				case StageType.ClearAR:
				case StageType.ArenaAR:
				default:
					{
						cameraType = CameraType.CameraARBack;
						filePath = CameraARFilePath;
					}
					break;
			}

			if (!cameras.ContainsKey(CameraType.CameraMain))
			{
				cameras[CameraType.CameraMain] = GameObject.FindGameObjectWithTag("MainCamera");
			}

			if (!cameras.ContainsKey(CameraType.CameraARBack))
			{
				cameras[CameraType.CameraARBack] = ARMode.Instance.ARCameraGO; /*GameObject.FindGameObjectWithTag("ARCamera");*/
			}

			if (cameras.ContainsKey(cameraType))
			{
				currentCameraType = cameraType;
				this.IsInitiated = true;
				callback();

				yield break;
			}

			ResourceRequest cameraResRequest = Resources.LoadAsync<GameObject>(filePath);

			yield return new WaitUntil(() => cameraResRequest.isDone);

			if (cameraResRequest.asset)
			{
				cameras[cameraType] = UnityEngine.Object.Instantiate((GameObject)cameraResRequest.asset);
				cameras[cameraType].transform.localPosition = Vector3.zero;
				cameras[cameraType].transform.SetAsFirstSibling();
				cameras[cameraType].SetActive(false);
			}

			currentCameraType = cameraType;
			this.IsInitiated = true;

			callback();
		}

		private void GetCameraObject()
		{

		}

		private bool IsAvailable()
		{
			return WebCamTexture.devices.Length > 0;
		}

		private void Activate()
		{
			cameras[currentCameraType].SetActive(true);

			if (currentCameraType == CameraType.CameraARBack || currentCameraType == CameraType.CameraARFront)
			{
				CameraDevice.Instance.Stop();
				CameraDevice.Instance.Deinit();

				CameraDevice.Instance.Init(CameraDevice.CameraDirection.CAMERA_DEFAULT);
				CameraDevice.Instance.Start();
			}

			foreach (var camera in cameras)
			{
				if (camera.Key == currentCameraType)
				{
					continue;
				}

				camera.Value.SetActive(false);
			}
		}

		private void Deactivate()
		{
			if (currentCameraType == CameraType.CameraARBack || currentCameraType == CameraType.CameraARFront)
			{
				CameraDevice.Instance.Stop();
				CameraDevice.Instance.Deinit();
			}

			cameras[currentCameraType].SetActive(false);

			cameras[CameraType.CameraMain].SetActive(true);
		}

		private void SwitchDeviceCamera()
		{

		}
	}
}