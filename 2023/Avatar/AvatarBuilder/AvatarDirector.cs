using Cysharp.Threading.Tasks;
using System.Collections.Generic;

namespace Project.Avatar
{
    public class AvatarDirector
    {
        public IAvatarBuilder Builder { set { _builder = value; } }
        private IAvatarBuilder _builder;

        //--------------------------------------------------------------

        public async UniTask BuildOnly3D()
        {
            await _builder.BuildModel();
        }

        public async UniTask BuildBoth3DAnd2D()
        {
            List<UniTask> tasks = new List<UniTask>
            {
                _builder.BuildModel(),
                _builder.BuildSprite()
            };

            await UniTask.WhenAll(tasks);
        }
    }
}
