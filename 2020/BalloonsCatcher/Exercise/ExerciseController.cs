using System.Collections.Generic;
using DataModels.UserData;
using ExerciseGeneration;
using Coconut.Core.Asyncs;
using UnityEngine;
using Zenject;

namespace MiniGames.Games.BalloonsCatcher
{
    public class ExerciseController : MonoBehaviour
    {
        [Header("Base")]
        public Table table;
        [SerializeField] private BalloonExerciseFx _fx;

        private MultiplierQueueExercise _exercise;
        private EzBalloon _correctBalloon;

        private PromiseCompletionSource _roundCompletionSource;

        // Inject
        [Inject] private UserProgressConfig _userProgress;
        
        private void Awake()
        {
            List<int> completedNumbers = _userProgress.data.GetCompletedNumbers();
            _exercise = new MultiplierQueueExercise(completedNumbers);
        }

        public void GenerateExercise()
        {
            _exercise.Generate();
            table.Init(_exercise.result);
        }

        public int Dequeue()
        {
            return _exercise.Dequeue();
        }

        public void ReportClick(EzBalloon ezBalloon)
        {
            if (_correctBalloon != null)
            {
                int result = _correctBalloon.number * ezBalloon.number;

                if (_exercise.IsCorrect(result))
                {
                    gameObject.Promise(Match, _correctBalloon, ezBalloon);
                    _correctBalloon = null;
                }
                else
                {
                    gameObject.Promise(Dismatch, _correctBalloon, ezBalloon);
                    _correctBalloon = null;
                }
            }
            else
            {
                if (_exercise.HasSecondMultiplier(ezBalloon.number))
                {
                    gameObject.Promise(GetCorrectBall, ezBalloon);
                }
                else
                {
                    gameObject.Promise(_fx.Incorrect, ezBalloon.transform);
                }
            }
        }

        private async Promise GetCorrectBall(EzBalloon ezBalloon)
        {
            _correctBalloon = ezBalloon;
            await _fx.Correct(ezBalloon.transform);
            if (_correctBalloon != null)
            {
                _correctBalloon.ChangeSortingOrder();
            }
        }

        private async Promise Match(EzBalloon ezBalloon1, EzBalloon ezBalloon2)
        {
            ezBalloon1.StopAnimation();
            ezBalloon2.StopAnimation();
            await _fx.Match(ezBalloon1.transform, ezBalloon2.transform, table);
            CompleteRound();
        }

        private async Promise Dismatch(EzBalloon ezBalloon1, EzBalloon ezBalloon2)
        {
            await _fx.Dismatch(ezBalloon1.transform, ezBalloon2.transform);
        }

        public async Promise Intro()
        {
            await _fx.Intro(table);
        }

        public async Promise WaitCompleteRound()
        {
            await (_roundCompletionSource = new PromiseCompletionSource()).promise;
        }

        private void CompleteRound()
        {
            _roundCompletionSource?.SetResult();
            _roundCompletionSource = null;
        }
    }
}