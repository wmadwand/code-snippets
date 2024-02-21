using Cysharp.Threading.Tasks;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project.Utils
{
    public interface ISceneManagement
    {
        ISceneContext GetContext(Scene scene);
        ISceneContext GetContext(string sceneName);
        UniTask<Scene> LoadAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Additive, bool setActive = true);
        UniTask UnloadAsync(string sceneName, int delay = 0);
        void SetActiveScene(string name);
    }

    public class SceneManagement : ISceneManagement
    {
        public ISceneContext GetContext(Scene scene)
        {
            var go = scene.GetRootGameObjects().FirstOrDefault(x => x.GetComponent<ISceneContext>() != null);
            return go?.GetComponent<ISceneContext>();
        }

        public ISceneContext GetContext(string sceneName)
        {
            ISceneContext result = null;
            var scene = SceneManager.GetSceneByName(sceneName);
            if (scene.isLoaded)
            {
                var go = scene.GetRootGameObjects().FirstOrDefault(x => x.GetComponent<ISceneContext>() != null);
                result = go?.GetComponent<ISceneContext>();
            }

            return result;
        }

        public void SetActiveScene(string name)
        {
            var scene = SceneManager.GetSceneByName(name);
            if (scene.IsValid() && scene.isLoaded)
            {
                SceneManager.SetActiveScene(scene);
            }
            else
            {
                Debug.LogError($"Scene {scene} is not valid/loaded and cannot be active");
            }
        }

        public async UniTask<Scene> LoadAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Additive, bool setActive = true)
        {
            var scene = SceneManager.GetSceneByName(sceneName);
            if (!scene.isLoaded)
            {
                await SceneManager.LoadSceneAsync(sceneName, mode);
                scene = SceneManager.GetSceneByName(sceneName);
            }
            //else if (!scene.IsValid())
            //{
            //    Debug.LogError($"Scene {name} is not valid");
            //    return default;
            //}

            if (setActive)
            {
                SceneManager.SetActiveScene(scene);
            }

            return scene;
        }

        public async UniTask UnloadAsync(string sceneName, int delay = 0)
        {
            var scene = SceneManager.GetSceneByName(sceneName);
            if (scene.IsValid() && scene.isLoaded)
            {
                await UniTask.Delay(delay);
                await SceneManager.UnloadSceneAsync(sceneName);
            }
            else
            {
                Debug.LogError($"Scene {sceneName} is not valid/loaded");
            }
        }
    }
}