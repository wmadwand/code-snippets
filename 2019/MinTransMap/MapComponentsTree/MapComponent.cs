using System.Collections.Generic;
using UnityEngine;

namespace MinTrans
{
	public abstract class MapComponent
	{
		public string Name => _name;

		protected string _name;
		protected GameObject[] _gameObjects;

		//---------------------------------------------------------

		protected MapComponent(string name, GameObject[] gameObjects)
		{
			_name = name;
			_gameObjects = gameObjects;
		}

		//---------------------------------------------------------

		public abstract void SetActive(bool value);

		public virtual void Add(MapComponent component) { }

		public virtual void Remove(MapComponent component) { }

		public virtual Dictionary<string, MapComponent> GetChildren()
		{
			return null;
		}

		public virtual MapComponent FindChild(string name)
		{
			return null;
		}
	}
}