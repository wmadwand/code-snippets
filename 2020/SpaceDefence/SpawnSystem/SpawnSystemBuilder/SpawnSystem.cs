using System;
using System.Collections.Generic;
using MiniGames.Games.SpaceDefence.Data;
using MiniGames.Games.SpaceDefence.Hazard.Projectile;

namespace MiniGames.Games.SpaceDefence.SpawnSystem.SpawnSystemBuilder
{
    public class SpawnSystem
    {
        private Dictionary<HazardType, IHazardSpawner> _spawners;

        public SpawnSystem()
        {
            _spawners = new Dictionary<HazardType, IHazardSpawner>();
        }

        public void AddSpawner(HazardType type, IHazardSpawner spawner)
        {
            _spawners[type] = spawner;
        }

        public IHazardSpawner GetSpawner(HazardType type)
        {
            _spawners.TryGetValue(type, out IHazardSpawner value);

            return value;
        }
    }

    public interface IHazardSpawner
    {
        int HazardCount { get; }
        void Spawn(HazardModelBase model, Action<bool> onHazardDestroyCallback);
    }
}