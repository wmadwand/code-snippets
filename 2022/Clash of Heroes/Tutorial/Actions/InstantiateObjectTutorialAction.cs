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

	[DropdownName("Instantiate Object")]
	public class InstantiateObjectAction : ITutorialAction
	{
        public GameObjectLocator GameObjectLocator;
        public GameObject Object;

        public IEnumerator Play(TutorialStep tutorial, TutorialContext context)
        {
            var go = GameObjectLocator.Locate();
            if (go)
            {
                GameObject.Instantiate(Object, go.transform);
            }
            yield break;
        }
    }
}
