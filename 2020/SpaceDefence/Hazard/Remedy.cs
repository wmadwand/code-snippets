using DG.Tweening;
using Coconut.Asyncs;
using MiniGames.Games.SpaceDefence.Hazard.Projectile;
using SpaceDefence.Core;
using UnityEngine;

namespace MiniGames.Games.SpaceDefence.Hazard
{
    public sealed class Remedy : ProjectileBase
    {
        public override bool IsCollectible => true;

        [SerializeField] private Transform _heartTransform;
        private const string AnimatorBoolName = "Blink";
        private Animator _animator;

        //-------------------------------------------------

        public override void AwakeOperations()
        {
            base.AwakeOperations();

            _animator = GetComponent<Animator>();
        }

        public override void StartOperations()
        {
            base.StartOperations();

            _heartTransform.DOLocalRotate(new Vector3(0f, 360f, 0f), 5f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear).SetLoops(-1);
        }

        public void DestroyAfterBlink()
        {
            var range = new RangeFloat(3, 5);

            Planner.Chain()
                   .AddTimeout(1)
                   .AddAction(_animator.SetBool, AnimatorBoolName, true)
                   .AddTimeout(range.Random())
                   .AddAction(_animator.SetBool, AnimatorBoolName, false)
                   .AddAction(SendOnDestroy, false);
        }
    }
}