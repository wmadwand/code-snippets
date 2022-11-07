using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ZombieMessage
{
	public class ZombieMessageItem : MonoBehaviour
	{
		[SerializeField] private float _waitBeforeHide = 2f;

		private Action<GameObject> _callback;
		private Text _text;
		private CanvasGroup _canvasGroup;

		//---------------------------------------------------------

		public void Init(IZombieMessage message, Action<GameObject> callback)
		{
			_text.text = message.GetText();
			_callback = callback;
		}

		//---------------------------------------------------------

		private void Awake()
		{
			_canvasGroup = GetComponent<CanvasGroup>();
			_text = GetComponentInChildren<Text>();
		}

		private void Start()
		{
			StartCoroutine(Hide());
		}

		private IEnumerator Hide()
		{
			yield return new WaitForSeconds(_waitBeforeHide);

			while (_canvasGroup.alpha > 0)
			{
				_canvasGroup.alpha -= Time.deltaTime;
				yield return new WaitForEndOfFrame();
			}

			_callback(this.gameObject);
			Destroy(gameObject);
		}
	}
}