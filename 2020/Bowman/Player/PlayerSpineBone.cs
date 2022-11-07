using System;
using UnityEngine;

namespace MiniGames.Games.Bowman
{
    public class PlayerSpineBone: MonoBehaviour
    {
        [SerializeField] private PlayerAngle _angle;

        private Transform _transform;

        private void Awake()
        {
            _transform = transform;
        }


        private void LateUpdate()
        {
            _transform.RotateAround(transform.position, Vector3.forward, _angle.rotation);
        }
    }
}