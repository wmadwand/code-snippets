using System;
using System.Collections.Generic;
using Coconut.Asyncs;
using MiniGames.Games.SpaceDefence.Data;
using MiniGames.Games.SpaceDefence.Hazard.Projectile;
using MiniGames.Games.SpaceDefence.SpawnSystem.SpawnSystemBuilder;

namespace MiniGames.Games.SpaceDefence.SpawnSystem.HazardSpawners
{
    public class CometSpawner : IHazardSpawner
    {
        public int HazardCount => _collection.Count;
                
        public SpawnSystemHelper SpawnHelper { get; private set; }
        private ProjectileFactory _projectileFactory;
        private List<IProjectile> _collection = new List<IProjectile>();
        private Action<bool> _onDestroyCallback;

        //-------------------------------------------------

        public CometSpawner(ProjectileFactory projectileFactory, SpawnSystemHelper spawnHelper)
        {
            _projectileFactory = projectileFactory;
            SpawnHelper = spawnHelper;
        }

        public void Spawn(HazardModelBase model, Action<bool> onDestroyCallback)
        {
            _onDestroyCallback = onDestroyCallback;

            var randomPath = SpawnHelper.GetRandomPath();
            var comet = _projectileFactory.Create(HazardType.Comet);
            var modelExplicit = (CometModel)model;
            var direction = randomPath.Direction;
            var speed = modelExplicit.speed.Random();

            comet.SetStartPosition(randomPath.start);
            comet.Init(direction, speed, OnDestroy);

            _collection.Add(comet);
        }

        //-------------------------------------------------

        private void OnDestroy(ProjectileBase comet, bool byPlayer)
        {
            _collection.Remove(comet);
            
            _onDestroyCallback?.Invoke(byPlayer);

            Planner.Chain(comet.gameObject)
                .AddAction(() => comet.gameObject.SetActive(false))
                .AddTimeout(0.5f)
                .AddAction(() => comet.Destroy())
                ;
        }
    }
}