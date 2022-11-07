using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ARSystem;

[RequireComponent(typeof(Button))]
public class ActivateAR : MonoBehaviour
{
	private void Awake()
	{
		GetComponent<Button>().onClick.AddListener(Broadcast);
	}

	private void Broadcast()
	{
		StartCoroutine(BroadcastRoutine());
	}

	private IEnumerator BroadcastRoutine()
	{
		if (!ARMode.Instance.IsInitiated)
		{
			ARMode.Instance._ARTransition.Play(true, () => //@TODO: must be driven by Global Fader/Transition class
			{
				ARMode.Instance.Init(StageType.ClearAR, MapController.Instance.MapObjectsInRange);
			});

			yield return new WaitUntil(() => ARMode.Instance.IsInitiated);

			ARMode.Instance.Run(() => ARMode.Instance._ARTransition.Play(false));
		}
		else
		{
			ARMode.Instance._ARTransition.Play(true, () =>
			{
				ARMode.Instance.Stop(() => ARMode.Instance._ARTransition.Play(false));
			});
		}
	}
}