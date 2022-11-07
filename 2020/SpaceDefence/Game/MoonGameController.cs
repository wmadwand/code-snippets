using System;
using Dollhouse.Tutorials.Scenarios;
using Coconut.Asyncs;
using Coconut.SoundSystem;
using MiniGames.Games.SpaceDefence.Core.MessageBus;
using MiniGames.Games.SpaceDefence.Difficulties;
using MiniGames.Games.SpaceDefence.SpawnSystem;
using MiniGames.UI;
using SpaceDefenceGeometry;
using UnityEngine;
using Utils.UI;
using Zenject;

namespace MiniGames.Games.SpaceDefence.Game
{
    public class SpaceDefenceGameController : MonoBehaviour
    {
        public MiniGameHealthPanel HealthPanel => _healthPanel;
        public bool IsGameRun { get; private set; }
        public SpaceDefenceSpline ShieldSpline => _shieldSpline;
        public AudioSource music;

        [SerializeField] private SpaceDefenceSpline _shieldSpline;
        [SerializeField] private MiniGameHealthPanel _healthPanel;

        [Inject] private MessageBus _messageBus;
        [Inject] private Shield.Shield _shield;
        [Inject] private CameraShaker _cameraShaker;
        [Inject] private SpaceDefenceSoundController _soundController;
        [Inject] private GameSpawner _gameSpawner;

        private bool _isComplete;
        private MiniGameProgress _progress;
        private int _enemyDestroyedCount;
        private PauseFade _pauseFade;
        private SpaceDefenceDifficultyController _difficultyController;

        //-------------------------------------------------

        public AsyncState RunGame()
        {
            _isComplete = false;

            return Planner.Chain()
                    .AddAction(StartLevel)
                    .AddAction(music.Play)
                    .AddAwait(GameCompleteAwait)
                    .AddAction(_gameSpawner.StopSpawn)
                    .AddAwait(_gameSpawner.RoundLastWaveDestroyAwait)
                    .AddAction(FinishLevel)
                    .AddTimeout(1, true)
                    ;
        }

        //-------------------------------------------------

        [Inject]
        private void Construct(MiniGameProgress progress, PauseFade pauseFade, SpaceDefenceDifficultyController difficultyController)
        {
            _progress = progress;
            _pauseFade = pauseFade;
            _difficultyController = difficultyController;
        }

        private void Awake()
        {
            _messageBus.HazardDestroyed.Receive += OnHazardDestroyed;
            _messageBus.PlayerDamaged.Receive += OnPlayerDamaged;
            _messageBus.PlayerGetHealth.Receive += OnPlayerGetHealth;
        }

        private void OnDestroy()
        {
            _messageBus.HazardDestroyed.Receive -= OnHazardDestroyed;
            _messageBus.PlayerDamaged.Receive -= OnPlayerDamaged;
            _messageBus.PlayerGetHealth.Receive -= OnPlayerGetHealth;
        }

        private void OnPlayerGetHealth(int obj, Transform remedyTransform)
        {
            _healthPanel?.AddHealth(remedyTransform);
        }

        private void OnPlayerDamaged(int value)
        {
            _cameraShaker.Shake();
            _healthPanel?.LoseHealth(true);
        }

        private void OnHazardDestroyed(Transform obj)
        {
            ++_enemyDestroyedCount;

            _progress.Increment(true);
            _soundController.Play(AudioName.Progress);

            if (_enemyDestroyedCount >= _difficultyController.CurrentDifficulty.GetTotalGameHazardCount())
            {
                _isComplete = true;
            }
        }

        private void GameCompleteAwait(AsyncStateInfo state)
        {
            state.IsComplete = _isComplete;
        }

        private void StartLevel()
        {
            IsGameRun = true;
            _gameSpawner.StartSpawn();
        }

        private void FinishLevel()
        {
            _soundController.Play(AudioName.WinGame);
            _shield.Movement.TerminateMoveJoint();
        }
    }

    //-------------------------------------------------

    [Serializable]
    public class GameSettings
    {
        public int minGamesCount;
        public int maxGamesCount;
    }
}