using System;
using System.Collections;
using UnityEngine;
using VertexDI.Attributes;
using VertexDI.Core;
using VertexDI.Game;

namespace ZombieTicket
{
	[InjectionAlias(typeof(IZombieTicketService))]
	public class ZombieTicketService : Service, IZombieTicketService
	{
		public int TicketCount => UserDBManager.userInventory.GetItemCount(TicketItem.id);
		public DateTime TicketTimestamp => UserDBManager.user.Current.zombieTicketTimestamp;
		public ItemSetting TicketItem => bl_GameData.Instance.ZombieTicketItem;
		public ZombieTicketAds Ads { get; private set; }
		public ZombieTicketShop Shop { get; private set; }

		[Inject] private IPlayerDataService _playerDataService;
		private ZombieTicketAds _zombieTicketAds;
		private ZombieTicketShop _zombieTicketShop;

		//---------------------------------------------------------

		public override void run()
		{
			base.run();

			bl_EventHandler.OnLocalPlayerSpawn += OnLocalPlayerSpawnHandler;

			Ads = new ZombieTicketAds();
			Shop = new ZombieTicketShop();
		}

		public void GivePlayerTicketsOnLoad()
		{
			StartCoroutine(WaitForUserInventory(() =>
			{
				if (!UserDBManager.user.Current.isZombieTicketFirstInitDone)
				{
					GivePlayerStartTickets();
				}
				else
				{
					GivePlayerAccruedTickets();
				}
			}));
		}

		public void AddTicket(int count = 1)
		{
			if (TicketCount == bl_GameData.Instance.ZombieTicketMaxCount) { return; }

			UserDBManager.userInventory.AddItem(TicketItem.id, count);
			SaveTimestamp(!GotTicketsMax());
		}

		public void RemoveTicket(int count = 1)
		{
			if (TicketCount <= 0) { return; }

			UserDBManager.userInventory.RemoveItem(TicketItem.id, count);
			SaveTimestamp(true);
		}

		/// <summary>
		/// true - save with the additional hours to the current moment, false - save as DateTime.MinValue
		/// </summary>
		/// <param name="addHours"></param>
		public void SaveTimestamp(bool addHours)
		{
			UserDBManager.user.Current.zombieTicketTimestamp = addHours ? DateTime.UtcNow.AddHours(bl_GameData.Instance.HoursForOneZombieTicket) : DateTime.MinValue;
			UserDBManager.user.Current.Save();
		}

		public bool GotTickets()
		{
			return TicketCount > 0;
		}

		public bool GotTicketsMax()
		{
			return TicketCount == bl_GameData.Instance.ZombieTicketMaxCount;
		}

		//---------------------------------------------------------

		[Inject]
		private void Construct(ZombieTicketAds zombieTicketAds, ZombieTicketShop zombieTicketShop)
		{
			_zombieTicketAds = zombieTicketAds;
			_zombieTicketShop = zombieTicketShop;
		}

		private void OnDestroy()
		{
			bl_EventHandler.OnLocalPlayerSpawn -= OnLocalPlayerSpawnHandler;
		}

		private void OnLocalPlayerSpawnHandler()
		{
			RemoveTicket();
		}

		private void GivePlayerStartTickets()
		{
			if (UserDBManager.user.Current.isZombieTicketFirstInitDone) { return; }

			UserDBManager.user.Current.isZombieTicketFirstInitDone = true;
			UserDBManager.user.Current.Save();

			AddTicket(GameSettings.Instance.zombieTicketStartCount);
		}

		private void GivePlayerAccruedTickets()
		{
			if (GotTicketsMax())
			{
				return;
			}

			int totalHoursPassed = DateTime.UtcNow.Subtract(UserDBManager.user.Current.zombieTicketTimestamp).Hours;
			int ticketCountResult = Mathf.FloorToInt(totalHoursPassed / bl_GameData.Instance.HoursForOneZombieTicket);

			if (ticketCountResult < 1)
			{
				return;
			}

			int availableSlotCount = bl_GameData.Instance.ZombieTicketMaxCount - TicketCount;
			AddTicket(Mathf.Min(availableSlotCount, ticketCountResult));
		}

		private IEnumerator WaitForUserInventory(Action callback)
		{
			yield return new WaitUntil(() => UserDBManager.userInventory != null);
			callback();
		}
	}
}