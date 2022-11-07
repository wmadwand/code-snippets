using Coconut.Asyncs;
using MiniGames.Core;
using MiniGames.Games.SpaceDefence.Difficulties;
using MiniGames.Games.Ship;
using MiniGames.UI;
using UnityEngine;
using Zenject;

namespace MiniGames.Games.SpaceDefence.Game
{
    public class SpaceDefenceScenario : GameScenarioBase
    {
        [Header("Config")]
        public float tasksTimeout = 1f;

        [Header("References")]
        public MiniGameProgress progress;
        public SpaceDefenceGameController gameController;
        public SpaceDefenceTutorialController tutorialController;
        public SpaceDefenceDifficultyController difficultyController;

        public float timeout = 2f;
        public StartCountDown countDownCallback;
        public Animator countDownAnimator;
        public string countDownTrigger = "CountDown";

        [Inject] private SpaceDefenceSoundController _soundController;

        private bool _countDownFinish;
        private GameSettings _gameSettings;

        //-------------------------------------------------

        protected override AsyncChain Execute()
        {
            // todo: add intro cut scene
            // todo: add tutorial
            // todo: add outro cut scene

            countDownCallback.onFinish += CountDownCallbackOnFinish;

            return Planner.Chain()
                    .AddTimeout(timeout)
                    //.AddAction(StartAnimation)
                    //.AddAwait(WaitAnimationFinish)                    
                    .AddFunc(RunGame)
                ;
        }

        private AsyncState RunGame()
        {
            difficultyController.SetDifficulty();

            _gameSettings = difficultyController.CurrentDifficulty.gameSettings;

            var asyncChain = Planner.Chain();
            
            progress.SetMaxValue(difficultyController.CurrentDifficulty.GetTotalGameHazardCount());
            progress.SetValue(0, false);

            asyncChain
                      .AddFunc(RunTutorial)
                      .AddAction(_soundController.Play, AudioName.PopUpAppear)
                      .AddFunc(progress.Show)
                      .AddFunc(gameController.RunGame)
                      .AddAction(_soundController.Play, AudioName.PopUpHide)
                      .AddFunc(progress.Hide)
                      .AddTimeout(1f, true)
                      .AddAction(difficultyController.CompleteDifficulty)
                      ;

            return asyncChain;
        }

        private AsyncState RunTutorial()
        {
            if (!difficultyController.CurrentDifficulty.tutorial) return Planner.Empty();
            return tutorialController.RunTutorial();
        }

        private void CountDownCallbackOnFinish()
        {
            _countDownFinish = true;
        }

        private void StartAnimation()
        {
            _countDownFinish = false;
            countDownAnimator.SetTrigger(countDownTrigger);
        }

        private void WaitAnimationFinish(AsyncStateInfo state)
        {
            state.IsComplete = _countDownFinish;
        }
    }
}