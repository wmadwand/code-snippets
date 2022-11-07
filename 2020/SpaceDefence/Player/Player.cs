using MiniGames.Games.SpaceDefence.Core.MessageBus;
using MiniGames.Games.SpaceDefence.Game;
using MiniGames.Games.SpaceDefence.Hazard;
using MiniGames.Games.SpaceDefence.Hazard.Projectile;
using UnityEngine;
using Zenject;

namespace MiniGames.Games.SpaceDefence.Player
{
    public class Player : MonoBehaviour
    {
        [Inject] private PlayerHealth _health;
        [Inject] private MessageBus _messageBus;
        [Inject] private SpaceDefenceSoundController _soundController;

        //-------------------------------------------------

        private void OnPlayerGetHealth(int obj, Transform tr)
        {
            _health.AddHealth(1);
        }

        private void Awake()
        {
            _messageBus.PlayerGetHealth.Receive += OnPlayerGetHealth;
        }

        private void OnDestroy()
        {
            _messageBus.PlayerGetHealth.Receive -= OnPlayerGetHealth;
        }

        private void OnTriggerEnter(Collider other)
        {
            var projectile = other.GetComponent<IProjectile>();

            if (projectile != null)
            {
                if (projectile.IsCollectible)
                {
                    projectile.Stop();
                    other.GetComponent<Remedy>().DestroyAfterBlink();

                    return;
                }

                _health.GetDamage(1);
                _messageBus.PlayerDamaged.Send(_health.Value);
                _soundController.Play(AudioName.PlayerDamaged);

                other.GetComponent<ProjectileBase>().SendOnDestroy();
                other.GetComponent<ProjectileBase>().CreateExplosion(other.transform.position);
            }
        }
    }
}