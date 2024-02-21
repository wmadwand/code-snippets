using Cysharp.Threading.Tasks;
using ReadyPlayerMe.Core;
using ReadyPlayerMe.WebView;
using Project.Controller;
using Project.Utils;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Avatar
{
    public class AvatarWebView : MonoSingleton<AvatarWebView>
    {
        public static event Action OnHide;

        public Camera UICamera => _uiCamera;

        [SerializeField] private Camera _uiCamera;

        [SerializeField] private Canvas _canvas;
        [SerializeField] private GameObject _content;
        [SerializeField] private WebViewPanel _webView;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _loadDefaultURL;
        [SerializeField] private string _defaultURL = "https://models.readyplayer.me/64ece5dd1db75f90dcf7ef2c.glb";

        //--------------------------------------------------------------

        public async UniTask Load(Camera camera)
        {
            _canvas.worldCamera = camera;

            _webView.LoadWebView();
            _webView.SetVisible(false);

            if (!Application.isEditor)
            {
                await UniTask.WaitUntil(() => _webView.Loaded);
                await UniTask.Delay(3000);
            }
            else
            {
                await UniTask.Delay(500);
            }

            _webView.SetVisible(true);
        }

        //--------------------------------------------------------------

        private void Start()
        {
            _webView.OnAvatarCreated.AddListener(OnAvatarCreated);
            _loadDefaultURL.onClick.AddListener(OnLoadDefaultURL);
        }

        public async UniTask OnCloseWebView()
        {
            _webView.SetVisible(false);
            _content.SetActive(false);

            await GameLoader.Instance.CloseAvatarEditor(null);
        }

        private void OnAvatarCreated(string url)
        {
            UniTask.Void(async () => { await OnAvatarCreatedAsync(url); });
        }

        private async UniTask OnAvatarCreatedAsync(string avatarURL)
        {
            _content.gameObject.SetActive(false);
            OnHide?.Invoke();
            //ClearCache();

            await GameLoader.Instance.CloseAvatarEditor(avatarURL);
        }

        private void OnLoadDefaultURL()
        {
            UniTask.Void(async () => { await OnAvatarCreatedAsync(_defaultURL); });
        }

        private void Failed(object sender, FailureEventArgs args)
        {
            Debug.LogError(args.Type);
        }

        private void OnDestroy()
        {
            _webView.OnAvatarCreated.RemoveListener(OnAvatarCreated);
            _loadDefaultURL.onClick.RemoveListener(OnLoadDefaultURL);
        }

        private void ClearCache()
        {

        }

    }
}