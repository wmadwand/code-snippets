using MiniGames.Games.SpaceDefence.Core.MessageBus;
using MiniGames.Games.SpaceDefence.Difficulties;
using MiniGames.Games.SpaceDefence.Effects;
using MiniGames.Games.SpaceDefence.Hazard.Enemy;
using MiniGames.Games.SpaceDefence.Hazard.Projectile;
using MiniGames.Games.SpaceDefence.SpawnSystem;
using MiniGames.Games.SpaceDefence.SpawnSystem.SpawnSystemBuilder;
using MiniGames.Games.Ship.Services;
using UnityEngine;
using Zenject;

namespace MiniGames.Games.SpaceDefence.Game
{
    public class SpaceDefenceGameInstaller : MonoInstaller
    {
        [SerializeField] private SpaceDefenceDifficultyController _difficultyController;
        [SerializeField] private SpaceDefenceGameController _gameController;

        [SerializeField] private GameObject _enemyTemplate;
        [SerializeField] private GameObject _projectileTemplate;
        [SerializeField] private GameObject _cometTemplate;
        [SerializeField] private GameObject _remedyTemplate;
        [SerializeField] private GameObject _explosionTemplate;

        //-------------------------------------------------

        public override void InstallBindings()
        {
            Container.Bind<MessageBus>().AsSingle();
            Container.Bind<SpawnSystemDirector>().AsSingle();
            Container.Bind<ProjectileFactory>().AsSingle();
            Container.Bind<SpaceDefenceSoundController>().AsSingle();

            Container.BindInstance(_difficultyController).IfNotBound();
            Container.BindInstance(_gameController).IfNotBound();

            Container.BindFactory<Vector3, Enemy, Enemy.Factory>()
                     .FromPoolableMemoryPool<Vector3, Enemy, EnemyPool>(poolBinder => poolBinder
                     .WithInitialSize(5)
                     .FromComponentInNewPrefab(_enemyTemplate)
                     .UnderTransformGroup("Enemies"));

            Container.BindFactory<ProjectileBase, ProjectileBase.Factory>().WithId(HazardType.Projectile)
                     .FromComponentInNewPrefab(_projectileTemplate)
                     .UnderTransformGroup("Hazards");

            Container.BindFactory<ProjectileBase, ProjectileBase.Factory>().WithId(HazardType.Comet)
                     .FromComponentInNewPrefab(_cometTemplate)
                     .UnderTransformGroup("Hazards");

            Container.BindFactory<ProjectileBase, ProjectileBase.Factory>().WithId(HazardType.Remedy)
                     .FromComponentInNewPrefab(_remedyTemplate)
                     .UnderTransformGroup("Hazards");

            Container.BindFactory<Explosion, Explosion.Factory>()
                     .FromPoolableMemoryPool<Explosion, ExplosionPool>(poolBinder => poolBinder
                     .WithInitialSize(5)
                     .FromComponentInNewPrefab(_explosionTemplate)
                     .UnderTransformGroup("Explosions"));
        }

        //-------------------------------------------------

        class EnemyPool : MonoPoolableMemoryPool<Vector3, IMemoryPool, Enemy> { }

        class ExplosionPool : MonoPoolableMemoryPool<IMemoryPool, Explosion> { }
    }
}