using System;
using VertexDI.Core;

namespace ZombieTicket
{
	public interface IZombieTicketService : IService
	{
		int TicketCount { get; }
		DateTime TicketTimestamp { get; }
		ItemSetting TicketItem { get; }
		ZombieTicketAds Ads { get; }
		ZombieTicketShop Shop { get; }

		void GivePlayerTicketsOnLoad();
		void AddTicket(int count = 1);
		void RemoveTicket(int count = 1);
		void SaveTimestamp(bool value);
		bool GotTickets();
		bool GotTicketsMax();
	}
}