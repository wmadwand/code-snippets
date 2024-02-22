using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using TutorialActions;

namespace TutorialConditions
{
	public class ModalWindowActive : ITutorialCondition
	{
		public string Name;

		public bool IsMet(TutorialContext context)
		{
			var name = ModalWindow.Active?.name;
			return name == Name;
		}
	}

}
