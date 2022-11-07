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

	[DropdownName("Highlight/UI")]
	public class HighlightAction : ITutorialAction
	{
        const float WaitBeforeRaycastCheckTime = 5f;

        public enum PointerTypes
		{
			Arrow,
			Glow
		}
		public bool ShowHint = true;
		[ShowIf("ShowHint")]
		public Speech Speech;
        public bool OnlyButton = true;
		[ShowIf("OnlyButton")]
		public bool WaitInteractable;
        [ShowIf("OnlyButton")]
		public IHighlightActionObjectLocator Target;
        [HideIf("OnlyButton")]
        public GameObjectLocator GameObjectLocator;
		public ITutorialAction[] CustomActions;
		public bool DoNotShowPointer;

		public bool DisableInputBlocker;

		[HideIf("DoNotShowPointer")]
		public PointerTypes PointerType;
		[HideIf("DoNotShowPointer")]
		public Vector2 PointerOffset;
		[HideIf("DoNotShowPointer")]
		public float PointerAngle;
		[HideIf("DoNotShowPointer")]
		public float PointerScale = 1.0f;

		public bool ShowHighlightBack;
		[ShowIf("ShowHighlightBack")]
		public bool CircleHole;
		[ShowIf("ShowHighlightBack")]
		public bool ManualParameters;
		[ShowIf("ManualParameters")]
		public Vector2 HoleOffset;
		[ShowIf("ManualParameters")]
		public Vector2 HoleSize;

        public IEnumerator Play(TutorialStep tutorial, TutorialContext context)
        {
            GameObject gameObject;
            Button button;
            Component highlitedObject;
            var action = this;
            if (action.OnlyButton)
            {
                var buttonLocator = action.Target;
                var locatedButton = buttonLocator.Locate();
                if (!locatedButton)
                {
                    yield break;
                }
                if (action.WaitInteractable)
                {
					while (!locatedButton.Button.gameObject.activeInHierarchy || !locatedButton.Button.interactable)
					{
                        yield return null;
					} 
                }

                if (!locatedButton.Button.interactable)
                {
                    Debug.LogErrorFormat("Located button is not interactible, skipping action: {0}", locatedButton.Button.name);
                    yield break;
                }
                gameObject = locatedButton.GameObject;
                highlitedObject = locatedButton.Button;
                button = locatedButton.Button;
            }
            else
            {
                gameObject = action.GameObjectLocator.Locate();
                if (!gameObject)
                {
                    Debug.LogErrorFormat("Game object not found, skipping action");
                    yield break;
                }
                highlitedObject = gameObject.transform;
                button = null;
            }
            var blocker = GameObject.Instantiate(Game.Config.Meta.UI.TutorialInputBlocker);
            blocker.GetComponentInChildren<TouchableFilter>().filterTransform = gameObject.transform as RectTransform;
            Canvas blockerCanvas = blocker.GetComponent<Canvas>();
            var highlighter = blocker.GetComponent<TutorialHighlightController>();
            if (action.DisableInputBlocker)
            {
                blockerCanvas.GetComponent<GraphicRaycaster>().enabled = false;
            }

            if (action.ShowHighlightBack)
            {
                highlighter.SetHighlightedObject(highlitedObject, action.CircleHole);
                if (action.ManualParameters)
                {
                    highlighter.Move(action.HoleOffset);
                    highlighter.SetSize(action.HoleSize);
                }
            }

            GameObject pointer = null;
            if (!action.DoNotShowPointer)
            {
                switch (action.PointerType)
                {
                    case HighlightAction.PointerTypes.Arrow:
                    default:
                        pointer = GameObject.Instantiate(Game.Config.Meta.UI.TutorialArrow);
                        foreach (var graphics in pointer.GetComponentsInChildren<MaskableGraphic>())
                        {
                            graphics.maskable = false;
                        }
                        pointer.transform.SetParent(highlitedObject.transform);
                        break;
                    case HighlightAction.PointerTypes.Glow:
                        pointer = GameObject.Instantiate(Game.Config.Meta.UI.TutorialGlow);
                        foreach (var graphics in pointer.GetComponentsInChildren<MaskableGraphic>())
                        {
                            graphics.maskable = false;
                        }
                        pointer.transform.SetParent(highlitedObject.transform);
                        break;
                }
                pointer.GetComponent<Transform>().localPosition = new Vector3(action.PointerOffset.x, action.PointerOffset.y);
                pointer.GetComponent<Transform>().localScale *= action.PointerScale;
                pointer.GetComponent<Transform>().Rotate(0, 0, action.PointerAngle);
                Canvas.ForceUpdateCanvases();
                pointer.transform.SetParent(blocker.transform, true);
                pointer.UpdateSortingLayer("UI");
                pointer.UpdateSortingOrder(blockerCanvas.sortingOrder + 1);
            }

            if (!button)
            {
                button = blocker.GetComponentInChildren<Button>(true);
                button.gameObject.SetActive(true);
            }

            bool hasCustomActions = action.CustomActions != null && action.CustomActions.Length > 0;

            if (hasCustomActions && button)
            {
                for (int i = 0; i < button.onClick.GetPersistentEventCount(); ++i)
                {
                    button.onClick.SetPersistentListenerState(i, UnityEventCallState.Off);
                }
            }

            bool clicked = false;
            UnityAction onClickAction = null;
            if (button)
            {
                onClickAction = () => {
					clicked = true;
				};
                button.onClick.AddListener(onClickAction);
            }

            var parentScrollRect = gameObject.GetComponentInParent<ScrollRect>();
            if (parentScrollRect)
            {
                parentScrollRect.enabled = false;
            }

            HeroSpeechWindow speechObj = null;
            if (action.ShowHint)
            {
                if (action.Speech != null)
                {
                    speechObj = GameObject.Instantiate(Game.Config.Meta.UI.HeroSpeechWindow);
					speechObj.name = string.Format("HighlightActionPlayer-{0}", action.Speech.Message.Value);
                    speechObj.Activate(action.Speech, highlighter.GetSpeechPosition(action.Speech.Anchor), action.ShowHighlightBack);
                }
            }

            //Check button touchable
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.pointerId = -1;
            List<RaycastResult> raycasts = new List<RaycastResult>();
            float checkTime = WaitBeforeRaycastCheckTime;
            bool visible = true;

            while (!clicked && visible && button && button.interactable)
            {
                checkTime -= Time.unscaledDeltaTime;
                if (checkTime <= 0)
                {
                    button.onClick.RemoveListener(onClickAction);
                    button.onClick.AddListener(onClickAction);
                    visible = button.gameObject.activeInHierarchy;
                    checkTime = WaitBeforeRaycastCheckTime;
                    pointerData.Reset();
                    raycasts.Clear();                   
                    pointerData.position = button.transform.position; 
                    EventSystem.current.RaycastAll(pointerData, raycasts);
                    foreach (var r in raycasts)
                    {
                        if (r.gameObject == button.gameObject || r.gameObject == button.targetGraphic?.gameObject)
                        {
                            break;
                        }
                        //If some object raycast before locatedButton it should be a child of button
                        if (r.gameObject.GetComponent<RectTransform>() && !r.gameObject.transform.IsChildOf(button.transform))
                        {
                            Debug.LogError($"Found input blocker {r.gameObject.name}, skipping tutorial {tutorial.Config.Id}", r.gameObject);
                            //visible = false;
                            break;
                        }
                    }
                }
                yield return null;
            }

            if (speechObj)
            {
                speechObj.Close();
            }

            if (parentScrollRect)
            {
                parentScrollRect.enabled = true;
            }

            if (button)
            {
                button.onClick.RemoveListener(onClickAction);
            }

            if (!action.DoNotShowPointer)
            {
                GameObject.Destroy(pointer);
            }
            GameObject.Destroy(blocker);

            if (hasCustomActions)
            {
                if (button)
                {
                    for (int i = 0; i < button.onClick.GetPersistentEventCount(); ++i)
                    {
                        button.onClick.SetPersistentListenerState(i, UnityEventCallState.RuntimeOnly);
                    }

                    foreach (var customAction in action.CustomActions)
                    {
                        yield return customAction.Play(tutorial, context);
                    }
                }
            }
        }
    }
}
