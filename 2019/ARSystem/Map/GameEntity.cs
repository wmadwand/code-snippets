using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ARSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum GameEntityType
{
	Player = 0,
	ArenaAR = 10,
	Arena3D = 20,
	Monster = 30,
	Chest = 40,
	Mimic = 50
}

public class GameEntity : MonoBehaviour, IPointerDownHandler
{
	#region Variables
	public GameEntityType Type => type;
	//public MapObject MapObject { get; private set; }
	public ARObject ARObject { get; private set; }

	[SerializeField] private GameEntityType type;

	[SerializeField] private GameObject popup;
	[SerializeField] private ToggleGroup toggleGroup;

	private StageType chosenStageType;
	#endregion

	//---------------------------------------------------------

	public void OnPointerDown(PointerEventData eventData)
	{
		switch (Type)
		{
			case GameEntityType.Monster:
				SetActive();
				break;
			case GameEntityType.Chest:
			case GameEntityType.Mimic:
			default:
				ExploreObject();
				break;
		}
	}

	public void SetActive()
	{
		popup.SetActive(!popup.activeSelf);
	}

	public void ExploreObject()
	{
		StartCoroutine(ExploreObjectRoutine());
	}

	public void StartBattle()
	{
		StartCoroutine(StartBattleRoutine());
	}

	public void SetStageType(int type)
	{
		chosenStageType = (StageType)type;
	}

	//---------------------------------------------------------

	private void Awake()
	{
		ARObject = ARObjectFactory.Construct(Type);
		chosenStageType = StageType.ArenaAR;
	}

	private IEnumerator ExploreObjectRoutine()
	{
		MapObject mapObj = new MapObject { gameEntity = this, position = new Vector3(0, -1, 5) };

		ARMode.Instance._ARTransition.Play(true, () => //@TODO: must be driven by Global Fader/Transition class
		{
			ARMode.Instance.Init(StageType.ClearAR, mapObj);
		});

		yield return new WaitUntil(() => ARMode.Instance.IsInitiated);

		ARMode.Instance.Run(() => ARMode.Instance._ARTransition.Play(false));
	}

	private IEnumerator StartBattleRoutine()
	{
		MapObject mapObj = new MapObject { gameEntity = this, position = new Vector3(0, -1, 5) };

		var sdf = toggleGroup?.ActiveToggles();

		ARBattle.Instance.Init(chosenStageType, mapObj);

		yield return new WaitUntil(() => ARBattle.Instance.IsInitiated);

		ARBattle.Instance.Run(() => MapController.Instance.ChangeHUDButtons());
	}
}