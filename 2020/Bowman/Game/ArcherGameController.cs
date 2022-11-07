using System.Collections.Generic;
using DataModels.UserData;
using DG.Tweening;
using ExerciseGeneration;
using Coconut.Core.Asyncs;
using Coconut.Game;
using Coconut.Game.Alignment;
using Coconut.Game.TimescaleUtils;
using UnityEngine;
using Utils;
using Zenject;

namespace MiniGames.Games.Bowman
{
    public class BowmanController : MonoBehaviour
    {
        [SerializeField] private ExerciseFlag _exerciseSceneFlag;
        [SerializeField] private TargetsController _targetsController;
        [SerializeField] private BowmanExerciseFx _fx;

        public AlignToCamera centerPoint;

        [Header("Audio")]
        [SerializeField] private ShootSoundOnEvent _showNumberSound;
        [SerializeField] private ShootSoundOnEvent _changeTextOnFlagSound;

        // Inject
        [Inject] private Player _player;
        [Inject] private UserProgressConfig _userProgress;
        [Inject] private BowmanSettings _gameSettings;

        private PromiseCompletionSource _roundComplete;
        private ResultVariantsExercise _exercise;
        private bool _isFirstRound = true;

        private void Awake()
        {
            List<int> completedNumbers = _userProgress.data.GetCompletedNumbers();
            _exercise = new ResultVariantsExercise(completedNumbers, _gameSettings.correctNumberOffsetPercent);
        }

        public void StartRound()
        {
            gameObject.CancelPromise(StartRoundAsync);
            gameObject.Promise(StartRoundAsync);
        }

        public async Promise WaitCompleteRound()
        {
            await (_roundComplete = new PromiseCompletionSource()).promise;
        }

        public void InitRound()
        {
            ResetSceneObjects();
            GenerateExercise();
        }

        public async Promise Intro()
        {
            using (var accessor = Timescale.CreateAccessor())
            {
                accessor.timeScale = 0.25f;
                //_exerciseSceneFlag.SetActive(false);
                await _fx.Intro(_exerciseSceneFlag);
            }
        }

        public void GenerateExercise()
        {
            _exercise.Generate();
            
            //_changeTextOnFlagSound.Play();
            _exerciseSceneFlag.Init(_exercise.expression);

            List<int> results = _exercise.GetInitialResults(_targetsController.targetCount);
            _targetsController.Init(OnCheckExerciseAnswer, results);
        }

        //---------------------------------------------------

        private void OnCheckExerciseAnswer(Target target)
        {
            gameObject.Promise(CheckExerciseAnswerAsync, target);
        }

        private async Promise CheckExerciseAnswerAsync(Target target)
        {
            if (_exercise.expression.IsCorrect(target.Number))
            {
                await CorrectAnswer(target);
                CompleteRound();
            }
            else
            {
                await IncorrectAnswer(target);
            }
        }

        private async Promise IncorrectAnswer(Target target)
        {
            using (var accessor = Timescale.CreateAccessor())
            {
                accessor.timeScale = 0.25f;

                await _fx.Incorrect(_exerciseSceneFlag, target);

                var newTargetNumber = _exercise.DequeueIncorrectResult();
                target.SetNumber(newTargetNumber);
                await target.ShowNumber();

                _player.ResetView();
            }
        }

        private async Promise CorrectAnswer(Target target)
        {
            using (var accessor = Timescale.CreateAccessor())
            {
                accessor.timeScale = 0.25f;

                await _fx.Correct(_exerciseSceneFlag, target);

                _targetsController.ResetTargets();
                _player.ResetView();
            }
        }

        private void ResetSceneObjects()
        {
            _player.SetShootingActive(false);
            _targetsController.StopTargets();
            _targetsController.ResetTargets();
        }

        private async Promise StartRoundAsync()
        {
            using (var accessor = Timescale.CreateAccessor())
            {
                accessor.timeScale = 0.25f;

                _exerciseSceneFlag.SetActive(true);
                _exerciseSceneFlag.SetTextActive(true);

                //_showNumberSound.Play();

                await _targetsController.ShowNumbers();
                await _targetsController.RunTargets();

                _player.SetShootingActive(true);

                if (_isFirstRound)
                {
                    //_taskPanel.ShowForDelay();
                    //_ = _progress.GetComponentInChildren<CanvasGroup>().DOFade(1, 1);

                    await _fx.ShowTutorial();
                    _isFirstRound = false;
                }
            }
        }

        private void CompleteRound()
        {
            _roundComplete?.SetResult();
            _roundComplete = null;
        }
    }
}