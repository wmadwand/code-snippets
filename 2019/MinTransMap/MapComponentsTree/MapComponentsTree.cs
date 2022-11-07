using System.Collections.Generic;
using UnityEngine;

namespace MinTrans
{
	public enum MapComponentType
	{
		Country,
		District,
		Region
	}

	public class MapComponentsTree : MapComponentCollection
	{
		private Dictionary<string, MapRegion> _mapRegions;

		//---------------------------------------------------------

		public MapComponentsTree(string name, GameObject[] gameObjects, Dictionary<string, MapRegion> mapRegions) : base(name, gameObjects)
		{
			_mapRegions = mapRegions;
		}

		public void Build()
		{
			if (_mapComponents.Count > 0)
			{
				return;
			}

			int counter = 0;

			foreach (KeyValuePair<string, District> district in AppManager.Instance.TextMananager.GetDistricts())
			{
				MapComponentCollection mapDistrictData = new MapComponentCollection(district.Key, null);

				// Add district of the current iteration as a child of the top node called MapComponentsTree
				this.Add(mapDistrictData);

				// Add all the regions as childs for the district has just been added, ones that belong it
				foreach (RegionItem item in district.Value.regions)
				{
					MapRegion mapRegionView;

					if (_mapRegions.TryGetValue(item.key, out mapRegionView))
					{
						MapComponentCollection mapRegionData = new MapComponentCollection(mapRegionView.Key, new GameObject[] { mapRegionView.gameObject });
						 
						mapDistrictData.Add(mapRegionData);

						List<MapObject> mapObjectsView;

						if (AppManager.Instance.ObjectManager.MapObjects.TryGetValue(item.key, out mapObjectsView))
						{
							foreach (MapObject mapObjectView in mapObjectsView)
							{
								mapRegionData.Add(new MapComponentItem(mapObjectView.name, mapObjectView.gameObjects.ToArray()));
							}
						}

						List<Mineral> mapMineralsView = AppManager.Instance.ObjectManager.MineralsManager.GetMineralsByRegion(item.key);

						// Add all the minerals as childs for the current iteration region, ones that belong it
						foreach (Mineral mineral in mapMineralsView)
						{
							mapRegionData.Add(new MapComponentItem(mineral.id.ToString() + "#" + counter++, new GameObject[] { mineral.gameObject }));
						}
					}
				}
			}
		}

		public void SelectMapComponentObjects(MapComponentType mapComponentType, string mapComponentName)
		{
			SetActiveAllMapObjects(false);

			switch (mapComponentType)
			{
				case MapComponentType.Country:
					{
						SetActiveAllMapObjects(true);
					}
					break;
				case MapComponentType.District:
					{
						_mapComponents[mapComponentName].SetActive(true);
					}
					break;
				case MapComponentType.Region:
					{
						// Alternative way: MapComponent region = this.FindChild(parentName); region?.SetActive(true);						
						MapComponent district = _mapComponents[AppManager.Instance.TextMananager.GetDistrictByRegion(mapComponentName).key];
						district.GetChildren()[mapComponentName]?.SetActive(true);
					}
					break;
			}
		}

		//---------------------------------------------------------

		private void SetActiveAllMapObjects(bool value)
		{
			foreach (KeyValuePair<string, MapComponent> item in _mapComponents)
			{
				item.Value.SetActive(value);
			}
		}
	}
}