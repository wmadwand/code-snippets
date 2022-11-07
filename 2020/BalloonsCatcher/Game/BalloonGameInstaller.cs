using MiniGames.Core;
using UnityEngine;

namespace MiniGames.Games.BalloonsCatcher
{
    public class BalloonsCatcherInstaller : MiniGameRoundInstaller<BalloonRound>
    {
        [SerializeField] private Character _character;

        public override void InstallBindings()
        {
            base.InstallBindings();
            Container.BindInstance(_character);
        }
    }
}