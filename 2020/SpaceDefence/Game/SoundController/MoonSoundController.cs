using Coconut.SoundSystem;
using Zenject;

namespace MiniGames.Games.SpaceDefence.Game
{
    public class SpaceDefenceSoundController
    {
        [Inject] private ISoundController _soundController;
        [Inject] private SoundLibrary _soundLibrary;

        public void Play(AudioName name)
        {
            _soundController.Play(_soundLibrary.GetSound(name));
        }
    }
}