using Project.Data;
using Project.IO.Camera;
using UnityEngine;

namespace Project.Gameplay
{
    public interface IPlayerCamera
    {
        Camera Camera { get; }
        float HeightPercent { get; }
        float CurrentAngle { get; }
    }

    [RequireComponent(typeof(Camera))]
    public class PlayerCamera : MonoBehaviour, IPlayerCamera
    {
        public Camera Camera { get; private set; }

        [SerializeField] private Transform _target = null;
        [SerializeField] private PlayerCameraInputBase _input = null;
        [SerializeField] private PlayerCameraSettings _settings;

        private float _rotationAngle = 0f;
        private Vector3 _targetDirection = Vector3.zero;
        private float _orbitRadius = 0f;

        private PlayerCameraGO _playerCameraGO;

        //--------------------------------------------------------------    

        public float HeightPercent => _settings.zoomCurve.Evaluate(1 - _orbitRadius / _settings.zoomClamp.max);
        //public float HeightPercent => _playerCameraGO.HeightPercent;
        public float CurrentAngle => _playerCameraGO.currentAngle;

        //--------------------------------------------------------------

        private void Awake()
        {
            Camera = GetComponent<Camera>();

            _playerCameraGO = GetComponent<PlayerCameraGO>();
        }

        private void Start()
        {
            _orbitRadius = _settings.zoomClamp.min;
        }

        private void Update()
        {
            transform.position = _target.position + GetRotationOrbit();
            transform.rotation = GetLoookRotationAt(_target);

            //RotateAround(_input.RotationValue);
            Zoom(_input.ZoomValue);
        }

        private Vector3 GetRotationOrbit()
        {
            var heightDirection = Vector3.up * HeightPercent * _settings.zoomHeightRate;
            //var raidusDirection = Vector3.forward * _orbitRadius;
            var orbit = /*raidusDirection +*/ heightDirection;

            return Quaternion.AngleAxis(_rotationAngle, Vector3.up) * orbit;
        }

        private Quaternion GetLoookRotationAt(Transform target)
        {
            _targetDirection = (target.position - transform.position).normalized;

            return Quaternion.LookRotation(_targetDirection);
        }

        private void RotateAround(float value)
        {
            if (value == 0) { return; }

            var scrollDelta = value * _settings.rotationSpeed;

            if (scrollDelta < -360F)
                scrollDelta += 360F;
            if (scrollDelta > 360F)
                scrollDelta -= 360F;

            _rotationAngle -= scrollDelta;
        }

        private void Zoom(float value)
        {
            if (value == 0) { return; }

            var radiusRes = _orbitRadius - value * _settings.zoomSpeed;
            radiusRes = Mathf.Clamp(radiusRes, _settings.zoomClamp.min, _settings.zoomClamp.max);
            _orbitRadius = radiusRes;
        }
    }
}