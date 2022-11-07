using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ARSystem
{
	public enum ARObjectHUDButtonAction
	{
		StartBattle = 100,
		MakeShot = 200,
		Explore = 300,
	}

	public class ARObjectHUDButton : MonoBehaviour, IPointerDownHandler
	{
		public event Action<ARObjectHUDButtonAction> OnClick;

		[SerializeField] private ARObjectHUDButtonAction Action;

		public void OnPointerDown(PointerEventData eventData)
		{
			OnClick(Action);
		}
	}
}