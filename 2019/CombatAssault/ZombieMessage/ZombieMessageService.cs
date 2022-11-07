using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieMessage
{
	public class ZombieMessageService : MonoBehaviour
	{
		public static event Action<IZombieMessage> OnNotify;		

		private Queue<object> _eventQueue = new Queue<object>();
		private ZombieMessageFactory _messageFactory;		

		//---------------------------------------------------------

		public void Start()
		{
			_messageFactory = new ZombieMessageFactory();
		}

		private void Update()
		{
			CheckMessageQueue();
		}

		private void CheckMessageQueue()
		{
			if (_eventQueue.Count <= 0)
			{
				return;
			}

			ProcessQueueItem(_eventQueue.Dequeue());
		}

		//---------------------------------------------------------

		public void OnGetNotified(/*NotificationType notificationtype, */object notificationparams)
		{
			_eventQueue.Enqueue(notificationparams);
		}

		private void ProcessQueueItem(object notificationparams)
		{
			ProcessEvent(notificationparams);
		}

		private void ProcessEvent(object settings)
		{
			IZombieMessage result = _messageFactory.Create(settings);
			OnNotify?.Invoke(result);
		}
	} 
}