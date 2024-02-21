using Cysharp.Threading.Tasks;
using Project.Controller;
using UnityEngine;

public class UISceneContext : MonoBehaviour, ISceneContext
{
    public UIController UIController => _uiController;

    [SerializeField] private UIController _uiController;
    [SerializeField] private GameObject _eventSystemGO;

    public async UniTask Run(params object[] data)
    {
        _eventSystemGO.SetActive(false);
        var uiCommonCamera = data[0] as Camera;
        _uiController.SetCamera(uiCommonCamera);

        await UniTask.Yield();
    }
}