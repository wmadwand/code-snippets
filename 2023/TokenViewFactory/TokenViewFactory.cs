using Project.Gameplay;
using Project.Utils;
using UnityEngine;

public interface ITokenViewFactory
{
    TokenView Get();
    void Return(TokenView view);
}

public class TokenViewFactory : MonoBehaviour, ITokenViewFactory
{
    [SerializeField] private TokenView _prefab;
    [SerializeField] private Transform _parent;

    private IMonoObjectPool<TokenView> _pool;

    //--------------------------------------------------------------

    public TokenView Get()
    {
        return _pool.Get();
    }

    public void Return(TokenView view)
    {
        _pool.Return(view);
    }

    //--------------------------------------------------------------

    private void Start()
    {
        //TODO: get values from GameSettings
        _pool = new MonoObjectPool<TokenView>(_prefab, 30, parent: _parent, actionOnReturn: (tokenView) => tokenView.DeInit());
    }

    private void OnDestroy()
    {
        _pool?.Dispose();
    }
}