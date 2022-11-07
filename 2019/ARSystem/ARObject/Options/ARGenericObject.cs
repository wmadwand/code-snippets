using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARSystem
{
	public class ARGenericObject : ARObject
	{
		public override string FilePath => "ARSystem/Monster01";
		public override string HUDFilePath => "ARSystem/HUD/MonsterHUD";


		protected override void OnTapHandler()
		{
			Debug.Log("GOTCHA ARGenericObject!!!");
		}

		protected override void OnHUDTapHandler(ARObjectHUDButtonAction type)
		{
			Debug.Log("GOTCHA ARGenericObject HUD!!!");
		}
	}
}