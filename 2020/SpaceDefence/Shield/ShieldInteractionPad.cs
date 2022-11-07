using SpaceDefence.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace MiniGames.Games.SpaceDefence.Shield
{
    public class ShieldInteractionPad : MonoBehaviour, IPointerDownHandler, IDragHandler, IEndDragHandler, IBeginDragHandler
    {
        public RangeFloat clampCameraRotation = new RangeFloat(-5f, 5f);
        public float rotationSensitivity = 1f;

        [Inject] private Shield _shield;
        [Inject] private CameraShieldRotate _cameraShieldRotate;

        private Plane _plane;
        private Ray _ray;
        private Vector3 _pos;

        //-------------------------------------------------

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            MoveShield(eventData);
            MoveCamera(eventData);
        }

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            _shield.Movement.OnBeginDrag();
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            MoveShield(eventData);
            MoveCamera(eventData);

            Debug.Log($"SPEED {eventData.delta}");
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            _shield.Movement.OnEndDrag();
        }

        private void MoveShield(PointerEventData eventData)
        {
            _plane = new Plane(Vector3.forward, _shield.transform.position);
            _ray = eventData.pressEventCamera.ScreenPointToRay(eventData.position);

            if (_plane.Raycast(_ray, out float distance))
            {
                _pos = _ray.origin + _ray.direction * distance;
                _shield.Movement.MoveTo(_pos, eventData);
            }
        }

        private void MoveCamera(PointerEventData eventData)
        {
            var angle = (eventData.position.x - Screen.width * 0.5f) * rotationSensitivity / Screen.height;
            _cameraShieldRotate.SetRotation(new Vector3(0, clampCameraRotation.Clamp(angle), 0));
        }
    }
}