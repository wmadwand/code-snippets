using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace TutorialConditions
{
	public class ActiveScene : ITutorialCondition
	{
		public string SceneName;

		public bool IsMet(TutorialContext context)
		{
			var name = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
			return name == SceneName;
		}
	}
}
