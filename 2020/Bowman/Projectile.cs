using System;
using UnityEngine;
using Zenject;

namespace MiniGames.Games.Bowman
{
    public class Projectile : MonoBehaviour
    {
        //[SerializeField] private Transform _centerOfMass;

        [Inject] private BowmanSettings _gameSettings;

        private Rigidbody2D _rigidbody;
        private bool isShooting;
        private Action<Projectile> _onDestroy;
        private MeshRenderer _renderer;
        private Plane[] _planes;

        //---------------------------------------------------

        public void Init(Action<Projectile> callback)
        {
            _onDestroy = callback;
            _rigidbody.gravityScale = 0;
        }

        public void Throw()
        {
            _rigidbody.AddForce(Vector2.right * _gameSettings.projectileSpeed, ForceMode2D.Force);
        }

        public void SetVelocity(Vector3 velocity)
        {
            transform.SetParent(transform.root);

            velocity *= _gameSettings.projectileSpeed;
            //_rigidbody.centerOfMass = centerOfMass.position;
            _rigidbody.gravityScale = 1;
            _rigidbody.velocity = velocity;

            isShooting = true;
        }

        public void Simulate()
        {
            _rigidbody.simulated = true;
        }

        //---------------------------------------------------

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            //_rigidbody.isKinematic = true;
            _renderer = GetComponentInChildren<MeshRenderer>();
        }

        private void Start()
        {
            _planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        }

        private void Update()
        {
            if (!isShooting)
            {
                return;
            }

            transform.up = Vector3.Slerp(transform.up, _rigidbody.velocity.normalized, Time.deltaTime / _gameSettings.projectileRotateRate);

            if (!IsOBjectInViewPort())
            {
                _onDestroy?.Invoke(this);
            }
        }

        public void SelfDestroy()
        {
            _onDestroy?.Invoke(this);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (isShooting && collision.gameObject.GetComponent<Target>())
            {
                Stop(collision.gameObject.GetComponent<Target>().Content);
            }
        }

        private void Stop(Transform target)
        {
            _rigidbody.simulated = false;
            _rigidbody.velocity = Vector3.zero;
            //_rigidbody.angularVelocity = 0;
            isShooting = false;

            transform.SetParent(target);
        }

        private bool IsOBjectInViewPort()
        {
            return GeometryUtility.TestPlanesAABB(_planes, _renderer.bounds);
        }

        //---------------------------------------------------

        public class Factory : PlaceholderFactory<Projectile> { }
    }
}