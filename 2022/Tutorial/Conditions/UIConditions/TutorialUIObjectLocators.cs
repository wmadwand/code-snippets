using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Linq;

namespace TutorialActions
{
	public abstract class GameObjectLocator
	{
		public abstract GameObject Locate(bool warningIfNotFound = true);
	}

	public class DirectGameObject : GameObjectLocator
	{
		private GameObject Go;

		public DirectGameObject(GameObject go)
		{
			this.Go = go;
		}

		public override GameObject Locate(bool warningIfNotFound)
		{
			return Go;
		}
	}

	public class GameObjectByPathLocator : GameObjectLocator
	{
		[SerializeField] public string Path;

		public override GameObject Locate(bool warningIfNotFound)
		{
			GameObject go = GameObject.Find(Path);
			if (!go && warningIfNotFound)
			{
				Debug.LogWarningFormat("GameObject with path not found: {0}", Path);
				return null;
			}

			return go;
		}
	}

	public class GameObjectByPathAndCountLocator : GameObjectByPathLocator
	{
		[SerializeField] private int Count;
		[SerializeField] private bool IncludeHidden;

		public override GameObject Locate(bool warningIfNotFound)
		{
			GameObject go = GameObject.Find(Path);
			if (!go && warningIfNotFound)
			{
				Debug.LogWarningFormat("GameObject with path not found: {0}", Path);
				return null;
			}


			int found = 0;
			foreach (Transform t in go.transform)
			{
				if (!IncludeHidden && !t.gameObject.activeInHierarchy)
				{
					continue;
				}
				if (Count == found)
				{
					return t.gameObject;
				}
				++found;
			}

			if (warningIfNotFound)
			{
				Debug.LogWarningFormat("GameObject with path and count not found: {0} ({1})", Path, Count);
			}
			return null;
		}
	}

	public class GameObjectByTagLocator : GameObjectLocator
	{
		[SerializeField]
		protected string Tag;

		public override GameObject Locate(bool warningIfNotFound)
		{
			GameObject go = GameObject.FindGameObjectWithTag(Tag);
			if (!go && warningIfNotFound)
			{
				Debug.LogWarningFormat("GameObject with tag not found: {0}", Tag);
				return null;
			}

			return go;
		}
	}

	public class GameObjectByTagAndCountLocator : GameObjectByTagLocator
	{
		[SerializeField]
		protected int Count;

		public override GameObject Locate(bool warningIfNotFound)
		{
			GameObject[] gos = GameObject.FindGameObjectsWithTag(Tag);
			if (gos.Length > Count)
			{
				return gos[Count];
			}

			if (warningIfNotFound)
			{
				Debug.LogWarningFormat("GameObject with tag and count not found: {0}", Tag, Count);
			}
			return null;
		}
	}

	public class GameObjectByTagAndSiblingIndexLocator : GameObjectByTagLocator
	{
		[SerializeField]
		protected int SiblingIndex;

		public override GameObject Locate(bool warningIfNotFound)
		{
			GameObject[] gos = GameObject.FindGameObjectsWithTag(Tag);
			foreach (var go in gos)
			{
				if (go.transform.GetSiblingIndex() == SiblingIndex)
				{
					return go;
				}
			}

			if (warningIfNotFound)
			{
				Debug.LogWarningFormat("GameObject with tag and sibling index not found: {0}", Tag, SiblingIndex);
			}
			return null;
		}
	}

	public class GameObjectByPathRootMatchLocator : GameObjectLocator
	{
		[SerializeField] public string StartWith;
		[SerializeField] public string ChildName;

		public override GameObject Locate(bool warningIfNotFound)
		{
			var count = UnityEngine.SceneManagement.SceneManager.sceneCount;
			for (var i = 0; i < count; i++)
			{
				var roots = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i).GetRootGameObjects();
				var go = roots.FirstOrDefault(g => g.name.StartsWith(StartWith));
				if (go)
				{
					if (!string.IsNullOrEmpty(ChildName))
					{
						var child = go.transform.FindChildRecursively(ChildName);
						if (child)
						{
							return child.gameObject;
						}
					}
					else
					{
						return go;
					}
				}
			}
			return null;
		}
	}

	public class LocatedUIButton
	{
		public GameObject GameObject;
		public Button Button;

		public static implicit operator bool(LocatedUIButton b)
		{
			return b != null && b.Button && b.GameObject;
		}
	}

	public class SpellSlotItemLocator : GameObjectLocator
	{
		public bool CanUpgrade;
		public ComparatorInt SublingIndex;

		public SpellSlotItem GetSlotItem()
        {
			var items = GameObject.FindObjectsOfType<SpellSlotItem>();
			foreach (var i in items)
			{
				if (i.CanUpgrade == CanUpgrade && SublingIndex.IsMet(i.transform.GetSiblingIndex()))
				{
					return i;
				}
			}
			return null;
		}

		public override GameObject Locate(bool warningIfNotFound)
		{
			var item = GetSlotItem();
			if (!item)
			{
				if (warningIfNotFound)
				{
					Debug.LogError("Not found SpellSlotItemLocator");
				}
				return null;
			}
			return item.gameObject;
		}
	}

	public class BonusDropItemLocator : GameObjectLocator
	{
		public override GameObject Locate(bool warningIfNotFound)
		{
			var item = GameObject.FindObjectOfType<DropView>();
			if (!item)
			{
				if (warningIfNotFound)
				{
					Debug.LogError("Not found BonusDropItemLocator");
				}
				return null;
			}
			return item.gameObject;
		}
	}


	public interface IHighlightActionObjectLocator
	{
		LocatedUIButton Locate();
	}

	public abstract class UIButtonLocator : IHighlightActionObjectLocator
	{
		public abstract LocatedUIButton Locate();
	}

	public class InstanceUIButtonLocator : UIButtonLocator
	{
		public LocatedUIButton Button;
		public override LocatedUIButton Locate()
		{
			return Button;
		}
	}

	public class SceneUIButtonLocator : UIButtonLocator
	{
		[SerializeField]
		public GameObjectLocator GameObjectLocator;
		[SerializeField]
		public string ButtonName;

		public override LocatedUIButton Locate()
		{
			var obj = GameObjectLocator.Locate();
			if (!obj)
			{
				return null;
			}

			if (!string.IsNullOrEmpty(ButtonName))
			{
				var buttonTransform = obj.transform.Find(ButtonName);
				if (!buttonTransform)
				{
					Debug.LogErrorFormat("GameObject does not contains transform with name: {0}", ButtonName);
					return null;
				}
				obj = buttonTransform.gameObject;
			}

			Button button = obj.GetComponentInChildren<Button>();
			if (!button)
			{
				Debug.LogErrorFormat("GameObject does not contains Button component: {0}", obj.name);
				return null;
			}

			return new LocatedUIButton
			{
				GameObject = obj,
				Button = button
			};
		}
	}
}
