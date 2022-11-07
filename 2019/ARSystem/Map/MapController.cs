using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoSingleton<MapController>
{
	public MapObject[] MapObjectsInRange => mapObjectsInRange;
	public GameObject[] HUDButtons => _HUDButtons;

	[SerializeField] private MapObject[] mapObjectsInRange;
	[SerializeField] private GameObject[] _HUDButtons;

	[SerializeField] private GameObject gameMap;

	public void SetMapActive(bool flag)
	{
		gameMap.SetActive(flag);
	}

	public void ChangeHUDButtons()
	{
		HUDButtons[0].SetActive(!HUDButtons[0].activeSelf);
		HUDButtons[1].SetActive(!HUDButtons[1].activeSelf);
	}
}