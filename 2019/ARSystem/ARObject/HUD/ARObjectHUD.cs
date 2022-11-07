using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARSystem
{
	public class ARObjectHUD : MonoBehaviour
	{
		public event Action<ARObjectHUDButtonAction> OnTap;

		private ARObjectHUDButton[] buttons;

		private void Awake()
		{
			buttons = GetComponentsInChildren<ARObjectHUDButton>();

			foreach (ARObjectHUDButton btn in buttons)
			{
				btn.OnClick += OnClickHandler;
			}
		}

		private void OnClickHandler(ARObjectHUDButtonAction type)
		{
			OnTap?.Invoke(type);
		}
	}
}