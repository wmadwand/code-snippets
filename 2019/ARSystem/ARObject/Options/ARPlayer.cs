using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARSystem
{
	public class ARPlayer : ARObject
	{
		public override string FilePath => "ARSystem/Player";
		public override string HUDFilePath => "ARSystem/HUD/MonsterHUD";


		protected override void OnTapHandler()
		{
			//Debug.Log("GOTCHA ARMonster!!!");
			//Handheld.Vibrate();
		}

		protected override void OnHUDTapHandler(ARObjectHUDButtonAction type)
		{
			Debug.Log("GOTCHA ARPlayer HUD!!!");
		}
	}
}