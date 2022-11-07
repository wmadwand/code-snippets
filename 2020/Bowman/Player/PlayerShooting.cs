using System;
using Coconut.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace MiniGames.Games.Bowman.Shooting
{
    public class PlayerShooting : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public IWeapon Weapon => _weapon;

        [SerializeField] private string _cameraTag = "MainCamera";
        [SerializeField] private Weapon _weapon;
        [SerializeField] private PlayerAnimations _playerAnimations;
        [SerializeField] private PlayerAngle _playerAngle;
        [SerializeField] private Transform _rotationCenter;
        [SerializeField] private BoxCollider2D _dragZone;
        [SerializeField] private RangeFloat _pullRange = new RangeFloat(1, 4);
        [SerializeField] private float _pullPower = 0.125f;
        [SerializeField] private RangeFloat _rotationRange = new RangeFloat(-45, 45);

        [Inject] private BowmanSettings _gameSettings;
        private Camera _camera;
        private float _nextShotTime;
        private float? _multiplier;

        //---------------------------------------------------

        private void Awake()
        {
            _camera = GameObjectLookup.ByTag<Camera>(_cameraTag);
            _weapon.changeRotation += WeaponOnChangeRotation;
        }

        private void WeaponOnChangeRotation(float rotation)
        {
            _playerAngle.rotation = rotation;
        }

        public void SetActive(bool value)
        {
            _dragZone.enabled = value;
        }

        //---------------------------------------------------

        private Vector3 CalcWorldPoint(PointerEventData e)
        {
            var ray = _camera.ScreenPointToRay(e.position);
            var plane = new Plane(_camera.transform.forward, transform.position);
            plane.Raycast(ray, out var enter);
            return ray.GetPoint(enter);
        }

        private bool CalcAngle(PointerEventData e, out float rotation)
        {
            var worldPoint = CalcWorldPoint(e);
            var direction = _rotationCenter.position - worldPoint;
            if (!_weapon.IsPulledEnough())
            {
                _multiplier = null;
            }
            else if (_multiplier == null)
            {
                if (direction.x < 0)
                {
                    _multiplier = -1f;
                }
                else
                {
                    _multiplier = 1f;
                }
            }

            if (_multiplier == null)
            {
                if (direction.x < 0)
                {
                    direction *= -1f;
                }
            }
            else
            {
                direction *= _multiplier.Value;
                if (direction.x < 0)
                {
                    rotation = 0f;
                    return false;
                }
            }

            rotation = Mathf.Atan2(direction.y, direction.x)*Mathf.Rad2Deg;
            return true;
        }
        
        private float CalcPullLevel(PointerEventData e)
        {
            var worldPoint = CalcWorldPoint(e);
            var distance = Vector3.Distance(_rotationCenter.position, worldPoint);
            return _pullRange.UnLerp(distance);
        }


        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            if (!IsNextShotAvailbale()) { return; }

            _playerAnimations.Prepare();
            Drag(eventData);
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            if (!IsNextShotAvailbale()) { return; }

            Drag(eventData);
        }

        private void Drag(PointerEventData eventData)
        {
            var pullLevel = CalcPullLevel(eventData);
            _weapon.SetPull(pullLevel);
            _playerAnimations.SetPullLevel(pullLevel);

            if (CalcAngle(eventData, out var angle))
            {
                var rotation = _rotationRange.Clamp(angle);
                if (!_weapon.IsPulledEnough())
                {
                    rotation = 0f;
                }

                _playerAngle.rotation = rotation;
                _weapon.SetRotation(_playerAngle.rotation);
            }

            _weapon.SetDistance(Mathf.Clamp01(pullLevel) * _pullPower);
            //_weapon.Move(eventData);
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            if (!IsNextShotAvailbale())
            {
                _playerAnimations.Cancel();
                return;
            }

            _weapon.Fire(SetNextShotTime);
        }

        private void SetNextShotTime(bool isShotMade)
        {
            if (!isShotMade)
            {
                _playerAnimations.Cancel();
                return;
            }
            _playerAnimations.Fire();
            _nextShotTime = _gameSettings.timeBetweenShots + Time.time;
        }

        private bool IsNextShotAvailbale()
        {
            return Time.time >= _nextShotTime;
        }
    }
}