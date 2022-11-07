using DG.Tweening;
using Coconut.Core.Asyncs;
using Coconut.Game;
using System;
using UnityEngine;
using Utils;

namespace MiniGames.Games.Bowman
{
    public class Target : MonoBehaviour
    {
        public int Number { get; private set; }
        public Transform Content => _content;

        [SerializeField] private Transform _content;
        [SerializeField] private ShootSoundOnEvent _hitSound;

        private Action<Target> _onHit;
        private TargetMovement _movement;
        private TargetView _view;
        private Projectile _hitProjectile;

        //---------------------------------------------------

        public void Init(Harmonic settings, Action<Target> callback)
        {
            _movement.Init(settings);
            _onHit = callback;
        }

        public void StartMove()
        {
            _movement.StartMove();
        }

        public void StopMove()
        {
            _movement.StopMove();
        }

        public void SetNumber(int number)
        {
            Number = number;
            _view.SetNumber(number.ToString());
        }

        public Tween ShowNumber()
        {
            return _view.ShowNumber();
        }

        public void ResetView()
        {
            _view.ResetView();
        }

        public void DestroyProjectile()
        {
            _hitProjectile?.SelfDestroy();
        }

        public async Promise Incorrect(int newTargetNumber)
        {
            //await _view.Incorrect();
            //_projectile?.SelfDestroy();
            //_hitProjectile?.SelfDestroy();
            //ResetView();
            SetNumber(newTargetNumber);            

            //await ShowNumber();
        }

        //---------------------------------------------------

        private void Awake()
        {
            _movement = GetComponent<TargetMovement>();
            _view = GetComponent<TargetView>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            _hitProjectile = collision.gameObject.GetComponent<Projectile>();

            if (_hitProjectile)
            {
                _onHit?.Invoke(this);
                _hitSound.Play();
            }
        }
    }
}