using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARSystem
{
	public class ARArena : ARObject
	{
		public override string FilePath => "ARSystem/SceneObjects/Generic/ARArena";
		public override string HUDFilePath => "";

		protected override void OnTapHandler() { }

		protected override void OnHUDTapHandler(ARObjectHUDButtonAction type) { }
	}
}