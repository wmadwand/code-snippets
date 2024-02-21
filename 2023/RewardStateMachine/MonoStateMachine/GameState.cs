using Cysharp.Threading.Tasks;

namespace Project.Utils.StateMachineMono
{
    public enum GameEvent
    {
        GettingReward,
        GenerateTokens
    }

    public class GameContext
    {
        public object context;
    }

    public abstract class GameState
    {
        protected MonoStateMachine _stateMachine;
        protected object[] _contextData;

        public GameState(params object[] data)
        {
            _contextData = data;
        }

        public void SetContext(MonoStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public abstract UniTask Enter();
        public abstract UniTask Exit();
        public abstract UniTask Update();
        public abstract UniTask InputEvent(GameEvent gameEvent, params object[] data);
    } 
}