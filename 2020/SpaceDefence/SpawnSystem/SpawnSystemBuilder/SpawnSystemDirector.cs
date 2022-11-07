using MiniGames.Games.SpaceDefence.Hazard.Enemy;
using Zenject;

namespace MiniGames.Games.SpaceDefence.SpawnSystem.SpawnSystemBuilder
{
    public class SpawnSystemDirector
    {
        [Inject] private ProjectileFactory _projectileFactory;
        [Inject] private Enemy.Factory _enemyFactory;        

        public SpawnSystem CreateSpawnSystemDefault(SpawnSystemBuilder builder)
        {
            builder.BuildSystem();
            builder.BuildCometSpawner(_projectileFactory);
            builder.BuildEnemySpawner(_enemyFactory);
            builder.BuildRemedySpawner(_projectileFactory);

            return builder.GetSystem();
        }

        public SpawnSystem CreateSpawnSystemHazardsOnly(SpawnSystemBuilder builder)
        {
            builder.BuildSystem();
            builder.BuildCometSpawner(_projectileFactory);
            builder.BuildEnemySpawner(_enemyFactory);            

            return builder.GetSystem();
        }
    } 
}