using Cysharp.Threading.Tasks;
using Project.Avatar;
using UnityEngine;

public class AvatarSceneContext : MonoBehaviour, ISceneContext
{
    [SerializeField] private AvatarWebView _avatarWebView;
    [SerializeField] private GameObject _eventSystemGO;

    public async UniTask Run(params object[] data)
    {
        var uiCommonCamera = data[0] as Camera;

        _avatarWebView.UICamera.enabled = false;
        _eventSystemGO.SetActive(false);

        await _avatarWebView.Load(uiCommonCamera);
        uiCommonCamera.enabled = true;
    }
}