using System;
using DG.Tweening;
using MiniGames.Games.Bowman.Shooting;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace MiniGames.Games.Bowman
{
    public class WeaponMovement : MonoBehaviour
    {
        [SerializeField] private float _yPosMax = 40f;
        [SerializeField] private float _yPosMin = -40f;
        [SerializeField] private float _pullRate = 0.05f;
        [SerializeField] private float _xPulledEnoughPos = .5f;
        [SerializeField] private float _xPosMin = -.4f;
        [SerializeField] private float _minPull = 0.25f;

        public event Action<float> changeRotation;

        //public float yRate;

        [Inject] BowmanSettings _gameSettings;

        private LineRenderer _lineRenderer;
        //private Sequence _resetPositionSequence;
        private Quaternion _originRotation;
        private WeaponHelper _helper;
        private WeaponTrajectory _trajectory;

        private float _pull;
        private float _distance;
        //---------------------------------------------------

        public void SetRotation(float angle)
        {
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        public void SetPull(float pull)
        {
            _pull = pull;
        }

        public void SetDistance(float distance)
        {
            _distance = distance;
        }

        public void Move(PointerEventData eventData)
        {
            //_resetPositionSequence?.Kill();

            Rotate(eventData);
            Pull(eventData);
        }

        public void ResetPosition(float time = 0)
        {
            _distance = 0f;
            _pull = 0f;

            changeRotation?.Invoke(0f);
            /*
            _resetPositionSequence = DOTween.Sequence()
                  .Append(_helper.ProjectileHolder.DOLocalMove(_helper.ProjectileSpawnPoint.localPosition, time))
                  .Join(transform.DORotateQuaternion(_originRotation, time))
                  .SetUpdate(UpdateType.Normal, true)
                  ;
            */
            //return _resetPositionSequence;
        }

        public bool IsPulledEnough()
        {
            return _pull > _minPull;
            //return _helper.ProjectileSpawnPoint.localPosition.x - _helper.ProjectileHolder.localPosition.x > _xPulledEnoughPos;
        }

        public Vector3 GetPulledVelocity()
        {
            return transform.right * _distance;

            /*
            Vector3 velocity = _helper.ProjectileShotPoint.position - _helper.ProjectileHolder.position;
            var distance = Vector3.Distance(_helper.ProjectileShotPoint.position, _helper.ProjectileHolder.position);

            return velocity * distance;
            */
        }

        //---------------------------------------------------

        private void Awake()
        {
            _helper = GetComponentInChildren<WeaponHelper>();
            _lineRenderer = GetComponent<LineRenderer>();
            _originRotation = transform.rotation;
            _trajectory = new WeaponTrajectory(_lineRenderer, _helper, _gameSettings);
        }

        private void Update()
        {
            if (!IsPulledEnough())
            {
                _lineRenderer.enabled = false;
                return;
            }
            //var distance = Vector3.Distance(_helper.ProjectileShotPoint.position, _helper.ProjectileHolder.position);
            _trajectory.Display(transform.right, _distance);
        }

        private void Rotate(PointerEventData eventData)
        {
            var pressPosition = eventData.pressPosition;
            var currentPosition = eventData.position;
            var delta = currentPosition - pressPosition;
            var yPosClamp = Mathf.Clamp(delta.y, _yPosMin, _yPosMax);
            var rotation = Quaternion.Euler(0, 0, -yPosClamp);
            //Quaternion.Lerp(transform.rotation, rot, 3 * Time.time);

            changeRotation?.Invoke(-yPosClamp);

            transform.rotation = rotation;
        }

        private void Pull(PointerEventData eventData)
        {
            var direction = eventData.delta.normalized;
            var position = _helper.ProjectileHolder.transform.localPosition + new Vector3(direction.x * _pullRate, 0);
            var xPosClamp = Mathf.Clamp(position.x, _xPosMin, _helper.ProjectileSpawnPoint.localPosition.x);
            position.x = xPosClamp;
            _helper.ProjectileHolder.transform.localPosition = position;
        }
    }
}