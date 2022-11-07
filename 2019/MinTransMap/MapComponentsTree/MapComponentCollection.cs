using System.Collections.Generic;
using UnityEngine;

namespace MinTrans
{
	public class MapComponentCollection : MapComponent
	{
		protected Dictionary<string, MapComponent> _mapComponents = new Dictionary<string, MapComponent>();

		//---------------------------------------------------------

		public MapComponentCollection(string name, GameObject[] gameObjects) : base(name, gameObjects) { }

		public override void SetActive(bool value)
		{
			foreach (KeyValuePair<string, MapComponent> item in _mapComponents)
			{
				item.Value.SetActive(value);
			}
		}

		public override void Add(MapComponent component)
		{
			_mapComponents[component.Name] = component;
		}

		public override void Remove(MapComponent component)
		{
			_mapComponents.Remove(component.Name);
		}

		public override Dictionary<string, MapComponent> GetChildren()
		{
			return _mapComponents;
		}

		public override MapComponent FindChild(string name)
		{
			MapComponent foundChild = null;

			foreach (KeyValuePair<string, MapComponent> item in _mapComponents)
			{
				if (item.Key == name)
				{
					foundChild = item.Value;
					break;
				}
				else
				{
					if (item.Value.GetType() == typeof(MapComponentItem))
					{
						continue;
					}

					foundChild = item.Value.FindChild(name);

					if (foundChild != null) { break; }
				}
			}

			return foundChild;
		}
	}
}