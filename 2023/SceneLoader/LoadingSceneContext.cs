using Cysharp.Threading.Tasks;
using UnityEngine;

public class LoadingSceneContext : MonoBehaviour, ISceneContext
{
    [SerializeField] private Canvas _canvas;    

    public void Init(Camera uiCamera)
    {
        _canvas.worldCamera = uiCamera;
    }

    public async UniTask Run(params object[] data) { await UniTask.Yield(); }
}
