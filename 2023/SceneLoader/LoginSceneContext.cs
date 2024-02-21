using Cysharp.Threading.Tasks;
using UnityEngine;

public class LoginSceneContext : MonoBehaviour, ISceneContext
{
    [SerializeField] private PromptEmail _promptEmail;
    [SerializeField] private GameObject _eventSystemGO;

    public async UniTask Run(params object[] data)
    {
        _eventSystemGO.SetActive(false);
        await _promptEmail.Run();
    }
}