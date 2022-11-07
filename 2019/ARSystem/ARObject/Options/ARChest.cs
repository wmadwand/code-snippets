using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARSystem
{
	public class ARChest : ARObject
	{
		public override string FilePath => "ARSystem/SceneObjects/Specific/Chest";
		public override string HUDFilePath => "ARSystem/HUD/MonsterHUD";


		protected override void OnTapHandler()
		{
			Debug.Log("GOTCHA ARChest!!!");
		}

		protected override void OnHUDTapHandler(ARObjectHUDButtonAction type)
		{
			Debug.Log("GOTCHA ARChest HUD!!!");
		}
	}
}