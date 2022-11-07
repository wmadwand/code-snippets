using Notifications;
using UI.Dialogs;
using UnityEngine;
using VertexDI.Attributes;
using VertexDI.Core;

namespace ZombieTicket
{
	public class ZombieTicketShop
	{
		public int Price => DBManager.itemTable.GetRecordById<ItemRecord>(_zombieTicketService.TicketItem.id).cost.GetValue();

		[Inject] private IPlayerDataService _playerDataService;
		[Inject] private IZombieTicketService _zombieTicketService;

		private bool _buyItemResult;

		//---------------------------------------------------------

		public ZombieTicketShop()
		{
			App.inject(this);
		}

		public void Buy()
		{
			if (_zombieTicketService.GotTicketsMax())
			{
				return;
			}

			_buyItemResult = UserDBManager.user.BuyItem(_zombieTicketService.TicketItem.id);

			if (_buyItemResult)
			{
				var alertParams = new ShowAlertNotificationParams
				{
					TitleAlignment = TextAnchor.MiddleCenter,
					BodyAlignment = TextAnchor.MiddleCenter,
					AlertType = AlertDialogType.Ok,
					Title = "Shop",
					Body = "Purchasing operation is complete",
					PositiveButtonText = "OK"
				};

				App.SharedNotificationDispatcher.dispatch(NotificationType.ShowAlert, alertParams);
			}
			else
			{
				var alertParams = new ShowAlertNotificationParams
				{
					TitleAlignment = TextAnchor.MiddleCenter,
					BodyAlignment = TextAnchor.MiddleCenter,
					AlertType = AlertDialogType.Ok,
					Title = "Shop",
					Body = "ERROR: not enough gold. Purchasing operation is not complete.",
					PositiveButtonText = "OK",
				    CloseDelegate = OnButtonGoToShopClick
                };

				App.SharedNotificationDispatcher.dispatch(NotificationType.ShowAlert, alertParams);
			}
		}

	    private void OnButtonGoToShopClick(AlertCloseResult result, object payload)
	    {
	        App.SharedNotificationDispatcher.dispatch(NotificationType.CloseDialog, CloseDialogNotificationParams.Get(ViewName.ChooseLevelMode, true));

            object[] data = new object[2];
	        data[0] = MainMenuTab.Shop;
	        data[1] = (int)ShopTab.Gold;
	        App.SharedNotificationDispatcher.dispatch(NotificationType.MainMenuEmulateClick, data);
        }

        public void ShowScreen()
		{
			App.SharedNotificationDispatcher.dispatch(NotificationType.ShowDialog, ShowDialogNotificationParams.Get(ViewName.ZombieTicketShopScreen));
		}
	}
}