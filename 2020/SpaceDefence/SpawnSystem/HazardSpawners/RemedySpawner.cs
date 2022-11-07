using System;
using System.Collections.Generic;
using MiniGames.Games.SpaceDefence.Data;
using MiniGames.Games.SpaceDefence.Hazard.Projectile;
using MiniGames.Games.SpaceDefence.SpawnSystem.SpawnSystemBuilder;

namespace MiniGames.Games.SpaceDefence.SpawnSystem.HazardSpawners
{
    public class RemedySpawner : IHazardSpawner
    {
        public int HazardCount => _collection.Count;
                
        private SpawnSystemHelper _spawnHelper;
        private ProjectileFactory _projectileFactory;
        private List<IProjectile> _collection = new List<IProjectile>();
        private Action<bool> _onDestroyCallback;

        //-------------------------------------------------

        public RemedySpawner(ProjectileFactory projectileFactory, SpawnSystemHelper spawnHelper)
        {
            _projectileFactory = projectileFactory;
            _spawnHelper = spawnHelper;
        }

        public void Spawn(HazardModelBase model, Action<bool> onDestroyCallback)
        {
            _onDestroyCallback = onDestroyCallback;

            var remedyModel = (RemedyModel)model;
            var randomPath = _spawnHelper.GetRandomPath();
            var remedy = _projectileFactory.Create(HazardType.Remedy);
            var direction = randomPath.Direction;
            var speed = remedyModel.speed.Random();

            remedy.SetStartPosition(randomPath.start);
            remedy.Init(direction, speed, OnDestroy);

            _collection.Add(remedy);
        }

        //-------------------------------------------------

        private void OnDestroy(ProjectileBase remedy, bool byPlayer)
        {
            _collection.Remove(remedy);
            remedy.Destroy();

            _onDestroyCallback?.Invoke(false);
        }
    }
}