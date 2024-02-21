using Cysharp.Threading.Tasks;
using ReadyPlayerMe.Core;
using Project.Analytics;
using Project.Avatar;
using Project.Data;
using Project.Data.Google;
using Project.Gameplay;
using Project.Gameplay.Map;
using Project.Gameplay.MainPlayer;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Controller
{
    public interface IGameController
    {
        Avatar.Avatar AvatarGenerator { get; }
        IPlayerCamera PlayerCamera { get; }

        UniTask Run(UIController UIController);
    }

    public class GameController : MonoBehaviour, IGameController
    {
        public static event Action<TimeSpan> OnGameStart;
        public IPlayerCamera PlayerCamera => _playerCamera;
        public Avatar.Avatar AvatarGenerator { get; private set; }

        [SerializeField] private RewardStateMachine _rewardStateMachine;
        [SerializeField] private MapController _mapController;
        [SerializeField] private PlayerCamera _playerCamera;

        private UIController _UIController;
        private ITreasureLocalStorage _treasureLocalStorage;

        //--------------------------------------------------------------

        public async UniTask Run(UIController UIController)
        {
            _UIController = UIController;

            await Init();

            var gameLoadedDuringTime = TimeSpan.FromSeconds(Time.unscaledTime);
            SendAnalyticsEvent("session start", gameLoadedDuringTime);

            OnGameStart?.Invoke(gameLoadedDuringTime);
            await UniTask.Delay(1000);
        }

        //--------------------------------------------------------------

        private async UniTask Init()
        {
            //TODO: implement like Rewards.Instance.Init(settings);
            //TODO: remove circle dependence
            _treasureLocalStorage = new TreasureLocalStorage(_rewardStateMachine);
            _UIController.Init(_playerCamera, _rewardStateMachine, _mapController);

            //TODO: remove it
            await _mapController.IsPreInitialized();

            var rewards = Game.Config.App.GoogleSheets.SheetsData[GoogleSheet.Rewards];
            AvatarGenerator = new Avatar.Avatar(new AvatarObject(new AvatarObjectLoader()), new AvatarSprite());
            List<UniTask> tasks = new List<UniTask>
            {
                 Player.Instance.Init(AvatarGenerator),
                _rewardStateMachine.Init(rewards, _treasureLocalStorage)
            };
            await UniTask.WhenAll(tasks);

            await _mapController.IsInitialized();
        }

        private void AskPermissions()
        {
            //if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
            //{
            //    Permission.RequestUserPermission(Permission.Camera);
            //}
            //AndroidRuntimePermissions.Permission result = AndroidRuntimePermissions.RequestPermission("android.permission.ACTIVITY_RECOGNITION");
        }

        private void SendAnalyticsEvent(string name, TimeSpan gameLoadedDuringTime)
        {
            UserCredentialsLocalStorage.Instance.TryGetValue(UserCredentials.Email, out string userEmail);
            var gameLoadedDuringTimeFormat = gameLoadedDuringTime.ToString(@"mm\:ss");

            var eventProps = new Dictionary<string, object>
            {
                { "game_loaded_in", gameLoadedDuringTimeFormat },
                { "user_id", SystemInfo.deviceUniqueIdentifier },
                { "user_email", userEmail ?? "NOT FOUND" },
            };

            AmplitudeAnalytics.Instance.SendEvent(name, eventProps);
        }

        private void OnApplicationQuit()
        {
            _treasureLocalStorage.Save();
        }

        private void OnApplicationFocus(bool focus)
        {
            if (!focus)
            {
                _treasureLocalStorage.Save();
            }
        }
    }
}