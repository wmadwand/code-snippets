using System;
using UnityEngine;

namespace MiniGames.Games.Bowman
{
    public class PlayerBowBone : MonoBehaviour
    {
        [SerializeField] private PlayerAngle _angle;
        [SerializeField] private Transform _rotationCenter;
        [SerializeField] private Transform _updateTransform;

        private Transform _transform;

        private void Awake()
        {
            _transform = transform;
        }

        private void LateUpdate()
        {
            _transform.RotateAround(_rotationCenter.position, Vector3.forward, _angle.rotation);
            if (_updateTransform == null) return;
            _updateTransform.position = _transform.position;
        }
    }
}