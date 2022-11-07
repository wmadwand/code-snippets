using Coconut.Core.Asyncs;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;
using Zenject;

namespace MiniGames.Games.Bowman.Shooting
{
    public class Weapon : MonoBehaviour, IWeapon
    {
        [Header("Audio")] [SerializeField] private ShootSoundOnEvent _flyArrowSound;
        [SerializeField] private ShootSoundOnEvent _shootSound;

        public event Action<float> changeRotation;

        private WeaponMovement _movement;
        private WeaponShooting _shooting;

        //---------------------------------------------------
        public bool IsPulledEnough()
        {
            return _movement.IsPulledEnough();
        }

        public void SetRotation(float angle)
        {
            _movement.SetRotation(angle);
        }

        public void SetPull(float pull)
        {
            _movement.SetPull(pull);
        }

        public void SetDistance(float distance)
        {
            _movement.SetDistance(distance);
        }

        public void Move(PointerEventData eventData)
        {
            _movement.Move(eventData);
        }

        public void Fire(Action<bool> callback)
        {
            gameObject.CancelPromise<Action<bool>>(FireAsync);
            gameObject.Promise(FireAsync, callback);
        }

        public void DestroyFiredProjectiles()
        {
            _shooting.DestroyFiredProjectiles();
        }

        //---------------------------------------------------

        private void Awake()
        {
            _movement = GetComponent<WeaponMovement>();
            _shooting = GetComponent<WeaponShooting>();
            _movement.changeRotation += MovementOnChangeRotation;
        }

        private void MovementOnChangeRotation(float angle)
        {
            changeRotation?.Invoke(angle);
        }

        private void Start()
        {
            //_shooting.CreateProjectile();
        }

        private async Promise FireAsync(Action<bool> callback)
        {
            if (!_movement.IsPulledEnough())
            {
                callback(false);
                _movement.ResetPosition(.5f);
                return;
            }

            _shootSound.Play();
            _flyArrowSound.Play();


            var velocity = _movement.GetPulledVelocity();
            _shooting.CreateProjectile();
            _shooting.FireProjectile(velocity);
            callback(true);

            _movement.ResetPosition(.5f);

            _flyArrowSound.Stop();
        }
    }

    public interface IWeapon
    {
        void Move(PointerEventData eventData);
        void Fire(Action<bool> callback);
        void DestroyFiredProjectiles();
    }
}