using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "GameSettingsInstaller", menuName = "MiniGames/Likes/GameSettingsInstaller")]
public class GameSettingsInstaller : ScriptableObjectInstaller<GameSettingsInstaller>
{
    [SerializeField] private ProfileCollection _profileCollection;

    public override void InstallBindings()
    {
        Container.BindInstance(_profileCollection);
    }
}