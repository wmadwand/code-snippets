using Cysharp.Threading.Tasks;
using Project.Controller;
using System;
using System.Threading;

public class CheckEnoughTokensAroundPlayer : TokenServiceBase
{
    public static event Action OnCheckEnoughTokensAroundPlayer;

    public override async UniTask Init(ITokenViewSpawner context)
    {
        Cancel();
        _cts = new CancellationTokenSource();
        _context = context;
        await UniTask.Yield();
    }

    public override async UniTask RunHandler()
    {
        while (!_cts.IsCancellationRequested)
        {
            OnCheckEnoughTokensAroundPlayer?.Invoke();
            await UniTask.Delay(1000 * Game.Config.Meta.CheckEnoughTokensAroundPlayerPeriod, cancellationToken: _cts.Token);
        }
    }
}