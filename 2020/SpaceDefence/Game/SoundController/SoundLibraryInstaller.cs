using UnityEngine;
using Zenject;

namespace MiniGames.Games.SpaceDefence.Game
{
    [CreateAssetMenu(fileName = "MiniGames/SpaceDefence/SoundLibraryInstaller", menuName = "Installers/SoundLibraryInstaller")]
    public class SoundLibraryInstaller : ScriptableObjectInstaller<SoundLibraryInstaller>
    {        
        public SoundLibrary soundLibrary;

        public override void InstallBindings()
        {            
            Container.BindInstance(soundLibrary).IfNotBound();
        }
    }
}