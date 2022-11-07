using UnityEngine;
using Zenject;

namespace MiniGames.Games.SpaceDefence.Game
{
    [CreateAssetMenu(fileName = "MiniGames/SpaceDefence/SpaceDefenceSettingsInstaller", menuName = "Installers/SpaceDefenceSettingsInstaller")]
    public class SpaceDefenceSettingsInstaller : ScriptableObjectInstaller<SpaceDefenceSettingsInstaller>
    {
        public Shield.Shield.Settings shieldSettings;

        public override void InstallBindings()
        {
            Container.BindInstance(shieldSettings).IfNotBound();
        }
    }
}