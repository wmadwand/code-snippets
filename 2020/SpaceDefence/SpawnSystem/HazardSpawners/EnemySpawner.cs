using System;
using System.Collections.Generic;
using MiniGames.Games.SpaceDefence.Data;
using MiniGames.Games.SpaceDefence.Hazard.Enemy;
using MiniGames.Games.SpaceDefence.SpawnSystem.SpawnSystemBuilder;

namespace MiniGames.Games.SpaceDefence.SpawnSystem.HazardSpawners
{
    public class EnemySpawner : IHazardSpawner
    {
        public int HazardCount => _collection.Count;

        private SpawnSystemHelper _spawnHelper;
        private Action<bool> _onDestroyCallback;
        private EnemySpotCollection _enemySpots;
        private List<Enemy> _collection = new List<Enemy>();
        private Enemy.Factory _enemyFactory;

        //-------------------------------------------------

        public EnemySpawner(Enemy.Factory enemyFactory, SpawnSystemHelper spawnHelper)
        {
            _enemyFactory = enemyFactory;
            _spawnHelper = spawnHelper;
        }

        public void Spawn(HazardModelBase model, Action<bool> onDestroyCallback)
        {
            _onDestroyCallback = onDestroyCallback;

            var freeSpot = _enemySpots.GetRandomFreeSpot();

            if (!freeSpot)
            {
                return;
            }

            var startPosition = _spawnHelper.GetRandomPoint(SpawnSystemHelper.PointPosition.Top);
            freeSpot.SetEnemyStartPosition(startPosition);

            var enemy = CreateEnemy((EnemyModel)model, freeSpot);
            _collection.Add(enemy);
            freeSpot.SetLockedWith(enemy);
        }

        public void SetEnemySpots(EnemySpotCollection collection)
        {
            _enemySpots = collection;
        }

        //-------------------------------------------------

        private Enemy CreateEnemy(EnemyModel model, EnemySpot spot)
        {
            var enemy = _enemyFactory.Create(spot.EnemyStartPosition);
            enemy.Init(spot, model, _spawnHelper, OnDestroy);

            return enemy;
        }

        private void OnDestroy(EnemySpot enemySpot, bool byPlayer = false)
        {
            _collection.Remove(enemySpot.Enemy);
            enemySpot.SetUnlocked();

            _onDestroyCallback?.Invoke(byPlayer);
        }
    }
}