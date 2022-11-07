using System;
using DG.Tweening;
using MiniGames.Core.Signals;
using Coconut.Core;
using Coconut.Core.Asyncs;
using Coconut.Game;
using Coconut.Game.AnimatorHelpers;
using UnityEngine;
using Zenject;

namespace MiniGames.Games.BalloonsCatcher
{
    [RequireComponent(typeof(Animator))]
    public class BalloonExerciseFx : MonoBehaviour
    {
        [SerializeField] private string _cameraTag = "MainCamera";
        [SerializeField] private string _noneState = "None";
        [SerializeField] private string _fadedState = "Faded";
        [SerializeField] private string _introTrigger = "Intro";
        [SerializeField] private string _incorrectTrigger = "Incorrect";
        [SerializeField] private string _correctTrigger = "Correct";
        [SerializeField] private string _matchTrigger = "Match";
        [SerializeField] private string _dismatchTrigger = "Dismatch";
        [SerializeField] private Transform _plateUIRoot;
        [SerializeField] private Transform _plateUIMatchRoot;
        [SerializeField] private Transform _plateGameRoot;
        [SerializeField] private Transform _balloonRoot;
        [SerializeField] private Transform _balloonRoot1;
        [SerializeField] private Transform _balloonRoot2;
        [SerializeField] private Transform _balloonMatchRoot1;
        [SerializeField] private Transform _balloonMatchRoot2;
        [SerializeField] private float _transitionDuration = 0.2f;
        [SerializeField] private Transform _correctTarget;
        [SerializeField] private float _jumpDuration = 0.3f;
        [SerializeField] private float _plateJumpDuration = 0.4f;
        [SerializeField] private bool _applyNewText;


        [Inject] private SignalBus _signalBus;
        [Inject] private Character _dog;

        private Table _table;
        private Animator _animator;
        private Raycaster2DLockProxy _locks;
        private bool _isFirstIntro = true;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _signalBus.Subscribe<PauseGameSignal>(OnPauseGameSignal);
            _signalBus.Subscribe<ResumeGameSignal>(OnResumeGameSignal);

            var worldCamera = GameObjectLookup.ByTag<Camera>(_cameraTag);
            if (worldCamera == null) return;
            _locks = worldCamera.GetOrAddComponent<Raycaster2DLockProxy>();
        }

        private void OnDestroy()
        {
            _signalBus.Unsubscribe<PauseGameSignal>(OnPauseGameSignal);
            _signalBus.Unsubscribe<ResumeGameSignal>(OnResumeGameSignal);
        }

        private void OnPauseGameSignal()
        {
            _animator.speed = 0f;
        }

        private void OnResumeGameSignal()
        {
            _animator.speed = 1f;
        }

        public async Promise Intro(Table table)
        {
            _table = table;
            PlateTransition(table.transform, _plateUIRoot);
            _animator.SetTrigger(_introTrigger);
            await _animator.WaitStateAsync(_noneState);
            if (!_isFirstIntro)
            {
                _dog.True();
            }

            gameObject.Promise(PlateTransitionDelayed, table.transform, _plateGameRoot);
            //PlateTransition(table.transform, _plateGameRoot);
            _table = null;
            _isFirstIntro = false;
        }

        public async Promise Incorrect(Transform balloonTransform)
        {
            Transition(balloonTransform, _balloonRoot);
            _animator.SetTrigger(_incorrectTrigger);
            await _animator.WaitStateAsync(_noneState);
            _dog.False();
            Destroy(balloonTransform.gameObject);
        }

        public async Promise Correct(Transform balloonTransform)
        {
            Transition(balloonTransform, _balloonRoot);
            _animator.SetTrigger(_correctTrigger);
            using (_locks.Lock())
            {
                await _animator.WaitStateAsync(_noneState);
                await JumpToCorrectTarget(balloonTransform);
            }
            _dog.True();
        }

        public async Promise PlateTransitionDelayed(Transform plate, Transform parent)
        {
            await Timeline.Delay(0.2f); // fade transition duration
            PlateTransition(plate, parent);
        }

        public void PlateTransition(Transform plate, Transform parent)
        {
            plate.DOKill();
            plate.SetParent(parent);
            //plate.DOLocalJump(Vector3.zero, 1.0f, 1, _plateJumpDuration).SetUpdate(UpdateType.Normal, true);
            plate.DOLocalMove(Vector3.zero, _plateJumpDuration).SetUpdate(UpdateType.Normal, true);
            plate.DOLocalRotate(Vector3.zero, _plateJumpDuration).SetUpdate(UpdateType.Normal, true);
            plate.DOScale(Vector3.one, _plateJumpDuration).SetUpdate(UpdateType.Normal, true);
        }

        public void Transition(Transform balloonTransform, Transform parent)
        {
            balloonTransform.DOKill();
            balloonTransform.SetParent(parent);
            balloonTransform.DOLocalMove(Vector3.zero, _transitionDuration).SetUpdate(UpdateType.Normal, true);
            balloonTransform.DOLocalRotate(Vector3.zero, _transitionDuration).SetUpdate(UpdateType.Normal, true);
            balloonTransform.DOScale(Vector3.one, _transitionDuration).SetUpdate(UpdateType.Normal, true);
        }

        public Tween JumpToCorrectTarget(Transform balloonTransform)
        {
            balloonTransform.DOKill();
            balloonTransform.SetParent(_correctTarget);
            balloonTransform.DOScale(Vector3.one, _jumpDuration).SetUpdate(UpdateType.Normal, true);
            return balloonTransform.DOLocalJump(Vector3.zero, 0.1f, 1, _jumpDuration).SetUpdate(UpdateType.Normal, true);
        }

        public async Promise Dismatch(Transform balloonTransform1, Transform balloonTransform2)
        {
            Transition(balloonTransform1, _balloonRoot1);
            Transition(balloonTransform2, _balloonRoot2);
            _animator.SetTrigger(_dismatchTrigger);
            await _animator.WaitStateAsync(_noneState);
            _dog.False();
            Destroy(balloonTransform1.gameObject);
            Destroy(balloonTransform2.gameObject);
        }

        public async Promise Match(Transform balloonTransform1, Transform balloonTransform2, Table table)
        {
            _table = table;
            PlateTransition(table.transform, _plateUIMatchRoot);
            Transition(balloonTransform1, _balloonMatchRoot1);
            Transition(balloonTransform2, _balloonMatchRoot2);

            _animator.SetTrigger(_matchTrigger);
            await _animator.WaitStateAsync(_fadedState);

            Destroy(balloonTransform1.gameObject);
            Destroy(balloonTransform2.gameObject);
            //gameObject.Promise(PlateTransitionDelayed, table.transform, _plateGameRoot);
            //PlateTransition(table.transform, _plateGameRoot);
            _table = null;
        }

        private void LateUpdate()
        {
            if (_table == null) return;
            if(!_applyNewText) return;
            _table.ApplyNewText();
            _applyNewText = false;
        }
    }
}