using Coconut.Asyncs;
using MiniGames.Games.SpaceDefence.Game;
using MiniGames.Games.SpaceDefence.Hazard.Projectile;
using UnityEngine;

using Random = UnityEngine.Random;

namespace MiniGames.Games.SpaceDefence.Hazard
{
    public sealed class Comet : ProjectileBase
    {
        [SerializeField] private float _tumbleVal = 5;
        [SerializeField] private Transform view;
        [SerializeField] private ParticleSystem fx;

        private Vector3 _rotationVelocity = Vector3.zero;

        //-------------------------------------------------

        public override void Reflect(float speed)
        {
            SetReflected();
            Explosion();

            Stop();
            SendOnDestroy(true);
        }

        public override void Stop()
        {
            base.Stop();
            fx.Stop();
            view.gameObject.SetActive(false);
        }

        public override void StartOperations()
        {
            base.StartOperations();
            view.gameObject.SetActive(true);
            fx.Play();
            _rotationVelocity = Random.insideUnitSphere * _tumbleVal;
            _soundController.Play(AudioName.CometAppear);
        }

        private void Update()
        {
            var rotationX = Quaternion.AngleAxis(_rotationVelocity.x * Time.deltaTime, Vector3.left);
            var rotationY = Quaternion.AngleAxis(_rotationVelocity.y * Time.deltaTime, Vector3.up);
            var rotationZ = Quaternion.AngleAxis(_rotationVelocity.z * Time.deltaTime, Vector3.forward);
            view.rotation *= rotationX*rotationY*rotationZ;
        }
    }
}