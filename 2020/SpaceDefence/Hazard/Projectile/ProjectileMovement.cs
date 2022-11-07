using UnityEngine;

namespace MiniGames.Games.SpaceDefence.Hazard.Projectile
{
    public class ProjectileMovement : MonoBehaviour
    {
        private Rigidbody _rigidbody;

        //-------------------------------------------------

        public void SetVelocity(Vector3 attackDirection, float speed)
        {
            _rigidbody.AddForce(-attackDirection * speed, ForceMode.Force);
        }

        public void SetVelocityOpposite(float speed)
        {
            var opposite = -_rigidbody.velocity;

            _rigidbody.AddForce(opposite.normalized * speed, ForceMode.Force);
        }

        public void SetVelocityOppositeMirror(float speed)
        {
            var opposite = -_rigidbody.velocity;
            opposite.x *= -1;

            _rigidbody.AddForce(opposite.normalized * speed, ForceMode.Force);
        }

        public void Stop()
        {
            _rigidbody.velocity = Vector3.zero;
        }

        //-------------------------------------------------

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }
    }
}