using System.Threading.Tasks;
using DG.Tweening;
using Coconut.Core.Asyncs;
using UnityEngine;

namespace MiniGames.Games.BalloonsCatcher
{
    public class BalloonMovement : MonoBehaviour
    {
        [Range(1, 100)] [SerializeField] private int _speed;
        [SerializeField] private float _jumpDuration = 0.3f;
        [SerializeField] private float _moveTarget = 20f;
        [SerializeField] private float _moveDuration = 25f;

        private Tween _moveYTween;
        private Sequence _initSequence;

        //-------------------------------------------------

        public async Promise Appear()
        {
            await transform.DOMoveY(transform.position.y + 3f, .3f);
            await PunchPosition();
        }

        public async Promise MoveUp()
        {
            var speed = 101 - _speed;
            await transform.DOMoveY(_moveTarget, _moveDuration);
        }

        public void Init()
        {

            var speed = 101 - _speed;
            _moveYTween = transform.DOMoveY(_moveTarget, speed);
            StopMove();

            _initSequence = DOTween.Sequence()
                .Append(transform.DOMoveY(transform.position.y + 3f, .3f))
                .Append(PunchPosition())
                .AppendInterval(.5f)
                .AppendCallback(() => _moveYTween.Play())
                ;
        }

        public Tween StopInit()
        {
            return _initSequence.Pause();
        }

        public Tween ProceedInit()
        {
            return _initSequence.Play();
        }

        public Tween StopMove()
        {
            _initSequence.Kill();
            return _moveYTween.Pause();
        }

        public Tween ProceedMove()
        {
            return _moveYTween.Play();
        }

        public Tween PunchPosition()
        {
            return transform.DOPunchPosition(-Vector3.up / 3, .5f, 5);
        }

        public Tween JumpTo(Vector3 position)
        {
            return transform.DOJump(position, 0.1f, 1, _jumpDuration);
        }

        public void KillMoveUpTween()
        {
            transform.DOKill();
        }
    }
}