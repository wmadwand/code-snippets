using System;
using UnityEngine;
using UnityEngine.Advertisements;
using VertexDI.Attributes;
using VertexDI.Core;

namespace ZombieTicket
{
	public class ZombieTicketAds
	{
		public DateTime Timestamp => UserDBManager.user.Current.zombieTicketAdsTimestamp;

		[Inject] private IZombieTicketService _zombieTicketService;

		// MANUAL & TUTORIALS
		//https://unity3d.com/learn/tutorials/topics/production/services-integrating-unity-ads
		//https://unity3d.com/services/ads/quick-start-guide
		//https://unityads.unity3d.com/help/unity/integration-guide-unity

		//---------------------------------------------------------

		public ZombieTicketAds()
		{
			App.inject(this);
		}

		public void GetFreeTicket()
		{
			if (_zombieTicketService.GotTicketsMax() || !UnityAds.Instance.FreeZombieTicketReady)
			{
				return;
			}

			UnityAds.AdResult = AdResultHandler;
			UnityAds.Instance.GetFreeZombieTicket(Callback);
		}

		private void Callback(bool flag)
		{
			if (flag)
			{
				return;
			}

			SaveTimestamp();
		}

		private void SaveTimestamp()
		{
			UserDBManager.user.Current.zombieTicketAdsTimestamp = DateTime.UtcNow;
			UserDBManager.user.Current.Save();
		}

		public bool IsNewDay()
		{
			return DateTime.UtcNow.Subtract(UserDBManager.user.Current.zombieTicketAdsTimestamp).Days > 0;
		}

		//---------------------------------------------------------		

		private void AdResultHandler(ShowResult showResult)
		{
			UnityAds.AdResult -= AdResultHandler;

			if (showResult == ShowResult.Finished)
			{
				_zombieTicketService.AddTicket();
			}
		}
	}
}