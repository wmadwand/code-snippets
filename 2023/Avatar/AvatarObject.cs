using Cysharp.Threading.Tasks;
using ReadyPlayerMe.Core;

namespace Project.Avatar
{
    public class AvatarObject
    {
        private readonly AvatarObjectLoader _loader;

        public AvatarObject(AvatarObjectLoader loader)
        {
            _loader = loader;
        }

        public async UniTask<CompletionEventArgs> Load(string url)
        {
            var result = await _loader.LoadAvatarAsync(url).AsUniTask();
            return result as CompletionEventArgs;
        }
    }
}