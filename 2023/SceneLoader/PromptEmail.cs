using Cysharp.Threading.Tasks;
using Project.Data;
using Project.UI;
using Surfer;
using UnityEngine;

public class PromptEmail : MonoBehaviour
{
    [SerializeField] private PromptEmailPopup _promptEmailPrefab;
    [SerializeField] private Transform _canvas;

    //--------------------------------------------------------------

    public async UniTask Run()
    {
        SurferManager.I.OpenPrefabState(_promptEmailPrefab.gameObject, _canvas, customData: new object[] { });

        while (IsOpen()) { await UniTask.Delay(500); }
    }

    private bool IsOpen()
    {
        return !UserCredentialsLocalStorage.Instance.TryGetValue(UserCredentials.Email, out _)
               || SurferManager.I.IsOpen(SUConsts.PROMPT_EMAIL, 0);
    }
}