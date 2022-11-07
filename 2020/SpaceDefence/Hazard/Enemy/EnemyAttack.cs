using MiniGames.Games.SpaceDefence.Data;
using MiniGames.Games.SpaceDefence.Hazard.Projectile;
using MiniGames.Games.SpaceDefence.SpawnSystem;
using UnityEngine;
using Zenject;

namespace MiniGames.Games.SpaceDefence.Hazard.Enemy
{
    public class EnemyAttack : MonoBehaviour
    {
        public bool IsReady => IsGreenLight && Time.time >= _nextTimeShot;
        public bool IsGreenLight { get; set; }
                
        [SerializeField] private Transform shellSpawnPoint;
        [Inject] private ProjectileFactory _projectileFactory;

        private float _nextTimeShot;
        private EnemyModel _enemyModel;

        //-------------------------------------------------

        public void Init(EnemyModel enemyModel)
        {
            _enemyModel = enemyModel;
            Reset();
        }

        public void Reset()
        {
            _nextTimeShot = Time.time + _enemyModel.afterAttackTime.Random();
            IsGreenLight = true;
        }

        public void Shoot()
        {
            IsGreenLight = false;

            var obj = _projectileFactory.Create(HazardType.Projectile);
            obj.SetStartPosition(shellSpawnPoint.position);
            var projectileSpeed = _enemyModel.projectileSpeed.Random();
            obj.Init(transform.up, projectileSpeed, OnProjectileDestroyed);
        }

        //-------------------------------------------------

        private void OnProjectileDestroyed(ProjectileBase projectile, bool byPlayer)
        {
            _nextTimeShot = Time.time + _enemyModel.afterAttackTime.Random();
            IsGreenLight = true;

            Destroy(projectile.gameObject);
        }
    }
}