using System;
using UnityEngine;
using UnityEngine.UI;
using VertexDI.Attributes;
using VertexDI.Core;

namespace ZombieTicket
{
	public class ZombieTicketManager : DIBehaviour
	{
		[SerializeField] private ZombieTicketTimer _timer;
		[SerializeField] private Text _zombieTicketsCountText;
		[SerializeField] private GameObject _ticketTimerLinePanel;
		[SerializeField] private GameObject _debugPanel;
		[SerializeField] private Button _watchAdButton;
		[SerializeField] private Button _buyTicketButton;

		[Inject] private IPlayerDataService _playerDataService;
		[Inject] private IZombieTicketService _zombieTicketService;

		private bool _isTimerInited;
		private TimeSpan _timeDelta;

		//---------------------------------------------------------

		#region DEBUGPanelOperations

#if POLYPLAY_DEBUG

		public void ResetTickets_DEBUG()
		{
			UserDBManager.user.Current.isZombieTicketFirstInitDone = false;
			UserDBManager.user.Current.zombieTicketTimestamp = DateTime.MinValue;

			UserDBManager.userInventory.RemoveItem(_zombieTicketService.TicketItem.id, UserDBManager.userInventory.GetItemCount(_zombieTicketService.TicketItem.id));
			UserDBManager.user.Current.Save();
		}

		public void AddTicket_DEBUG()
		{
			_zombieTicketService.AddTicket();
		}

		public void RemoveTicket_DEBUG()
		{
			_zombieTicketService.RemoveTicket();
		}

		public void SetTimerNsec_DEBUG(int secValue)
		{
			_timer.SetTimer_DEBUG(secValue);
		}

		public void SetTimestamp_DEBUG(float hr)
		{
			UserDBManager.user.Current.zombieTicketTimestamp = UserDBManager.user.Current.zombieTicketTimestamp.AddHours(2);
			UserDBManager.user.Current.Save();
		}

		public void ResetTicketDailyAds_DEBUG()
		{
			UnityAds.Instance.ResetTicketDailyAds_DEBUG();
		}
#endif

		#endregion

		//---------------------------------------------------------

		private void OnEnable()
		{
			if (_zombieTicketService?.TicketCount < bl_GameData.Instance.ZombieTicketMaxCount)
			{
				_isTimerInited = false;
			}
		}

		protected override void onAppInitialized()
		{
			base.onAppInitialized();

			_watchAdButton.onClick.AddListener(() => { _zombieTicketService.Ads.GetFreeTicket(); });
			_buyTicketButton.onClick.AddListener(() => { _zombieTicketService.Shop.Buy(); });
		}

		private void Start()
		{
			_buyTicketButton.GetComponentInChildren<Text>().text = $"BUY TICKET FOR {_zombieTicketService.Shop.Price} GOLD";

#if POLYPLAY_DEBUG
			_debugPanel.SetActive(true);
#else
		_debugPanel.SetActive(false);
#endif
		}

		private void Update()
		{
			UpdateButtons();
			UpdateTimer();
		}

		private void UpdateButtons()
		{
			if (_zombieTicketService.GotTicketsMax())
			{
				_watchAdButton.gameObject.SetActive(false);
				_buyTicketButton.gameObject.SetActive(false);
			}
			else
			{
				_watchAdButton.gameObject.SetActive(UnityAds.Instance.FreeZombieTicketReady);
				_buyTicketButton.gameObject.SetActive(!UnityAds.Instance.FreeZombieTicketReady);
			}

			_zombieTicketsCountText.text = $"{_zombieTicketService.TicketCount}";
		}

		private void UpdateTimer()
		{
			if (_zombieTicketService.TicketTimestamp == DateTime.MinValue || _zombieTicketService.GotTicketsMax())
			{
				_ticketTimerLinePanel.SetActive(false);
				_timer.Stop();
			}
			else
			{
				if (_isTimerInited)
				{
					return;
				}

				_isTimerInited = true;
				_ticketTimerLinePanel.SetActive(true);

				_timeDelta = _zombieTicketService.TicketTimestamp.Subtract(DateTime.UtcNow);
				InitTimer(Math.Abs(_timeDelta.TotalSeconds));
			}
		}

		private void InitTimer(double seconds)
		{
			_timer.OnFinish = OnTimerFinish;
			_timer.InitAndStartTimer(seconds);
		}

		private void OnTimerFinish()
		{
			if (_zombieTicketService.TicketTimestamp == DateTime.MinValue || _zombieTicketService.GotTicketsMax())
			{
				return;
			}

			_zombieTicketService.AddTicket();
			OnEnable();
		}		
	}
}