using Cysharp.Threading.Tasks;
using Project.Controller;
using UnityEngine;

namespace Project.Applikation
{
    public class ApplicationLoader : MonoBehaviour
    {
        [SerializeField] private GameLoader _gameLoader;

        private void Start()
        {
            UniTask.Void(async () =>
            {
                await Game.Init();

                if (Game.IsInited)
                {
                    await _gameLoader.StartGame();
                }
                else
                {
                    Debug.LogError("Failed continue load. Because game is not inited");
                }
            });
        }
    }
}