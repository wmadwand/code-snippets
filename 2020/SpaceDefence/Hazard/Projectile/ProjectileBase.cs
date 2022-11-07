using System;
using MiniGames.Games.SpaceDefence.Difficulties;
using MiniGames.Games.SpaceDefence.Effects;
using MiniGames.Games.SpaceDefence.Game;
using UnityEngine;
using Zenject;

namespace MiniGames.Games.SpaceDefence.Hazard.Projectile
{
    [RequireComponent(typeof(ProjectileMovement))]
    public abstract class ProjectileBase : MonoBehaviour, IProjectile
    {
        public virtual bool IsCollectible => false;
        public bool IsReflected { get; private set; }

        [SerializeField] protected GameObject _explosionTemplate;
        [SerializeField] protected float _explosionOffset;

        [Inject] protected SpaceDefenceDifficultyController _difficultyController;
        [Inject] protected Explosion.Factory _explosionFactory;
        [Inject] protected SpaceDefenceSoundController _soundController;

        protected Collider _collider;
        protected ProjectileMovement _movement;
        protected Action<ProjectileBase, bool> _onDestroy;

        //-------------------------------------------------

        public void Init(Vector3 direction, float speed, Action<ProjectileBase, bool> callback)
        {
            _onDestroy = callback;
            _movement.SetVelocity(direction, speed);

            //TODO: rework this
            if (IsCollectible)
            {
                return;
            }
            
            transform.LookAt(transform.position + direction, Vector3.forward);
        }

        public void SetReflected()
        {
            IsReflected = true;
        }

        public virtual void Reflect(float speed)
        {
            SetReflected();
            _movement.SetVelocityOpposite(speed);
        }

        public void CreateExplosion(Vector3 position)
        {
            Instantiate(_explosionTemplate, position, Quaternion.identity);
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }

        public void SendOnDestroy(bool byPlayer = false)
        {
            _onDestroy?.Invoke(this, byPlayer);
        }

        public virtual void Stop()
        {
            _movement.Stop();
        }

        public virtual void Explosion()
        {
            var rigidbody = GetComponent<Rigidbody>();
            var offset = rigidbody.velocity.normalized * _explosionOffset;
            CreateExplosion(transform.position + offset);
        }

        public void SetStartPosition(Vector3 value)
        {
            transform.SetPositionAndRotation(value, Quaternion.identity);
        }

        public virtual void StartOperations() { }
        public virtual void AwakeOperations() { }

        //-------------------------------------------------

        private void Awake()
        {
            _movement = GetComponent<ProjectileMovement>();
            _collider = GetComponent<Collider>();

            AwakeOperations();
        }

        private void Start()
        {
            StartOperations();
        }

        private void OnTriggerEnter(Collider other)
        {
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponent<SafeArea>())
            {
                SendOnDestroy(true);
            }
        }

        //-------------------------------------------------

        public class Factory : PlaceholderFactory<ProjectileBase> { }
    }

    //-------------------------------------------------

    public enum HazardType
    {
        Enemy = 10,
        Comet = 20,
        Projectile = 30,
        Remedy = 40
    }

    [Serializable]
    public class ProjectileSettings
    {
        public float speed;
    }
}