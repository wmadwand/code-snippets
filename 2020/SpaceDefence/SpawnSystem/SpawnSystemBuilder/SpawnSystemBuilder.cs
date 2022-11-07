using MiniGames.Games.SpaceDefence.Hazard.Enemy;

namespace MiniGames.Games.SpaceDefence.SpawnSystem.SpawnSystemBuilder
{
    public abstract class SpawnSystemBuilder
    {
        protected SpawnSystemBuilder()
        {
        }

        public virtual void BuildSystem() { }

        public virtual void BuildCometSpawner(ProjectileFactory factory) { }
        public virtual void BuildEnemySpawner(Enemy.Factory factory) { }
        public virtual void BuildRemedySpawner(ProjectileFactory factory) { }

        public virtual SpawnSystem GetSystem() { return null; }
    }
}