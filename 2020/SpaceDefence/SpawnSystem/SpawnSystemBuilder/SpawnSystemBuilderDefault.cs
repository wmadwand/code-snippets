using MiniGames.Games.SpaceDefence.Hazard.Enemy;
using MiniGames.Games.SpaceDefence.Hazard.Projectile;
using MiniGames.Games.SpaceDefence.SpawnSystem.HazardSpawners;

namespace MiniGames.Games.SpaceDefence.SpawnSystem.SpawnSystemBuilder
{
    public class SpawnSystemBuilderDefault : SpawnSystemBuilder
    {
        private SpawnSystem _system;
        private SpawnSystemHelper _helper;
        private EnemySpot[] _enemySpots;

        public SpawnSystemBuilderDefault()
        {
            _system = null;            
        }

        public override void BuildSystem()
        {
            _system = new SpawnSystem();
            _helper = new SpawnSystemHelper();
        }

        public override void BuildCometSpawner(ProjectileFactory factory)
        {
            _system.AddSpawner(HazardType.Comet, new CometSpawner(factory, _helper));
        }

        public override void BuildEnemySpawner(Enemy.Factory factory)
        {
            _system.AddSpawner(HazardType.Enemy, new EnemySpawner(factory, _helper));
        }

        public override void BuildRemedySpawner(ProjectileFactory factory)
        {
            _system.AddSpawner(HazardType.Remedy, new RemedySpawner(factory, _helper));
        }

        public override SpawnSystem GetSystem()
        {
            return _system;
        }
    }
}