using UnityEngine;

namespace MinTrans
{
	public class MapComponentItem : MapComponent
	{
		public MapComponentItem(string name, GameObject[] gameObjects) : base(name, gameObjects) { }

		public override void SetActive(bool value)
		{
			foreach (GameObject item in _gameObjects)
			{
				item.SetActive(value);
			}
		}
	}
}