using Cysharp.Threading.Tasks;
using Project.Controller;
using System.Threading;
using UnityEngine;

public abstract class TokenServiceBase : MonoBehaviour
{
    public bool IsRunning { get; private set; } = false;

    [SerializeField] protected float _updateRate = 0.4f;
    protected ITokenViewSpawner _context;
    protected CancellationTokenSource _cts;

    //--------------------------------------------------------------

    public abstract UniTask Init(ITokenViewSpawner context);
    public abstract UniTask RunHandler();
    public virtual async UniTask StopHandler() { }

    public void Stop()
    {
        StopHandler();
        Cancel();
        IsRunning = false;
    }

    public void Run()
    {
        RunHandler();
        IsRunning = true;
    }

    protected void Cancel()
    {
        _cts?.Cancel();
        //_cts?.Dispose();
        //_cts = null;        
    }
}