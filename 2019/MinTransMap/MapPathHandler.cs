using DG.Tweening;
using UnityEngine;

namespace MinTrans
{
	public class MapPathHandler : MonoBehaviour, IObserver
	{
		[SerializeField] protected string _category;
		protected bool _isShown;
		protected Material _material;

		//---------------------------------------------------------

		public void UpdateState(Category category)
		{
			SetVisible(!_isShown && category.isActive ? 1 : 0);
		}

		//---------------------------------------------------------

		protected virtual void Awake()
		{
			AppManager.Instance?.UIManager.Subscribe(this, _category);

			_material = GetComponent<SpriteRenderer>().material;
			_material.color = new Color(_material.color.r, _material.color.g, _material.color.b, 0);
		}

		protected void Start()
		{
			DOTween.SetTweensCapacity(1250, 50);
			_material.DOFade(0, 1.5f).SetAutoKill(false).Pause();
		}

		protected void OnDestroy()
		{
			AppManager.Instance?.UIManager.Unsubscribe(this, _category);
		}

		protected void SetVisible(float endValue, float duration = 1.5f)
		{
			_material.DOFade(endValue, duration);
			_isShown = !_isShown;
		}
	}
}