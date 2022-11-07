using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ARSystem;

[RequireComponent(typeof(Button))]
public class StopBattleButton : MonoBehaviour
{
	private void Awake()
	{
		GetComponent<Button>().onClick.AddListener(Broadcast);
	}

	private void Broadcast()
	{
		ARBattle.Instance.Stop();
		//StartCoroutine(BroadcastRoutine());
	}

	private IEnumerator BroadcastRoutine()
	{

		ARBattle.Instance.Stop(() => MapController.Instance.ChangeHUDButtons());

		yield return null;/* new WaitWhile(() => !ARMode.Instance.IsInitiated);*/

	}
}