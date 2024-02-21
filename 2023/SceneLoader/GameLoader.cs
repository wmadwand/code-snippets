using Cysharp.Threading.Tasks;
using Project.Analytics;
using Project.Data;
using Project.Utils;
using UnityEngine;

namespace Project.Controller
{
    public class GameLoader : MonoSingleton<GameLoader>
    {
        [SerializeField] private Camera _uiCommonCamera;
        [SerializeField] private Canvas _splashScreenCanvas;
        private ISceneManagement _scene;

        //--------------------------------------------------------------

        public async UniTask StartGame()
        {
            await UniTask.Delay(500);
            _splashScreenCanvas.enabled = false;

            //TODO: check Game.Config.PlayerProfile instead
            if (!UserCredentialsLocalStorage.Instance.TryGetValue(UserCredentials.Email, out _))
            {
                await PromptEmail();
            }
            AmplitudeAnalytics.Instance.Init();

            await _scene.LoadAsync(Game.Config.App.Scenes.loading);

            var UIScene = await _scene.LoadAsync(Game.Config.App.Scenes.UI, setActive: false);
            //TODO: deal with it
            var UISceneContext = (UISceneContext)_scene.GetContext(UIScene);
            await UISceneContext.Run(_uiCommonCamera);

            var mainScene = await _scene.LoadAsync(Game.Config.App.Scenes.map);
            var mainSceneContext = _scene.GetContext(mainScene);
            await mainSceneContext.Run(UISceneContext.UIController);

            _ = _scene.UnloadAsync(Game.Config.App.Scenes.loading);
        }

        public async UniTask OpenAvatarEditor()
        {
            await _scene.LoadAsync(Game.Config.App.Scenes.loading);

            //TODO: deal with it
            var gameSceneContext = (MainSceneContext)_scene.GetContext(Game.Config.App.Scenes.map);
            if (gameSceneContext)
            {
                gameSceneContext.SetStuffActive(false, _uiCommonCamera);
            }

            var avatarScene = await _scene.LoadAsync(Game.Config.App.Scenes.avatar, setActive: false);
            var avatarSceneContext = _scene.GetContext(avatarScene);

            await avatarSceneContext.Run(_uiCommonCamera);

            _ = _scene.UnloadAsync(Game.Config.App.Scenes.loading);
        }

        public async UniTask CloseAvatarEditor(string avatarURL)
        {
            await _scene.LoadAsync(Game.Config.App.Scenes.loading);

            //TODO: deal with it
            var gameSceneContext = (MainSceneContext)_scene.GetContext(Game.Config.App.Scenes.map);
            await gameSceneContext.LoadAvatarModelAsync(avatarURL);

            await _scene.UnloadAsync(Game.Config.App.Scenes.avatar);
            await _scene.UnloadAsync(Game.Config.App.Scenes.loading, 500);

            _scene.SetActiveScene(Game.Config.App.Scenes.map);
            gameSceneContext?.SetStuffActive(true, _uiCommonCamera);
        }

        //--------------------------------------------------------------

        private void Awake()
        {
            _scene = new SceneManagement();
        }

        private async UniTask PromptEmail()
        {
            var loginScene = await _scene.LoadAsync(Game.Config.App.Scenes.login);
            var loginSceneContext = _scene.GetContext(loginScene);

            await loginSceneContext.Run();
            await _scene.UnloadAsync(Game.Config.App.Scenes.login);
        }
    }
}