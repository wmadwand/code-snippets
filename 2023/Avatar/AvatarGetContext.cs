using Cysharp.Threading.Tasks;
using ReadyPlayerMe.Core;

namespace Project.Avatar
{
    public class AvatarGetContext
    {
        private const string TAG = nameof(AvatarGetContext);

        public int Timeout { get; set; }
        private readonly bool _avatarCachingEnabled;

        public AvatarGetContext(bool avatarCachingEnabled)
        {
            _avatarCachingEnabled = avatarCachingEnabled;
        }

        //--------------------------------------------------------------

        public async UniTask<AvatarContext> Get(string url)
        {
            var context = new AvatarContext { Url = url, AvatarCachingEnabled = _avatarCachingEnabled };
            var executor = new OperationExecutor<AvatarContext>(new IOperation<AvatarContext>[]
            {
                new UrlProcessor(),
                new MetadataDownloader()
            });
            executor.ProgressChanged += Executor_ProgressChanged;
            executor.Timeout += Timeout;

            try
            {
                context = await executor.Execute(context).AsUniTask();
            }
            catch (CustomException exception)
            {
                SDKLogger.Log(TAG, $"Failed with error type: {exception.FailureType} and message: {exception.Message}");
            }

            return context;
        }

        private void Executor_ProgressChanged(float arg1, string arg2)
        {
            //throw new System.NotImplementedException();
        }
    }
}