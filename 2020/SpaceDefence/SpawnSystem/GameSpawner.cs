using Coconut.Asyncs;
using System;
using System.Linq;
using MiniGames.Games.SpaceDefence.Core;
using MiniGames.Games.SpaceDefence.Core.MessageBus;
using MiniGames.Games.SpaceDefence.Data;
using MiniGames.Games.SpaceDefence.Difficulties;
using MiniGames.Games.SpaceDefence.Game;
using MiniGames.Games.SpaceDefence.Hazard.Projectile;
using MiniGames.Games.SpaceDefence.SpawnSystem.HazardSpawners;
using MiniGames.Games.SpaceDefence.SpawnSystem.SpawnSystemBuilder;
using UnityEngine;
using Zenject;

namespace MiniGames.Games.SpaceDefence.SpawnSystem
{
    public class GameSpawner : MonoBehaviour
    {
        [SerializeField] private EnemySpotCollection[] _enemySpotsCollections;

        [Inject] private MessageBus _messageBus;
        [Inject] private SpaceDefenceGameController _gameController;
        [Inject] private SpaceDefenceDifficultyController _difficultyController;
        [Inject] private SpawnSystemDirector _spawnSystemDirector;

        private bool _lockSpawn;
        private int _currentRoundIndex;
        private GameRound _gameRound;
        private EnemySpotCollection _gameRoundEnemySpots;
        private int _hazardKilledCount;
        private int _hazardActiveCount;
        private int _hazardActiveCountStamp;
        private HazardSpawnModel _hazardSpawnModel;
        private HazardSpawnModel[] _hazardSpawns;
        private float _hazardLimitCount;
        private AsyncChain _roundHazardSpawnChain;
        private AsyncChain _spawnHazardsChain;
        private AsyncChain _spawnHazardWaveChain;
        private SpawnSystemBuilder.SpawnSystem _spawnSystem;
        private bool _isReachedRoundHazardCount;

        //-------------------------------------------------

        public void StartSpawn()
        {
            _currentRoundIndex = 0;
            _spawnSystem = CreateSpawnSystem();

            Planner.Chain()
                   .AddAwait(SettingsAvailableAwait)
                   .AddAwait(GameStartAwait)
                   .AddAction(() =>
                   {
                       SetupRound(_currentRoundIndex);
                       HazardSpawnLoop();
                   });
        }

        public void StopSpawn()
        {
            _roundHazardSpawnChain.Terminate();
            _spawnHazardWaveChain.Terminate();
            _spawnHazardsChain.Terminate();
        }

        //-------------------------------------------------

        private void SetupRound(int index)
        {
            _gameRound = _difficultyController.CurrentDifficulty.GetCurrentRound(index);

            Array.ForEach(_enemySpotsCollections, item => item.SetActive(false));
            _gameRoundEnemySpots = _enemySpotsCollections.GetRandomItem();
            _gameRoundEnemySpots.SetActive(true);


            //TODO: rework this
            var CometSpawner = (CometSpawner)_spawnSystem.GetSpawner(HazardType.Comet);
            CometSpawner.SpawnHelper.InitializeHazardPaths(0.028f);
            var enemySpawner = (EnemySpawner)_spawnSystem.GetSpawner(HazardType.Enemy);
            enemySpawner.SetEnemySpots(_gameRoundEnemySpots);
        }

        private void ChangeRound()
        {
            ++_currentRoundIndex;

            _spawnHazardWaveChain.Terminate();
            _spawnHazardsChain.Terminate();
            _lockSpawn = false;

            _hazardKilledCount = 0;
            _hazardActiveCountStamp = 0;

            _isReachedRoundHazardCount = false;

            if (_currentRoundIndex >= _difficultyController.CurrentDifficulty.rounds.Length)
            {
                return;
            }

            SetupRound(_currentRoundIndex);
        }

        private void HazardSpawnLoop()
        {
            _roundHazardSpawnChain = Planner.Chain();

            if (_isReachedRoundHazardCount)
            {
                _roundHazardSpawnChain.AddAwait(RoundLastWaveDestroyAwait);
                _roundHazardSpawnChain.AddAction(ChangeRound);
                _roundHazardSpawnChain.AddAction(HazardSpawnLoop);

                return;
            }

            _hazardSpawns = new HazardSpawnModel[_gameRound.HazardSpawns.Length];
            _gameRound.HazardSpawns.CopyTo(_hazardSpawns, 0);
            if (_gameRound.shuffle) _hazardSpawns.Shuffle();

            if (_hazardSpawns.Length == 0)
            {
                Debug.LogError($"hazardSpawns is empty in {_difficultyController.CurrentDifficulty}");
                return;
            }

            foreach (var hazardSpawnModel in _hazardSpawns)
            {
                if (!hazardSpawnModel)
                {
                    Debug.LogError($"hazardSpawnModel is null in {_difficultyController.CurrentDifficulty}");
                    continue;
                }

                _roundHazardSpawnChain
                    .AddAction(() =>
                    {
                        _hazardSpawnModel = hazardSpawnModel;
                        _hazardLimitCount = hazardSpawnModel.limit.Random();
                    })
                   .AddAction(SpawnHazardWave, hazardSpawnModel)
                   .AddAwait(WaveUnlockAwait)
                ;
            }

            _roundHazardSpawnChain.AddAction(HazardSpawnLoop);
        }

