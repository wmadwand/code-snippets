using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;
using UI;

namespace TutorialActions
{

	[DropdownName("Show hide")]
	public class ShowHideAction : ITutorialAction
	{
        public GameObjectLocator GameObjectLocator;
        public bool Active;

        public IEnumerator Play(TutorialStep tutorial, TutorialContext context)
        {
            var go = GameObjectLocator.Locate();
            if (go)
            {
                go.SetActive(Active);
            }
            yield break;
        }
    }
}
