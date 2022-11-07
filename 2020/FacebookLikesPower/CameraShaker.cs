using DG.Tweening;
using UnityEngine;

namespace Dollhouse.Tutorials.Scenarios
{
    public class CameraShaker : MonoBehaviour
    {
        public float duration = 1f;
        public float strength = 0.1f;
        public int vibrato = 20;
        public bool active;

        private Transform _camera;

        private void Awake()
        {
            if (Camera.main == null) return;
            _camera = Camera.main.GetComponent<Transform>();
        }

        /// <summary>
        /// animation event
        /// </summary>
        public void Shake()
        {
            if (_camera == null) return;
            if (!active) return;
            _camera.DOShakePosition(duration, strength, vibrato).SetUpdate(true);
        }
    }
}