        private void SpawnHazardWave(HazardSpawnModel spawnModel)
        {
            _lockSpawn = true;

            _spawnHazardWaveChain = Planner.Chain()
                .AddAwait(FreeLimitAwait)
                .AddTimeout(spawnModel.timeout.Random())
                .AddAction(SpawnHazards, spawnModel)
                ;
        }

        private void SpawnHazards(HazardSpawnModel spawnModel)
        {
            _spawnHazardsChain = Planner.Chain();

            var interval = spawnModel.interval.Random();
            var intervalFinal = spawnModel.count > 1 ? interval / (spawnModel.count - 1) : interval;
            var diffCount = spawnModel.limit.Random() - _hazardActiveCount;
            var allowedCount = (int)Mathf.Min(diffCount, spawnModel.count);


            //TODO: rework this
            CheckForFreeEnemySpots(ref allowedCount);
            var remedyModel = spawnModel.hazardType as RemedyModel;
            if (remedyModel)
            {
                CheckForRemedy(remedyModel, ref allowedCount);
            }


            var roundRestCount = _gameRound.hazardCount - _hazardKilledCount;
            var allowedCountFinal = Mathf.Min(allowedCount, roundRestCount);
            _hazardActiveCount += allowedCountFinal;
            _hazardActiveCountStamp = _hazardActiveCount;

            if (!_isReachedRoundHazardCount && _hazardActiveCountStamp >= roundRestCount)
            {
                _isReachedRoundHazardCount = true;
            }

            for (int i = 0; i < allowedCountFinal; i++)
            {
                _spawnHazardsChain
                    .AddAction(GetHazardMethodCreation(spawnModel))
                    .AddTimeout(intervalFinal)
                    ;
            }

            _spawnHazardsChain.AddAction(() => _lockSpawn = false);
        }

        private Action GetHazardMethodCreation(HazardSpawnModel spawnModel)
        {
            Action result = null;

            switch (spawnModel.hazardType)
            {
                case CometModel _:
                    {
                        var model = (CometModel)spawnModel.hazardType;
                        result = () => _spawnSystem.GetSpawner(HazardType.Comet).Spawn(model, OnHazardDestroy);
                        break;
                    }

                case EnemyModel _:
                    {
                        var model = (EnemyModel)spawnModel.hazardType;
                        result = () => _spawnSystem.GetSpawner(HazardType.Enemy).Spawn(model, OnHazardDestroy);
                        break;
                    }

                case RemedyModel _:
                    {
                        var model = (RemedyModel)spawnModel.hazardType;
                        result = () => _spawnSystem.GetSpawner(HazardType.Remedy).Spawn(model, OnHazardDestroy);
                        break;
                    }
            }

            return result;
        }

        private void OnHazardDestroy(bool byPlayer)
        {
            --_hazardActiveCount;

            if (!byPlayer || _hazardKilledCount >= _gameRound.hazardCount)
            {
                return;
            }

            ++_hazardKilledCount;

            _messageBus.HazardDestroyed.Send(null);
        }

        private SpawnSystemBuilder.SpawnSystem CreateSpawnSystem()
        {
            var builder = new SpawnSystemBuilderDefault();
            _spawnSystemDirector.CreateSpawnSystemDefault(builder);

            return builder.GetSystem();
        }

        private void CheckForRemedy(RemedyModel remedyModel, ref int count)
        {
            if (_gameController.HealthPanel.IsDamaged() && SRandom.IsLuckyChanceWithPercent(remedyModel.spawnChancePercent))
            {
                return;
            }

            count = 0;
        }

        private void CheckForFreeEnemySpots(ref int count)
        {
            count = Mathf.Min(count, _gameRoundEnemySpots.GetFreeSpots().Count());
        }

        #region Awaiters
        private void GameStartAwait(AsyncStateInfo state)
        {
            state.IsComplete = _gameController.IsGameRun;
        }

        private void SettingsAvailableAwait(AsyncStateInfo state)
        {
            state.IsComplete = _difficultyController.CurrentDifficulty != null;
        }

        private void FreeLimitAwait(AsyncStateInfo state)
        {
            state.IsComplete = _hazardActiveCount < _hazardLimitCount;
        }

        private void WaveUnlockAwait(AsyncStateInfo state)
        {
            state.IsComplete = !_lockSpawn;
        }

        public void RoundLastWaveDestroyAwait(AsyncStateInfo state)
        {
            state.IsComplete = _hazardActiveCount <= 0;
        }
        #endregion
    }
}