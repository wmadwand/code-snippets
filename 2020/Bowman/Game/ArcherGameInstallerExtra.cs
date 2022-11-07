using MiniGames.Games.Bowman;
using Coconut.Game.Movers;
using UnityEngine;
using Zenject;

public class BowmanInstallerExtra : MonoInstaller
{
    [SerializeField] private GameObject _projectileTemplate;
    [SerializeField] private LerpMoverService _lerpService;

    public override void InstallBindings()
    {
        Container.BindInstance(_lerpService);
        Container.BindFactory<Projectile, Projectile.Factory>()
            .FromComponentInNewPrefab(_projectileTemplate)
            .UnderTransformGroup("Projectiles")
        ;

    }
}