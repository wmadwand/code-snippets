using UnityEngine;

namespace MiniGames.Games.Bowman
{
    public class TargetMovement : MonoBehaviour
    {
        private Vector3 _originPos;
        private Vector3 _offsetPos;
        private Harmonic _settings;
        private bool _isMoving;

        //---------------------------------------------------

        public void Init(Harmonic settings)
        {
            _settings = settings;
            SetPosition();
        }

        public void StartMove()
        {
            _isMoving = true;
        }

        public void StopMove()
        {
            _isMoving = false;
        }

        //---------------------------------------------------

        private void SetPosition()
        {
            _offsetPos = _originPos;
            Floating();
        }

        private void Update()
        {
            if (!_isMoving)
            {
                return;
            }

            Floating();
        }

        private void Floating()
        {
            _offsetPos.x = Vector3.zero.x + /*_gameController.centerPoint.transform.position.x*/ +_settings.amplitude * Mathf.Sin(2 * Mathf.PI * (_settings.phase + Time.timeSinceLevelLoad / _settings.period));

            transform.localPosition = _offsetPos;
        }
    }
}