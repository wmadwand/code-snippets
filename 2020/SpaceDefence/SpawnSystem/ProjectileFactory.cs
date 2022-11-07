using MiniGames.Games.SpaceDefence.Hazard.Projectile;
using Zenject;

namespace MiniGames.Games.SpaceDefence.SpawnSystem
{
    public class ProjectileFactory
    {
        private ProjectileBase.Factory _cometFactory;
        private ProjectileBase.Factory _remedyFactory;
        private ProjectileBase.Factory _projectileFactory;

        public ProjectileFactory([Inject(Id = HazardType.Projectile)] ProjectileBase.Factory projectileFactory,
                                 [Inject(Id = HazardType.Comet)] ProjectileBase.Factory cometFactory,
                                 [Inject(Id = HazardType.Remedy)] ProjectileBase.Factory remedyFactory)
        {
            _projectileFactory = projectileFactory;
            _cometFactory = cometFactory;
            _remedyFactory = remedyFactory;
        }

        public virtual IProjectile Create(HazardType type)
        {
            IProjectile instance = null;

            switch (type)
            {
                case HazardType.Projectile: instance = _projectileFactory.Create(); break;
                case HazardType.Comet: instance = _cometFactory.Create(); break;
                case HazardType.Remedy: instance = _remedyFactory.Create(); break;
                default: break;
            }

            return instance;
        }
    }
}