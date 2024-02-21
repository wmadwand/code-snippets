using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace Project.IO.Camera
{
    public class PlayerCameraInputEnhancedTouchSupport : PlayerCameraInputBase
    {
        public override float ZoomValue => _zoomValue;
        public override float RotationValue => _rotationValue;

        [SerializeField] private float _zoomDeltaRate = 0.05f;

        private float _zoomValue;
        private float _rotationValue;
        private Vector2 _startPos = Vector2.zero;
        private Vector2 _directionTouch;

        public float TouchSpeed = 10f;

        private float lastMultiTouchDistance;
        private bool isBuilding = false;

        //--------------------------------------------------------------

        //private void Awake()
        //{
        //    //Enable support for the new Enhanced Touch API and testing with the mouse
        //    EnhancedTouchSupport.Enable();

        //    //Uncomment the next line if you are using mouse to simulate touch
        //    //TouchSimulation.Enable();
        //}

        private void OnEnable()
        {
            EnhancedTouchSupport.Enable();
        }

        private void OnDisable()
        {
            EnhancedTouchSupport.Disable();
        }

        protected override void UpdateHandler()
        {
            var touchscreen = GetRemoteDevice<Touchscreen>();
            if (touchscreen == null)
            {
                StandaloneInput();
                return;
            }

            if (Touch.activeFingers.Count == 1)
            {
                MoveCamera(Touch.activeTouches[0]);
            }
            else if (Touch.activeFingers.Count == 2)
            {
                ZoomCamera(Touch.activeTouches[0], Touch.activeTouches[1]);
            }
            else if (Touch.activeFingers.Count == 0)
            {
                _zoomValue = 0;
                _rotationValue = 0;
            }
        }

        private void ZoomCamera(Touch firstTouch, Touch secondTouch)
        {
            if (firstTouch.phase == TouchPhase.Began || secondTouch.phase == TouchPhase.Began)
            {
                lastMultiTouchDistance = Vector2.Distance(firstTouch.screenPosition, secondTouch.screenPosition);
            }

            if (firstTouch.phase != TouchPhase.Moved || secondTouch.phase != TouchPhase.Moved)
            {
                return;
            }

            float newMultiTouchDistance = Vector2.Distance(firstTouch.screenPosition, secondTouch.screenPosition);
            _zoomValue = _zoomDeltaRate * (newMultiTouchDistance - lastMultiTouchDistance);
            lastMultiTouchDistance = newMultiTouchDistance;
        }

        private void MoveCamera(Touch touch)
        {
            if (touch.phase != TouchPhase.Moved)
            {
                return;
            }

            _rotationValue = -touch.delta.normalized.x;
        }

        // Make sure we're not thrown off track by locally having sensors on the device. Instead
        // explicitly grab the remote ones.
        private static TDevice GetRemoteDevice<TDevice>()
            where TDevice : InputDevice
        {
            foreach (var device in InputSystem.devices)
                if (device.remote && device is TDevice deviceOfType)
                    return deviceOfType;
            return default;
        }

        private void StandaloneInput()
        {
            if (Input.GetMouseButton(0))
            {
                var xInput = Input.GetAxis("Mouse X");
                _rotationValue = xInput;
            }
            else
            {
                _rotationValue = 0;
            }

            if (Input.GetMouseButton(1))
            {
                var scrollDelta = Input.GetAxis("Mouse Y");
                _zoomValue = -scrollDelta;
            }
            else
            {
                _zoomValue = 0;
            }
        }
    } 
}