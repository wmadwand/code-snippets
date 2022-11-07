using MiniGames.Core;
using Coconut.Game.Movers;
using UnityEngine;

namespace MiniGames.Games.Bowman
{
    public class BowmanInstaller : MiniGameRoundInstaller<BowmanRound>
    {
        public override void InstallBindings()
        {
            Container.Bind<BowmanMessageBus>().AsSingle();
            Container.BindIFactory<IGameScenario>().FromFactory<RoundsFactory<BowmanRound>>();
        }
    }
}