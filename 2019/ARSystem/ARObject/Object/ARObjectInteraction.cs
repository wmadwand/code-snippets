using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ARSystem
{
	public class ARObjectInteraction : MonoBehaviour, IPointerClickHandler
	{
		public event Action OnTap;

		public void OnPointerClick(PointerEventData eventData)
		{
			OnTap();
		}
	}
}