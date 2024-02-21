using Cysharp.Threading.Tasks;
using Project.Controller;
using Project.Gameplay.MainPlayer;
using System.Threading;

public class CheckTokensDistanceToCollect : TokenServiceBase
{
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
            for (int i = 0; i < _context.Collection.Count; i++)
            {
                var mapToken = _context.Collection[i];
                if (mapToken != null && mapToken.IsVisible)
                {
                    var distance = Player.Instance.Coordinates.DistanceFromPoint(mapToken.token.coordinates);
                    mapToken.view.CanCollect = distance < Game.Config.Meta.InteractivityRadius;
                }

                await UniTask.Yield(cancellationToken: _cts.Token);
            }

            await UniTask.WaitForSeconds(_updateRate, cancellationToken: _cts.Token);
        }
    }

    public override async UniTask StopHandler()
    {
        Cancel();

        for (int i = 0; i < _context.Collection.Count; i++)
        {
            var mapToken = _context.Collection[i];
            if (mapToken != null && mapToken.view != null)
            {
                mapToken.view.CanCollect = true;
            }
        }

        await UniTask.Yield();
    }
}