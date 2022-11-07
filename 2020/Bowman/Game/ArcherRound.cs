using MiniGames.Core;
using Coconut.Core.Asyncs;
using Zenject;

namespace MiniGames.Games.Bowman
{
    public class BowmanRound : IGameScenario
    {
        [Inject] private BowmanController _controller;

        public async Promise RunScenario()
        {
            _controller.InitRound();
            await _controller.Intro();
            _controller.StartRound();
            await _controller.WaitCompleteRound();
        }
    }
}