using MiniGames.Core;
using Coconut.Core.Asyncs;
using UnityEngine;
using Zenject;

namespace MiniGames.Games.FBLikesPower
{
    public class LikesRound : IGameScenario
    {
        //[Inject] private IEmitter _emitter;
        [Inject] private FBLikesPowerController _controller;
        [Inject] private LikesMessageBus _messageBus;

        private PromiseCompletionSource _roundComplete;

        public async Promise RunScenario()
        {
            _controller.StartRound();
            //_emitter.Play();
            _messageBus.correctAnswer.receive += OnCorrectAnswer;
            await (_roundComplete = new PromiseCompletionSource()).promise;
            _messageBus.correctAnswer.receive -= OnCorrectAnswer;
            // todo: kill balloons?
            //_emitter.Stop();
        }

        private void OnCorrectAnswer()
        {
            Debug.Log("MATCH !!!");
            _roundComplete.SetResult();
        }
    }
}