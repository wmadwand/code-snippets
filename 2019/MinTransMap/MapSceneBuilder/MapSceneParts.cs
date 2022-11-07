using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DTools;

namespace MinTrans.MapSceneBuilderModule
{
	public abstract class MapScenePart
	{
		public MapScenePart(string path)
		{
			CreateObject(path);
		}

		public virtual void CreateObject(string path)
		{
			GameObject prefab = Resources.Load<GameObject>(path);
			Object.Instantiate(prefab, GameObject.FindGameObjectWithTag("ContentParent").transform);

			//UpdateManager.PerformCoroutine(loadFromResourcesFolder(path));
		}

		IEnumerator ResourcesLoadAsync(string path)
		{
			ResourceRequest loadAsync = Resources.LoadAsync<GameObject>(path);
			loadAsync.allowSceneActivation = true;

			while (!loadAsync.isDone)
			{
				Debug.Log($"{path}Load Progress:  + {loadAsync.progress}");
				yield return null;
			}

			GameObject prefab = loadAsync.asset as GameObject;

			Object.Instantiate(prefab, GameObject.FindGameObjectWithTag("ContentParent").transform);
		}
	}

	public class Map : MapScenePart
	{
		public MapScenePart Background { get; set; }
		public Model Model { get; set; }
		public List<MapScenePart> MapPaths { get; set; } = new List<MapScenePart>();

		public Map(string path) : base(path) { }

		public override void CreateObject(string path) { }
	}

	public class Background : MapScenePart
	{
		public Background(string path) : base(path) { }
	}

	public class Model : MapScenePart
	{
		public MapModelManager Manager { get; private set; }

		public Model(string path) : base(path) { }

		public override void CreateObject(string path)
		{
			GameObject prefab = Resources.Load<GameObject>(path);
			GameObject go = Object.Instantiate(prefab, GameObject.FindGameObjectWithTag("ContentParent").transform);
			Manager = go.GetComponent<MapModelManager>();

			//UpdateManager.PerformCoroutine(loadFromResourcesFolder(path));
		}
	}

	public class Path : MapScenePart
	{
		public Path(string path) : base(path) { }
	}
}