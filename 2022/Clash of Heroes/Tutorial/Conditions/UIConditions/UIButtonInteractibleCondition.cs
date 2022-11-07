using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using TutorialActions;

namespace TutorialConditions
{
	public class UIButtonInteractibleCondition : ITutorialCondition
	{
		public UIButtonLocator ButtonLocator;

		public bool IsMet(TutorialContext context)
		{
			var button = ButtonLocator.Locate();
			if (button == null)
			{
				return false;
			}
			return button.Button.interactable;
		}
	}
}
