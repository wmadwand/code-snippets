using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ARSystem
{
	public struct SceneObject
	{
		public IARObject ARObject;
		public MapObject mapObject;

		public SceneObject(IARObject ARObject, MapObject mapObject)
		{
			this.ARObject = ARObject;
			this.mapObject = mapObject;
		}
	}

	public class ARComposer
	{
		#region Variables
		private List<SceneObject> sceneObjects;
		private GameObject sceneObjectParent;
		#endregion

		//---------------------------------------------------------

		public ARComposer(GameObject sceneObjectParent)
		{
			sceneObjects = new List<SceneObject>();
			this.sceneObjectParent = sceneObjectParent;
		}

		public void AddObjects(Action callback, params MapObject[] mapObjects)
		{
			if (mapObjects.Length == 0)
			{
				callback();
			}

			foreach (MapObject mapObject in mapObjects)
			{
				ARObject ARObject = mapObject.ARObject != null ? mapObject.ARObject : mapObject.gameEntity.ARObject; //@TODO: temporary kostil'

				sceneObjects.Add(new SceneObject(ARObject, mapObject));
			}

			ARMode.Instance.StartCoroutine(AddObjectsRoutine(callback));
		}

		public void ShowObjects(Action callback = null)
		{
			ARMode.Instance.StartCoroutine(ShowObjectsRoutine(callback));
		}

		public void HideObjects(Action callback = null)
		{
			sceneObjects.ForEach(sceneObj => sceneObj.ARObject.SetActive(false));

			sceneObjects.Clear();

			callback?.Invoke();
		}

		//---------------------------------------------------------

		private IEnumerator AddObjectsRoutine(Action callback)
		{
			foreach (SceneObject sceneObject in sceneObjects)
			{
				sceneObject.ARObject.Init(sceneObjectParent, sceneObject.mapObject.position);
			}

			if (sceneObjects.Count > 0)
			{
				yield return new WaitWhile(() => sceneObjects.Exists(sceneObj => !sceneObj.ARObject.IsInitiated));
			}

			callback();
		}

		private IEnumerator ShowObjectsRoutine(Action callback)
		{
			if (sceneObjects.Count > 0)
			{
				yield return new WaitWhile(() => sceneObjects.Exists(sceneObj => !sceneObj.ARObject.IsInitiated));

				sceneObjects.ForEach(sceneObj => sceneObj.ARObject.SetActive(true));
			}

			callback();
		}

		private void UpdateArrangement()
		{

		}
	}
}