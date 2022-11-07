using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "BowmanSettingsInstaller", menuName = "MiniGames/Bowman/BowmanSettingsInstaller")]
public class BowmanSettingsInstaller : ScriptableObjectInstaller<BowmanSettingsInstaller>
{
    public BowmanSettings gameSettings;

    public override void InstallBindings()
    {
        Container.BindInstance(gameSettings).IfNotBound();
    }
}