using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace MiniGames.Games.Bowman.Shooting
{
    public class WeaponShooting : MonoBehaviour
    {
        [Inject] Projectile.Factory _projectileFactory;

        private WeaponHelper _helper;
        private Projectile _projectile;
        private readonly List<Projectile> _firedProjectiles = new List<Projectile>();

        //---------------------------------------------------

        public void CreateProjectile()
        {
            _projectile = _projectileFactory.Create();
            _projectile.transform.SetParent(_helper.ProjectileHolder, false);
            _projectile.Init(OnProjectileDestroy);
        }

        public void FireProjectile(Vector3 velocity)
        {
            _projectile.SetVelocity(velocity);
            _firedProjectiles.Add(_projectile);
        }

        public void DestroyFiredProjectiles()
        {
            foreach (var item in _firedProjectiles)
            {
                Destroy(item.gameObject);
            }

            _firedProjectiles.Clear();
        }

        //---------------------------------------------------

        private void Awake()
        {
            _helper = GetComponentInChildren<WeaponHelper>();
        }

        private void OnProjectileDestroy(Projectile projectile)
        {
            _firedProjectiles.Remove(projectile);
            Destroy(projectile.gameObject);
        }
    }
}