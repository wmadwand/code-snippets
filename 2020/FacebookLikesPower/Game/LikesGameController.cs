using System.Collections.Generic;
using DataModels.UserData;
using DG.Tweening;
using Dollhouse.Tutorials.Scenarios;
using MiniGames.Core.TaskPanel;
using Coconut.Core;
using Coconut.Core.Asyncs;
using Coconut.Game;
using UnityEngine;
using Utils;
using Zenject;

namespace MiniGames.Games.FBLikesPower
{
    public class FBLikesPowerController : MonoBehaviour
    {
        [SerializeField] private Dashboard _dashboard;
        [SerializeField] private DevicesGrid _devicesGrid;
        [SerializeField] private GameObject _progress;
        [SerializeField] private TaskPanel _taskPanel;
        [SerializeField] private bool _shakeCameraOnWrongAnswer;
        [SerializeField] private RangeFloat _timeoutBeforeFirstRound = new RangeFloat(2, 3);
        [SerializeField] private RangeFloat _timeoutBetweenRoundsRange = new RangeFloat(3, 5);

        [Inject] private UserProgressConfig _userProgress;
        [Inject] private LikesMessageBus _messageBus;
        [Inject] private ProfileCollection _profileCollection;
        [Inject] private CameraShaker _cameraShaker;

        private CompareExercise _exercise;
        private ProfileProxy _profile01;
        private ProfileProxy _profile02;
        private DeviceNode _chosenDevice;
        private PromiseCompletionSource _roundComplete;

        public Transform centerPoint;

        [Header("Sound")]
        [SerializeField] private ShootSoundOnEvent _moveToNodeSound;
        [SerializeField] private float _moveToNodeSoundTimeout = 0.5f;

        [Space]
        [SerializeField] private ShootSoundOnEvent _moveFromNodeSound;
        [SerializeField] private float _moveFromNodeSoundTimeout = 0.5f;

        [Space]
        [SerializeField] private ShootSoundOnEvent _likeSound;
        [SerializeField] private float _likeSoundTimeout;

        public bool IsFirstRound { get; private set; } = true;

        //---------------------------------------------------
        public void StartRound()
        {
            gameObject.CancelPromise(StartRoundAsync);
            gameObject.Promise(StartRoundAsync);
        }

        //---------------------------------------------------

        private void Awake()
        {
            List<int> completedNumbers = _userProgress.data.GetCompletedNumbers();
            _exercise = new CompareExercise(completedNumbers);

            _messageBus.mathButtonClick.receive += OnMathButtonClick;

            _profile01 = new ProfileProxy(_profileCollection.GetProfile(0));
            _profile02 = new ProfileProxy(_profileCollection.GetProfile(1));
        }

        private void Start()
        {
            _progress.GetComponentInChildren<CanvasGroup>().alpha = 0;
        }

        private void OnDestroy()
        {
            _messageBus.mathButtonClick.receive -= OnMathButtonClick;
        }

        private void OnMathButtonClick(MathButton button)
        {
            gameObject.Promise(CheckExerciseAnswer, button);
        }

        private async Promise CheckExerciseAnswer(MathButton button)
        {
            await _dashboard.ShowChosenSymbol(button);
            await Timeline.Delay(.5f);

            if (!_exercise.IsAnswerCorrect(button.Symbol))
            {
                if (_shakeCameraOnWrongAnswer)
                {
                    _cameraShaker.Shake();
                }

                await _dashboard.SetWrongAnswer();

                return;
            }

            await _dashboard.SetCorrectAnswer(_exercise.GetWinPosition());
            await Timeline.Delay(1);

            if (IsFirstRound) IsFirstRound = false;

            _messageBus.correctAnswer.Send();

            _ = _devicesGrid.FadeIn();
            await _dashboard.HideAsync();

            PlaySound(_moveFromNodeSound, _moveFromNodeSoundTimeout);

            await GoBackToGrid();

            _roundComplete.SetResult();
        }        

        private float GetTimeout()
        {
            return IsFirstRound ? _timeoutBeforeFirstRound.Random() : _timeoutBetweenRoundsRange.Random();
        }

        private async Promise StartRoundAsync()
        {
            if (!IsFirstRound)
            {
                await (_roundComplete = new PromiseCompletionSource()).promise;
            }            

            _dashboard.ResetView();

            var timeout = GetTimeout();
            await Timeline.Delay(timeout);

            var cardsPair = _exercise.NextComparePair();
            _dashboard.Init(cardsPair, _profile01, _profile02);

            await ChooseDeviceOnGridAsync();
            var chosenDevice = _chosenDevice;

            _devicesGrid.GetComponent<BGScroller>().Stop(true);
            _devicesGrid.transform.SetParent(chosenDevice.transform);

            _ = _devicesGrid.FadeOut(.7f, 3, chosenDevice);

            PlaySound(_likeSound, _likeSoundTimeout);

            await chosenDevice.ShowChosen(centerPoint.position).OnPlay(() => PlaySound(_moveToNodeSound, _moveToNodeSoundTimeout));

            _dashboard.SetCardsStartPosition(centerPoint.position);

            await Timeline.Delay(.5f);

            _ = chosenDevice.FadeOut();
            _ = _devicesGrid.FadeOut(0f, 1, chosenDevice);
            await _dashboard.ShowAsync();

            if (IsFirstRound)
            {
                _taskPanel.ShowForDelay();
                _ = _progress.GetComponentInChildren<CanvasGroup>().DOFade(1, 1);

                await _dashboard.ShowTutorial(centerPoint);
            }

            await _dashboard.ShowButtonsPanel();
            _ = _dashboard.PulseButtonsAsyncOnce();
        }

        private async Promise ChooseDeviceOnGridAsync()
        {
            while (true)
            {
                _chosenDevice = _devicesGrid.GetRandomDevice();

                if (_chosenDevice != null)
                {
                    _chosenDevice.SetChosen(true);
                    break;
                }

                await Timeline.SkipFrame();
            }
        }

        private Tween GoBackToGrid()
        {
            return DOTween.Sequence()
                   .Append(_devicesGrid.FadeIn())
                   .Join(_chosenDevice.RestoreOnGrid())
                   .AppendCallback(() =>
                   {
                       //_devicesGrid.transform.SetParent(null);
                       _devicesGrid.RestoreParent();
                       _chosenDevice.SetChosen(false, _devicesGrid.transform);
                       _devicesGrid.GetComponent<BGScroller>().Stop(false);
                   })
                   ;
        }

        private void PlaySound(ShootSoundOnEvent sound, float timeout = 0f)
        {
            if (sound == null) return;
            sound.Play(timeout);
        }
    }
}