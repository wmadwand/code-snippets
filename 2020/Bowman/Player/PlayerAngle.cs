using System;
using Coconut.Game.Movers;
using Coconut.Game.Movers.Core;
using Coconut.Game.Movers.Core.LerpMove;
using UnityEngine;
using Zenject;

namespace MiniGames.Games.Bowman
{
    public class PlayerAngle: MonoBehaviour, ILerpClient<float>
    {
        [SerializeField] private float _rotation = 0f;
        [Inject] private LerpMoverService _lerpService;

        private LerpJoint<float> _rotationJoint;

        private void Awake()
        {
            //_rotationJoint = _lerpService.AddJoint(this);
        }

        public float rotation
        {
            get => _rotation;
            set => _rotationJoint.target.lerpPosition = value;
        }

        float ILerpClient<float>.lerpPosition { get => _rotation; set => _rotation = value; }
    }
}