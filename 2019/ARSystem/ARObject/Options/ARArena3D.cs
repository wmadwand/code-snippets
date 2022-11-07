using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARSystem
{
	public class ARArena3D : ARObject
	{
		public override string FilePath => "ARSystem/SceneObjects/Generic/ARArena3D";
		public override string HUDFilePath => "";

		protected override void OnTapHandler() { }

		protected override void OnHUDTapHandler(ARObjectHUDButtonAction type) { }
	}
}