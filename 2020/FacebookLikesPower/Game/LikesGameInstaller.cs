using MiniGames.Core;
using MiniGames.Core.UI;
using MiniGames.UI;
using Coconut.Game.Scenes;
using UnityEngine;
using Zenject;
using Installers;

namespace MiniGames.Games.FBLikesPower
{
    public class FBLikesPowerInstaller : MonoInstaller
    {
        [SerializeField] private FBLikesPowerController _gameController;
        //[SerializeField] private Dialog _exitGameDialog;
        //[SerializeField] private Message _winGameMessage;
        //[SerializeField] private Fader _uiFader;

        public override void InstallBindings()
        {
            Container.Bind<LikesMessageBus>().AsSingle();
            Container.BindIFactory<IGameScenario>().FromFactory<RoundsFactory<LikesRound>>();
            //Container.BindInstance(_balloonSkinManager);
            Container.BindInstance(_gameController);

            //Container.Bind<IDialog<DialogResult>>().WithId(DialogsBindings.exitGameDialog).FromInstance(_exitGameDialog);
            //Container.Bind<IMessage>().WithId(DialogsBindings.winGameDialog).FromInstance(_winGameMessage);
            //Container.Bind<IFader>().WithId(FaderBindings.uiFader).FromInstance(_uiFader);
        }
    }
}