using UnityEngine;

namespace Project.IO.Camera
{
    public class PlayerCameraInputLegacy : PlayerCameraInputBase
    {
        public override float ZoomValue => _zoomValue;
        public override float RotationValue => _rotationValue;

        [SerializeField] private float _zoomDeltaRate = 0.05f;

        private float _zoomValue;
        private float _rotationValue;
        private Vector2 _startPos = Vector2.zero;
        private Vector2 _directionTouch;

        //--------------------------------------------------------------

        protected override void UpdateHandler()
        {
            if (Input.touchSupported)
            {
                if (Input.touchCount > 0)
                {
                    MobileInput();
                }
                else
                {
                    _zoomValue = 0;
                    _rotationValue = 0;
                }
            }
            else
            {
                StandaloneInput();
            }
        }

        private void MobileInput()
        {
            switch (Input.touchCount)
            {
                case 1:
                    {
                        // Track a single touch as a direction control.
                        if (Input.touchCount > 0)
                        {
                            Touch touch = Input.GetTouch(0);

                            // Handle finger movements based on TouchPhase
                            switch (touch.phase)
                            {
                                //When a touch has first been detected, change the message and record the starting position
                                case TouchPhase.Began:
                                    // Record initial touch position.
                                    _startPos = touch.position;
                                    //message = "Begun ";
                                    break;

                                //Determine if the touch is a moving touch
                                case TouchPhase.Moved:
                                    // Determine direction by comparing the current touch position with the initial one
                                    _directionTouch = touch.position - _startPos;
                                    //RotateAround(-directionTouch.normalized.x);
                                    _rotationValue = -_directionTouch.normalized.x;

                                    //message = "Moving ";
                                    break;

                                case TouchPhase.Stationary:
                                    _startPos = touch.position;
                                    break;

                                case TouchPhase.Ended:
                                    // Report that the touch has ended when it ends
                                    //message = "Ending ";
                                    break;
                            }
                        }

                    }
                    break;
                case 2:
                    {
                        // Store both touches.
                        Touch touchZero = Input.GetTouch(0);
                        Touch touchOne = Input.GetTouch(1);

                        // Find the position in the previous frame of each touch.
                        Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                        Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                        // Find the magnitude of the vector (the distance) between the touches in each frame.
                        float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                        float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

                        // Find the difference in the distances between each frame.
                        var zoomDelta = _zoomDeltaRate * (touchDeltaMag - prevTouchDeltaMag);
                        //Zoom(zoomDelta);

                        _zoomValue = zoomDelta;
                    }

                    break;
                default:
                    break;
            }
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