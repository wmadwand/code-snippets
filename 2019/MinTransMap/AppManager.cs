using MinTrans.MapSceneBuilderModule;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MinTrans
{
	public class AppManager : MonoSingleton<AppManager>
	{
		#region Variables
		public const string RUSSIA_REGION = "Russia";

		public ObjectManager ObjectManager;
		public ObjectCardManager ObjectCardManager;
		public TextManager TextMananager;
		public UIManager UIManager;
		public CameraMovement CameraManager => _cameraManager;		

		[HideInInspector] public MapModelManager MapModelManager { get; private set; }
		[HideInInspector] public MapZoom MapZoom { get; private set; }
		[HideInInspector] public string CurrentRegion { get; private set; }
		[HideInInspector] public Dictionary<string, RegionItem> regions;

		[SerializeField] private CameraMovement _cameraManager;		

		private Map _mapScene;		
		#endregion

		//---------------------------------------------------------

		public void BackToDistricts()
		{
			MapModelManager.DeselectAllRegions();

			if (MapZoom.stateEnum == MapZoomStateEnum.ZoomIn02)
			{
				string currDistrict = TextMananager.GetDistrictByRegion(CurrentRegion).key;
				CurrentRegion = currDistrict;

				MapModelManager.SelectRegions(TextMananager.GetDistricts()[currDistrict].regions, currDistrict);

			}
			else if (MapZoom.stateEnum == MapZoomStateEnum.ZoomIn01)
			{
				CurrentRegion = RUSSIA_REGION;

				UIManager.SwitchPanels();
				UIManager.ClearListOfButtons();
			}

			MapZoom.ZoomOut();

			UIManager.ScrollRegionListToTop();
		}

		public void ShowDistrictRegions(string district)
		{
			CurrentRegion = district;

			MapZoom.ZoomIn();

			MapModelManager.SelectRegions(TextMananager.GetDistricts()[district].regions, district);

			UIManager.CreateRegionsButtons(district);
			UIManager.SwitchPanels();
		}

		public void GetBackHome()
		{
			CurrentRegion = RUSSIA_REGION;

			MapModelManager.DeselectAllRegions();

			UIManager.GetBackHome();

			ObjectCardManager.CloseCard();

			MapZoom.ZoomOut();
			MapZoom.ZoomOut();
		}

		public void GoToRegion(string region)
		{
			CurrentRegion = region;

			MapZoom.ZoomIn();
			MapModelManager.SelectOneRegion(region);
		}

		//---------------------------------------------------------

		private void Awake()
		{
			BuildMapScene();

			MapZoom = new MapZoom(new ZoomIn00MapZoomState(), MapZoomStateEnum.ZoomIn00);
			MapRegion.OnClick += MapRegionOnClickHandler;
		}

		private void Start()
		{
			StartCoroutine(GarbageCollectLoop());

			CurrentRegion = RUSSIA_REGION;

			if (Display.displays.Length > 1) Display.displays[1].Activate();
			if (Display.displays.Length > 2) Display.displays[2].Activate();

			regions = TextMananager.GetRegions();

			// DO NOT REMOVE. It's for service event sending with initial map zoom state = ZoomIn00MapState
			MapZoom.ZoomOut();
		}

		private void OnDestroy()
		{
			MapRegion.OnClick -= MapRegionOnClickHandler;
		}

		private void BuildMapScene()
		{
			MapSceneDirector director = new MapSceneDirector();
			MapSceneBuilder builder = new PCMapSceneBuilder();
			director.CreateMap(builder);

			_mapScene = builder.Map;
			MapModelManager = _mapScene.Model.Manager;
		}

		private void MapRegionOnClickHandler(string region)
		{
			if (region == CurrentRegion)
			{
				return;
			}

			CurrentRegion = region;

			if (MapZoom.stateEnum != MapZoomStateEnum.ZoomIn02)
			{
				MapZoom.ZoomIn();
				MapZoom.ZoomIn();
			}
			else
			{
				MapZoom.ZoomIn();
			}

			MapModelManager.SelectOneRegion(region);
		}		
	}
}