using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

namespace ARSystem
{
	public abstract class ARObject : IARObject
	{
		#region Variables
		public bool IsInitiated { get; private set; }
		public bool IsActive { get; private set; }

		public abstract string FilePath { get; } //@TODO: move to the global file with paths
		public abstract string HUDFilePath { get; }

		protected ARObjectAnimation animation;
		protected bool isAlive; //@TODO: move it to PlayerHealth or Player

		private GameObject view;
		private GameObject viewParent;
		private Vector3 viewParentPosition;

		private GameObject HUD;
		private ARObjectHUD HUDInteraction;

		private ARObjectInteraction interaction;
		#endregion

		//---------------------------------------------------------

		public ARObject()
		{
			IsInitiated = false;
			IsActive = false;
		}

		public void Init(GameObject parent, Vector3 parentPosition)
		{
			ARMode.Instance.StartCoroutine(InitRoutine(parent, parentPosition));
		}

		public void SetActive(bool flag)
		{
			viewParent?.SetActive(flag); //@TODO: return it back to the pool
			HUD?.SetActive(false);

			IsActive = !IsActive;

			isAlive = true;
		}

		public void AttachToScene(Vector3 point) { }

		public void AttachToCamera() { }

		//---------------------------------------------------------

		protected abstract void OnTapHandler();

		protected abstract void OnHUDTapHandler(ARObjectHUDButtonAction buttonType);

		protected void ShowHUD()
		{
			HUD.SetActive(!HUD.activeSelf);
		}

		//---------------------------------------------------------

		private IEnumerator InitRoutine(GameObject parent, Vector3 parentPosition)
		{
			if (!view && !viewParent)
			{
				viewParentPosition = parentPosition;
				viewParent = UnityEngine.Object.Instantiate(parent, viewParentPosition, Quaternion.identity);
				viewParent.SetActive(false);

				ResourceRequest viewResRequest = Resources.LoadAsync<GameObject>(FilePath);
				ResourceRequest HUDResRequest = Resources.LoadAsync<GameObject>(HUDFilePath);

				yield return new WaitUntil(() => viewResRequest.isDone && HUDResRequest.isDone);

				if (viewResRequest.asset)
				{
					view = UnityEngine.Object.Instantiate((GameObject)viewResRequest.asset);
					view.transform.SetParent(viewParent.transform.Find("View"));
					view.transform.localPosition = Vector3.zero;

					interaction = viewParent.GetComponentInChildren<ARObjectInteraction>();
					interaction.OnTap += OnTapHandler;

					animation = new ARObjectAnimation(viewParent.GetComponentInChildren<Animator>());
				}
				else
				{
					Debug.Log($"File {FilePath} not found");
				}

				if (HUDResRequest.asset)
				{
					HUD = UnityEngine.Object.Instantiate((GameObject)HUDResRequest.asset);
					HUD.transform.SetParent(viewParent.transform.Find("HUD"));
					HUD.transform.localPosition = Vector3.zero;
					HUD.SetActive(false);

					HUDInteraction = HUD.GetComponent<ARObjectHUD>();
					HUDInteraction.OnTap += OnHUDTapHandler;
				}
				else
				{
					Debug.Log($"File {HUDFilePath} not found");
				}
			}

			IsInitiated = true;
		}

		private void InitView()
		{
			//viewParentPosition = parentPosition;
			//viewParent = UnityEngine.Object.Instantiate(parent, viewParentPosition, Quaternion.identity);
			//viewParent.SetActive(false);
		}

		private void InitViewParent()
		{

		}

		private void InitHUD()
		{
			//HUD = UnityEngine.Object.Instantiate(parent, viewParentPosition, Quaternion.identity);
			//viewParent.SetActive(false);
		}

		private void InitAnimation()
		{

		}

		private void Destroy()
		{
			interaction.OnTap -= OnTapHandler;
			HUDInteraction.OnTap -= OnHUDTapHandler;
		}
	}
}