using Cysharp.Threading.Tasks;

public interface ISceneContext
{
    UniTask Run(params object[] data);
}