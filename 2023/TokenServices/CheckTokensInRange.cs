using Cysharp.Threading.Tasks;
using Project.Controller;
using Project.Gameplay.MainPlayer;
using System.Threading;

public class CheckTokensInRange : TokenServiceBase
{
    private ITokenViewFactory _factory;

    //--------------------------------------------------------------

    public override async UniTask Init(ITokenViewSpawner context)
    {
        Cancel();
        _cts = new CancellationTokenSource();
        _context = context;

        //TODO: rework it - pass from ITokenViewSpawner
        _factory = GetComponentInParent<ITokenViewFactory>();
        await UniTask.Yield();
    }

    public override async UniTask RunHandler()
    {
        while (!_cts.IsCancellationRequested)
        {
            for (int i = 0; i < _context.Collection.Count; i++)
            {
                var mapToken = _context.Collection[i];
                if (mapToken == null) { continue; }

                var distance = Player.Instance.Coordinates.DistanceFromPoint(mapToken.token.coordinates);
                if (mapToken.IsVisible && distance > Game.Config.Meta.PlayerZoneRadius)
                {
                    _factory.Return(mapToken.view);
                    mapToken.view = null;
                }
                else if (!mapToken.IsVisible && distance < Game.Config.Meta.PlayerZoneRadius)
                {
                    var tokenView = _factory.Get();
                    tokenView.Init(mapToken.token, mapToken.agent);
                    mapToken.view = tokenView;
                }

                await UniTask.Yield(cancellationToken: _cts.Token);
            }

            await UniTask.WaitForSeconds(_updateRate, cancellationToken: _cts.Token);
        }
    }
}