using Cysharp.Threading.Tasks;
using ReadyPlayerMe.Core;
using UnityEngine;

namespace Project.Avatar
{
    public class Avatar
    {
        private AvatarData _data;

        private readonly AvatarObject _object;
        private readonly AvatarSprite _sprite;
        private AvatarGetContext _avatarGetContext;

        private bool _avatarCachingEnabled;
        private AvatarLoaderSettings _avatarLoaderSettings;

        //--------------------------------------------------------------

        public Avatar(AvatarObject objectLoader, AvatarSprite spriteLoader)
        {
            _object = objectLoader;
            _sprite = spriteLoader;

            _avatarLoaderSettings = Resources.Load<AvatarLoaderSettings>(AvatarLoaderSettings.SETTINGS_PATH);
            _avatarCachingEnabled = _avatarLoaderSettings && _avatarLoaderSettings.AvatarCachingEnabled;

            //TODO: inject
            _avatarGetContext = new AvatarGetContext(_avatarCachingEnabled);
        }

        public async UniTask<AvatarData> Load(string url)
        {
            var director = new AvatarDirector();
            var builder = new AvatarBuilderDefault(url, _object, _sprite);
            director.Builder = builder;

            //await director.BuildBoth3DAnd2D();
            await director.BuildOnly3D();

            return builder.Data;
        }
    }
}