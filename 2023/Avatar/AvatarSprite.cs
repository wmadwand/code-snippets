using Cysharp.Threading.Tasks;
using ReadyPlayerMe.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Avatar
{
    public class AvatarSprite
    {
        private const string TAG = nameof(AvatarSprite);

        private readonly string[] blendShapeMeshes = { "Wolf3D_Avatar"/*, "Wolf3D_Teeth"*/ };
        private Dictionary<string, float> blendShapes = new Dictionary<string, float>
        {
            { "mouthSmile", 0.7f },
            { "viseme_aa", 0.5f },
            { "jawOpen", 0.3f }
        };

        private readonly AvatarRenderLoader _loader;
        private AvatarTextureFile _file;

        //--------------------------------------------------------------

        public async UniTask<Sprite> Load(string url) => await DownloadSprite(url);

        private async UniTask<Sprite> DownloadSprite(string url)
        {
            Texture2D texture = null;
            bool isCompleted = false;

            var avatarRenderer = new AvatarRenderLoader();
            avatarRenderer.OnCompleted += (textureRes) =>
            {
                texture = textureRes;
                isCompleted = true;
            };
            avatarRenderer.OnFailed += (type, message) =>
            {
                isCompleted = true;
                SDKLogger.Log(TAG, $"Failed with error type: {type} and message: {message}");
            };

            avatarRenderer.LoadRender(url, AvatarRenderScene.FullBodyPostureTransparent, blendShapeMeshes, blendShapes);
            await UniTask.WaitUntil(() => isCompleted);

            return CreateSprite(texture);
        }

        private Sprite LoadSpriteFromFile(string path)
        {
            var texture = _file.Load(path);

            return CreateSprite(texture);
        }

        //--------------------------------------------------------------

        private Sprite CreateSprite(Texture2D texture)
        {
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(.5f, .5f));
        }
    }
}