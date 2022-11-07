using DG.Tweening;
using Coconut.Core;
using Coconut.Core.Asyncs;
using Coconut.Core.Collections;
using Coconut.Game;
using System;
using ExerciseGeneration;
using TMPro;
using UnityEngine;

namespace MiniGames.Games.FBLikesPower
{
    public class ExerciseCard : MonoBehaviour
    {
        [SerializeField] private RangeFloat _revealSpeedRange = new RangeFloat(1, 2);
        [SerializeField] private RangeFloat _pulseSpeedRange = new RangeFloat(1, 2);
        [SerializeField] private RangeFloat _pulseSizeRange = new RangeFloat(.7f, .9f);

        private ExerciseCardView _view;
        private PositionType _cardSide;
        private Vector3 _positionOrigin;
        private Vector3 _rotationOrigin;

        //---------------------------------------------------

        public void Init(Expression expression, PositionType cardSide, ProfileProxy profile)
        {
            _cardSide = cardSide;
            _view.SetExerciseText(expression.x, expression.y);
            _view.SetProfile(profile);
        }

        public Tween PushUp()
        {
            return transform.DOJump(transform.position + new Vector3(0, .5f, 0), 0.1f, 0, 1);
        }

        public Tween PushUp2()
        {
            return transform.DOMoveY(transform.position.y + 1f, 1.5f);
        }

        public void ResetView(Vector3 startPos)
        {
            _view.ResetView(startPos);

            transform.DOKill();
            StopFloating();

            transform.rotation = Quaternion.Euler(_rotationOrigin);
        }

        public void BacklightSetActive(bool value)
        {
            _view.BacklightSetActive(value);
        }

        public void SunRaysSetActive(bool value)
        {
            _view.SunRaysSetActive(value);
        }

        public void SetWin(bool value)
        {
            _view.BacklightSetActive(value);
            //_view.SetFrame(value);
        }

        public Tween ShakeWrongAnswer()
        {
            return DOTween.Sequence()
                .Append(transform.DOShakePosition(2, .5f, 5, 45))
                .Join(transform.DOShakeRotation(2, new Vector3(0, 0, 30), 10, 45))
                ;
        }

        //TODO: DRY
        public Tween ShowTo(Vector3 position)
        {
            var speed = _revealSpeedRange.Random();
            var rotateDir = new int[] { 360, -360 }.GetRandom();

            return DOTween.Sequence()
                .Append(transform.DOMove(position, speed))
                .Join(transform.DORotate(new Vector3(0, 0, rotateDir), speed, RotateMode.LocalAxisAdd))
                .Join(transform.DOScale(1, speed))
                .SetUpdate(UpdateType.Normal, true)
                ;
        }

        public Tween HideTo(Vector3 position)
        {
            var speed = _revealSpeedRange.Random();
            var rotateDir = new int[] { 360, -360 }.GetRandom();

            return DOTween.Sequence()
                .Append(transform.DOMove(position, speed))
                .Join(transform.DORotate(new Vector3(0, 0, rotateDir), speed, RotateMode.LocalAxisAdd))
                .Join(transform.DOScale(0, speed))
                .SetUpdate(UpdateType.Normal, true)
                ;
        }

        public void Blink()
        {
            gameObject.Promise(BlinkAsync);
        }

        public void StartFloating()
        {
            gameObject.Promise(ShakeAsyncLoop);
            gameObject.Promise(MoveYAsyncLoop);
            gameObject.Promise(MoveXAsyncLoop);
        }

        public void StopFloating()
        {
            gameObject.CancelPromise(ShakeAsyncLoop);
            gameObject.CancelPromise(MoveYAsyncLoop);
            gameObject.CancelPromise(MoveXAsyncLoop);
        }       

        //---------------------------------------------------

        private void Awake()
        {
            _view = GetComponent<ExerciseCardView>();
            //PulseStart();

            _positionOrigin = transform.localPosition;
            _rotationOrigin = transform.rotation.eulerAngles;

        }

        private void OnDestroy()
        {
            transform.DOKill();
        }

        private async Promise ShakeAsyncLoop()
        {
            await Timeline.Delay(1);

            while (true)
            {
                await ShakePingPong();
            }

        }

        private async Promise MoveYAsyncLoop()
        {
            while (true)
            {
                await MoveYPingPong();
            }

        }

        private async Promise MoveXAsyncLoop()
        {
            while (true)
            {
                await MoveXPingPong();
            }

        }

        //TODO: DRY
        private Tween MoveYPingPong()
        {
            var speed = new RangeFloat(15, 19).Random();
            var array = new float[] { .3f, -0.3f };
            var other = array.GetRandom();
            var another = Array.Find(array, item => item != other);

            return DOTween.Sequence()
                .Append(transform.DOLocalMoveY(_positionOrigin.y + other, speed))
                .Append(transform.DOLocalMoveY(_positionOrigin.y + another, speed))
                .SetUpdate(UpdateType.Normal, true)
                ;
        }

        private Tween MoveXPingPong()
        {
            var speed = new RangeFloat(14, 20).Random();
            var array = new float[] { .1f, -.2f };
            var other = array.GetRandom();
            var another = Array.Find(array, item => item != other);

            return DOTween.Sequence()
                .Append(transform.DOLocalMoveX(_positionOrigin.x + other, speed))
                .Append(transform.DOLocalMoveX(_positionOrigin.x + another, speed))
                ;
        }

        private Tween ShakePingPong()
        {
            var speed = new RangeFloat(10, 14).Random();
            var array = new float[] { 2.5f, -2.5f };
            var other = array.GetRandom();
            var another = Array.Find(array, item => item != other);

            return DOTween.Sequence()
               .Append(transform.DORotate(_rotationOrigin + new Vector3(0, 0, other), speed))
                   .Append(transform.DORotate(_rotationOrigin + new Vector3(0, 0, another), speed))
                   .SetUpdate(UpdateType.Normal, true)
               ;
        }

        private async Promise BlinkAsync()
        {
            for (int i = 0; i < 2; i++)
            {
                foreach (var item in GetComponentsInChildren<SpriteRenderer>())
                {
                    _ = Blinkk(item);
                }

                foreach (var item in GetComponentsInChildren<TextMeshPro>())
                {
                    _ = BlinkkTMP(item);
                }

                await Timeline.Delay(.6f);
            }
        }

        private Tween Blinkk(SpriteRenderer renderer)
        {
            return DOTween.Sequence()
                  .Append(renderer.DOFade(.3f, .4f))
                  .Append(renderer.DOFade(1f, .4f))
                  ;
        }

        private Tween BlinkkTMP(TextMeshPro text)
        {
            return DOTween.Sequence()
                   .Append(text.DOFade(.3f, .4f))
                   .Append(text.DOFade(1f, .4f))
                   ;
        }

        private Tween Pulse()
        {
            return DOTween.Sequence()
                .Append(transform.DOScale(_pulseSizeRange.Random(), _pulseSpeedRange.Random()))
                .Append(transform.DOScale(1, _pulseSpeedRange.Random()))
                ;
        }
    }
}