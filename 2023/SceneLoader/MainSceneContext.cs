using Cysharp.Threading.Tasks;
using Project.Controller;
using Project.Data;
using Project.Gameplay.MainPlayer;
using UnityEngine;

public class MainSceneContext : MonoBehaviour, ISceneContext
{
    [SerializeField] private GameController _gameController;
    [SerializeField] private GameObject _eventSystemGO;

    public async UniTask Run(params object[] data)
    {
        _eventSystemGO.SetActive(false);
        var uiController = data[0] as UIController;
        await _gameController.Run(uiController);
    }

    public void SetStuffActive(bool value, Camera uiCommonCamera)
    {
        uiCommonCamera.enabled = value;
        _gameController.PlayerCamera.Camera.enabled = value;
        UIController.Instance.SetUIElemetsActive(value);
    }

    public async UniTask LoadAvatarModelAsync(string avatarURL)
    {
        if (!string.IsNullOrEmpty(avatarURL))
        {
            //TODO: move this scope out from the class
            {
                UserCredentialsLocalStorage.Instance.Save(UserCredentials.AvatarURL, avatarURL);

                var avatarGenerator = _gameController.AvatarGenerator;
                var avatarData = await avatarGenerator.Load(avatarURL);

                if (avatarData?.model)
                {
                    var avaCopy = Instantiate(avatarData.model, Player.Instance.transform, false);
                    Player.Instance.SetAvatar(avaCopy);

                    UIController.Instance.OnAvatarCreatedAndBackToGameScene();
                }
            }
        }
    }
}