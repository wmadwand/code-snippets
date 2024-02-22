using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using TutorialActions;

namespace TutorialConditions
{
	public class UIObjectActive : ITutorialCondition
	{
		public GameObjectLocator Locator;

		public bool IsMet(TutorialContext context)
		{
			var obj = Locator.Locate(true);
			if (obj == null)
			{
				return false;
			}
			return obj.activeInHierarchy;
		}
	}

}
