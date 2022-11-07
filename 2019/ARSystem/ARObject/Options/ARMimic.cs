using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARSystem
{
	public class ARMimic : ARObject
	{
		public override string FilePath => "ARSystem/SceneObjects/Specific/Mimic";
		public override string HUDFilePath => "ARSystem/HUD/MonsterHUD";


		protected override void OnTapHandler()
		{
			Debug.Log("GOTCHA ARMimic!!!");
		}

		protected override void OnHUDTapHandler(ARObjectHUDButtonAction type)
		{
			Debug.Log("GOTCHA ARMimic HUD!!!");
		}
	} 
}