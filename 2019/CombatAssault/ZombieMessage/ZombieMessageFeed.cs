using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieMessage
{
	public class ZombieMessageFeed : MonoBehaviour
	{

		[SerializeField] private GameObject _itemMessage;
		[SerializeField] private Transform[] _pivotTransforms;
		[SerializeField] private float _dropDownTime = .5f;

		private List<GameObject> _itemUIList = new List<GameObject>();
		private Dictionary<GameObject, int> _itemUICurrentPivot = new Dictionary<GameObject, int>();
		private Queue<IZombieMessage> _messageQueue = new Queue<IZombieMessage>();
		private bool _isMessageAdding;

		//---------------------------------------------------------

		private void Awake()
		{
			ZombieMessageService.OnNotify += ZombieAnnouncer_OnNotify1;
		}

		private void Update()
		{
			if (_messageQueue.Count < 1 || _isMessageAdding)
			{
				return;
			}

			StartCoroutine(ShowMessageRoutine(_messageQueue.Dequeue()));
		}

		private void OnDestroy()
		{
			ZombieMessageService.OnNotify -= ZombieAnnouncer_OnNotify1;
		}

		private void ZombieAnnouncer_OnNotify1(IZombieMessage message)
		{
			_messageQueue.Enqueue(message);
		}

		private void RemoveUIItemFromList(GameObject go)
		{
			_itemUIList.Remove(go);
			_itemUICurrentPivot.Remove(go);
		}

		private IEnumerator ShowMessageRoutine(IZombieMessage message)
		{
			_isMessageAdding = true;

			for (int i = 0; i < _itemUIList.Count; i++)
			{
				int index = _itemUICurrentPivot[_itemUIList[i]] += 1;
				_itemUIList[i].transform.DOMove(_pivotTransforms[index].position, _dropDownTime);
			}

			CreateMessage(message);

			yield return new WaitForSeconds(1);
			_isMessageAdding = false;
		}

		private void CreateMessage(IZombieMessage message)
		{
			GameObject go = Instantiate(_itemMessage, _pivotTransforms[0], false);
			go.GetComponent<ZombieMessageItem>().Init(message, RemoveUIItemFromList);

			_itemUIList.Add(go);
			_itemUICurrentPivot[go] = 0;
		}
	}
}