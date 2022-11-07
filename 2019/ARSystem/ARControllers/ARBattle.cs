using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARSystem
{
	public enum StageType
	{
		ArenaAR = 10,
		Arena3D = 20,
		ClearAR = 30
	}

	public class ARBattle : MonoSingleton<ARBattle>, IARController
	{
		#region Variables
		public bool IsInitiated { get; private set; }
		public bool IsActive { get; private set; }

		private Dictionary<StageType, MapObject> stages;
		private MapObject currentStage;
		#endregion

		//---------------------------------------------------------

		public ARBattle()
		{
			stages = new Dictionary<StageType, MapObject>();
		}

		public void Init(StageType stage, MapObject rivalObject)
		{
			if (IsInitiated)
			{
				Debug.Log("ARBattle has been already initiated.");
				return;
			}

			StartCoroutine(InitRoutine(stage, rivalObject));
		}

		public void Run(Action callback = null)
		{
			if (!IsInitiated)
			{
				Debug.Log("ARBattle cannot be ran unless it's initiated.");
				return;
			}

			ARMode.Instance.Run(() => ARMode.Instance._ARTransition.Play(false));

			RunPuppets();

			this.IsActive = true;

			callback?.Invoke();
		}

		public void Stop(Action callback = null)
		{
			if (!IsInitiated)
			{
				Debug.Log("ARBattle cannot be stopped unless it's initiated.");
				return;
			}

			ARMode.Instance._ARTransition.Play(true, () =>
			{
				ARMode.Instance.Stop(() =>
				{
					MapController.Instance.ChangeHUDButtons();
					ARMode.Instance._ARTransition.Play(false);
				});
			});

			this.IsInitiated = false; //@TODO: cope with it
			this.IsActive = false;

			StopPuppets();

			callback?.Invoke();
		}

		//---------------------------------------------------------

		private IEnumerator InitRoutine(StageType stageType, MapObject rivalMapObject)
		{
			bool isARModeReinited = false;

			if (!stages.ContainsKey(stageType))
			{
				ARObject stageARObject = InitStage(stageType);
				MapObject stageMapObject = new MapObject { gameEntity = null, ARObject = stageARObject, position = Vector3.zero };

				stages[stageType] = stageMapObject;
				currentStage = stages[stageType];
			}
			else
			{
				currentStage = stages[stageType];
			}

			ARMode.Instance._ARTransition.Play(true, () => //@TODO: must be driven by Global Fader/Transition class
			{
				if (!ARMode.Instance.IsInitiated)
				{
					ARMode.Instance.Init(stageType, currentStage, rivalMapObject);
					isARModeReinited = true;
				}
				else
				{
					ARMode.Instance.Stop(() =>
					{
						ARMode.Instance.Init(stageType, currentStage, rivalMapObject);
						isARModeReinited = true;
					});
				}
			});

			yield return new WaitUntil(() => ARMode.Instance.IsInitiated && isARModeReinited);

			this.IsInitiated = true;
		}

		private ARObject InitStage(StageType type)
		{
			GameEntityType stage = GameEntityType.ArenaAR;

			switch (type)
			{
				case StageType.ArenaAR: stage = GameEntityType.ArenaAR; break;
				case StageType.Arena3D: stage = GameEntityType.Arena3D; break;
				case StageType.ClearAR:
				default: return null;
			}

			return ARObjectFactory.Construct(stage);
		}

		private void RunPuppets() { }

		private void StopPuppets() { }
	}
}