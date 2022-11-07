using DG.Tweening;
using Coconut.Core.Asyncs;
using Coconut.Game;
using System;
using UnityEngine;
using Utils;

namespace MiniGames.Games.FBLikesPower
{
    public class HeartResultPanel : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _heartDefaulttRenderer;
        [SerializeField] private SpriteRenderer _heartCorrectRenderer;
        [SerializeField] private SpriteRenderer _symbolRenderer;
        [SerializeField] private ParticleSystem _heartEmitter;
        [SerializeField] private Sprite _questionMark;

        [Header("Sound")]
        [SerializeField] private ShootSoundOnEvent _flyHeartsSound;
        [SerializeField] private float _flyHeartsSoundTimeout;

        [Space]
        [SerializeField] private ShootSoundOnEvent _startupChangeSymbolsSound;
        [SerializeField] private float _startupChangeSymbolsSoundTimeout;

        [Space]
        [SerializeField] private ShootSoundOnEvent _startupChangeSymbolsQuestionSound;
        [SerializeField] private float _startupChangeSymbolsQuestionSoundTimeout;

        private Animator _animator;
        private Vector3 _posOrigin;

        Vector3 mathSymbolPosOrigin;

        //---------------------------------------------------

        public Tween ShowSymbolTween(Sprite sprite)
        {
            _symbolRenderer.sprite = sprite;
            return _symbolRenderer.DOFade(1, .5f);
        }

        public Tween ShowTutorial(MathButton[] buttons, Transform centerPoint)
        {
            var seq = DOTween.Sequence();

            seq.Append(_symbolRenderer.DOFade(1, 0));

            foreach (var item in buttons)
            {
                seq.AppendCallback(() => PlaySound(_startupChangeSymbolsSound, _startupChangeSymbolsSoundTimeout));
                seq.AppendCallback(() => ShowSymbol(item.SymbolSprite));
                seq.AppendInterval(.5f);
            }

            seq.AppendCallback(() => PlaySound(_startupChangeSymbolsSound, _startupChangeSymbolsSoundTimeout));
            seq.AppendCallback(() => PlaySound(_startupChangeSymbolsQuestionSound, _startupChangeSymbolsQuestionSoundTimeout));
            seq.AppendCallback(() => ShowSymbol(_questionMark));
            seq.AppendInterval(.5f);

            var mathSymbol = transform.Find("View/MathSymbol").gameObject;

            seq.Append(mathSymbol.transform.DOScale(10, 2))
               .Join(mathSymbol.transform.DOMove(centerPoint.position, 2))
               .Join(_symbolRenderer.DOFade(0, 2))
               .Append(mathSymbol.transform.DOScale(1, 0))
               .Append(mathSymbol.transform.DOMove(mathSymbolPosOrigin, 0))
               ;

            return seq;
        }

        public async Promise SetCorrectAnswer(PositionType winSide)
        {
            await _heartCorrectRenderer.DOFade(1, .5f);
            _ = PlayHeartEmitter(winSide, async () =>
            {
                await Timeline.Delay(.5f);
                //SetDefaultSprite();
            });
        }

        public async Promise SetWrongAnswer()
        {
            _animator.SetTrigger("Break");
            await _animator.WaitStateAsync("BreakHeart");

            await Timeline.Delay(.5f);

            //animator.SetTrigger("Tilt");
            //await animator.WaitStateAsync("TiltHeart");

            await Tilt();

            _animator.SetTrigger("Restore");
            await _animator.WaitStateAsync("RestoreHeart");

            await HideSymbol();
        }

        public Tween Show()
        {
            transform.position += new Vector3(0, 1, 0);

            return DOTween.Sequence()
                .Append(_heartDefaulttRenderer.DOFade(1, 1))
                .Join(transform.DOMoveY(_posOrigin.y, 1))
                .SetUpdate(UpdateType.Normal, true)
                ;
        }

        public Tween Hide()
        {
            //var compnents = GetComponentsInChildren<SpriteRenderer>();
            var seq = DOTween.Sequence();

            seq
                .Append(_symbolRenderer.DOFade(0, .5f))
                .Join(_heartCorrectRenderer.DOFade(0, .5f))
                .Join(_heartDefaulttRenderer.DOFade(0, 1))
                .Join(transform.DOMoveY(_posOrigin.y + 1, 1))
                .SetUpdate(UpdateType.Normal, true)
                ;

            return seq;
        }

        public void ResetView()
        {
            var compnents = GetComponentsInChildren<SpriteRenderer>();

            foreach (var item in compnents)
            {
                item.DOFade(0, 0);
            }
        }

        //---------------------------------------------------

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _posOrigin = transform.position;
            mathSymbolPosOrigin = transform.Find("View/MathSymbol").gameObject.transform.position;

            SetDefaultSprite();
            ResetView();
        }

        private Tween Tilt()
        {
            return transform.DOPunchRotation(Vector3.forward * 30, 3, 2);
        }

        private void ShowSymbol(Sprite sprite)
        {
            _symbolRenderer.sprite = sprite;
        }

        private void SetDefaultSprite()
        {
            _symbolRenderer.DOFade(0, 0);
            _heartCorrectRenderer.DOFade(0, 0);
        }

        private Tween HideSymbol()
        {
            return _symbolRenderer.DOFade(0, .5f);
        }

        //TODO: extract HeartResultFx class
        private async Promise PlayHeartEmitter(PositionType direction, Action callback)
        {
            switch (direction)
            {
                case PositionType.Left:
                    _heartEmitter.transform.rotation = Quaternion.Euler(0, -90, 0);
                    break;
                case PositionType.Right:
                    _heartEmitter.transform.rotation = Quaternion.Euler(0, 90, 0);
                    break;
                case PositionType.Both:
                    _heartEmitter.transform.rotation = Quaternion.Euler(0, 0, 0);
                    break;
            }

            PlaySound(_flyHeartsSound, _flyHeartsSoundTimeout);

            _heartEmitter.Play();
            await Timeline.Delay(1.5f);
            _heartEmitter.Stop();
            callback?.Invoke();
        }

        private void PlaySound(ShootSoundOnEvent sound, float timeout = 0f)
        {
            if (sound == null) return;
            sound.Play(timeout);
        }
    }
}