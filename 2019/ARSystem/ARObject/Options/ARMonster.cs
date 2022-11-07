using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ARSystem
{
	public class ARMonster : ARObject
	{
		public override string FilePath => "ARSystem/SceneObjects/Specific/Monster01";
		public override string HUDFilePath => "ARSystem/HUD/MonsterHUD";

		protected override void OnTapHandler()
		{
			if (isAlive)
			{
				Debug.Log("GOTCHA ARMonster!!!");

				ShowHUD();

				Handheld.Vibrate();
			}
			else
			{
				BringBackToLife();
			}
		}

		protected override void OnHUDTapHandler(ARObjectHUDButtonAction buttonAction)
		{
			switch (buttonAction)
			{
				case ARObjectHUDButtonAction.StartBattle:
					{
						Debug.Log("Start battle!");
						StartBattle();
					}
					break;
				case ARObjectHUDButtonAction.MakeShot:
					{
						Debug.Log("MakeShot!");
						Kill(); ShowHUD();
					}
					break;
				default:
					Debug.Log("Action not found!");
					break;
			}
		}

		private void StartBattle()
		{
			ARMode.Instance.StartCoroutine(StartBattleRoutine());
		}

		private IEnumerator StartBattleRoutine()
		{
			GameEntity _gameEntity = new GameEntity { };
			MapObject mapObj = new MapObject { gameEntity = null, ARObject = this, position = new Vector3(0, -1, 5) };

			ARBattle.Instance.Init(StageType.Arena3D, mapObj);

			yield return new WaitUntil(() => ARBattle.Instance.IsInitiated);

			ARBattle.Instance.Run(() => MapController.Instance.ChangeHUDButtons());
		}

		private void Kill()
		{
			isAlive = false;
			animation.Play(ARObjectAnimation.DEATH/*, ()=> IsAlive = false*/);
		}

		private void BringBackToLife()
		{
			isAlive = true;
			animation.Play(ARObjectAnimation.RECOVER/*, () => IsAlive = true*/);

		}
	}
}