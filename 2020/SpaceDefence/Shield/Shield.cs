using MiniGames.Games.SpaceDefence.Core.MessageBus;
using MiniGames.Games.SpaceDefence.Game;
using MiniGames.Games.SpaceDefence.Hazard;
using MiniGames.Games.SpaceDefence.Hazard.Projectile;
using SpaceDefenceGeometry;
using System;
using UnityEngine;
using Zenject;

namespace MiniGames.Games.SpaceDefence.Shield
{
    public sealed class Shield : MonoBehaviour
    {
        public ShieldMovement Movement { get; private set; }

        [Inject] private MessageBus _messageBus;
        [Inject] private SpaceDefenceGameController _gameController;
        [Inject] private Settings _settings;
        [Inject] private SpaceDefenceSoundController _soundController;

        private readonly float _xRotation = -4f;
        private BezierCurvesFilter _splineFilter;
        private AudioName _soundName;

        //-------------------------------------------------

        private void Awake()
        {
            Movement = GetComponent<ShieldMovement>();

            _splineFilter = new BezierCurvesFilter(new[] { _gameController.ShieldSpline });
        }

        private void Update()
        {
            Debug.DrawRay(transform.position, -transform.up, Color.cyan);

            _splineFilter.Projection(transform.position, out BezierCurvesProjection projection);
            transform.position = projection.projection;

            var rot = Quaternion.FromToRotation(Vector3.down, projection.Normal(Vector3.forward));
            var rot2 = Quaternion.Euler(_xRotation, rot.eulerAngles.y, rot.eulerAngles.z);
            transform.rotation = rot2;
        }

        private void OnTriggerEnter(Collider other)
        {
            var projectile = other.GetComponent<IProjectile>();

            if (projectile != null)
            {
                if (projectile.IsCollectible)
                {
                    _messageBus.PlayerGetHealth.Send(1, other.transform);

                    projectile.SendOnDestroy(false);

                    other.GetComponent<ProjectileBase>().CreateExplosion(other.transform.position);
                }
                else
                {
                    projectile.Reflect(_settings.reflectionSpeed);                    
                }

                PlaySound(projectile);
            }
        }

        private void PlaySound(IProjectile projectile)
        {
            switch (projectile)
            {
                case Projectile _:
                    _soundName = AudioName.ShieldReflectProjectile;
                    break;

                case Comet _:
                    _soundName = AudioName.ShieldReflectComet;
                    break;

                case Remedy _:
                    _soundName = AudioName.ShieldReflectRemedy;
                    break;

                default:
                    break;
            }

            _soundController.Play(_soundName);
        }

        //-------------------------------------------------

        [Serializable]
        public class Settings
        {
            public float reflectionSpeed;
        }
    }
}