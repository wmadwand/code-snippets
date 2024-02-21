using Cysharp.Threading.Tasks;

namespace Project.Avatar
{
    public interface IAvatarBuilder
    {
        UniTask BuildModel();
        UniTask BuildSprite();
        AvatarData Data { get; }
    }

    public class AvatarBuilderDefault : IAvatarBuilder
    {
        private AvatarData _avatar;
        private string _url;
        private readonly AvatarObject _model;
        private readonly AvatarSprite _sprite;

        public AvatarData Data => _avatar;

        //--------------------------------------------------------------

        public AvatarBuilderDefault(string url, AvatarObject model, AvatarSprite sprite)
        {
            _avatar = new AvatarData(url);

            _url = url;
            _model = model;
            _sprite = sprite;
        }

        public async UniTask BuildModel()
        {
            var model = await _model.Load(_url);
            _avatar.model = model?.Avatar;
        }

        public async UniTask BuildSprite()
        {
            var sprite = await _sprite.Load(_url);
            _avatar.sprite = sprite;
        }
    }
}
