using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;

namespace ARSystem
{
	public class ARMode : MonoSingleton<ARMode>, IARController
	{
		#region Variables
		public bool IsInitiated { get; private set; }
		public bool IsActive { get; private set; }

		public GameObject ARCameraGO => _ARCameraGO;

		public ARTransition _ARTransition { get; private set; }

		[SerializeField] private GameObject _ARCameraGO;
		[SerializeField] private Animator _ARTransitionAnimator;
		[SerializeField] private GameObject _ARSceneObjectParent;

		private ARCamera _ARCamera;
		private ARComposer _ARComposer;

		private object[] modulesToCall;
		private Action moduleCallback;
		private int modulesFinishedCount = 0;
		#endregion

		//---------------------------------------------------------

		public void Init(StageType stage, params MapObject[] mapObjects)
		{
			if (IsInitiated)
			{
				Debug.Log("ARMode has been already initiated.");
				return;
			}

			StartCoroutine(InitRoutine(stage, mapObjects));
		}

		public void Run(Action callback = null)
		{
			if (!IsInitiated)
			{
				Debug.Log("ARMode cannot be ran unless it's initiated.");
				return;
			}

			MapController.Instance.SetMapActive(false);

			_ARCamera.Run();

			_ARComposer.ShowObjects(() =>
			{
				this.IsActive = true;
				callback?.Invoke();

				//_ARTransition.Play(false, () => IsActive = !IsActive);
			});
		}

		public void Stop(Action callback = null)
		{
			if (!IsInitiated)
			{
				Debug.Log("ARMode cannot be stopped unless it's initiated.");
				return;
			}

			_ARComposer.HideObjects();

			MapController.Instance.SetMapActive(true);

			_ARCamera.Stop();

			this.IsInitiated = false; //@TODO: cope with it
			this.IsActive = false;

			callback?.Invoke();
		}

		//---------------------------------------------------------

		private void Awake()
		{
			_ARComposer = new ARComposer(_ARSceneObjectParent);
			_ARCamera = new ARCamera();
			_ARTransition = new ARTransition(_ARTransitionAnimator);

			modulesToCall = new object[2] { _ARComposer, _ARCamera/*, _ARTransition */};
			moduleCallback = () => modulesFinishedCount++;
		}

		private IEnumerator InitRoutine(StageType stage, params MapObject[] mapObjects)
		{
			modulesFinishedCount = 0;

			_ARComposer.AddObjects(moduleCallback, mapObjects);
			_ARCamera.Init(stage, moduleCallback);

			yield return new WaitWhile(() => modulesFinishedCount < modulesToCall.Length);

			this.IsInitiated = true;
		}

		private void AddObjects(params MapObject[] mapObjects)
		{
			_ARComposer.AddObjects(null, mapObjects);
		}

		private void RemoveObjects(params MapObject[] mapObjects)
		{
			//_ARComposer.HideObjects(null, mapObjects);
		}

		private void TakeObjects(params MapObject[] mapObjects)
		{
			//ARObject.currentObject[obj].Deactivate();
		}
	}
}