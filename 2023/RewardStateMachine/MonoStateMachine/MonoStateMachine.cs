using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Project.Utils.StateMachineMono
{
    public abstract class MonoStateMachine : MonoBehaviour
    {
        protected GameState State = null;

        public async UniTask SetState(GameState state)
        {
            if (State != null)
            {
                await State.Exit();
            }

            State = state;
            State.SetContext(this);

            await State.Enter();
        }

        public async UniTask UpdateState()
        {
            if (State != null)
            {
                await State.Update();
            }
            else
            {
                Debug.LogError($"{GetType()} State == null");
            }
        }

        public async UniTask InputEvent(GameEvent gameEvent, params object[] data)
        {
            if (State != null)
            {
                await State.InputEvent(gameEvent, data);
            }
            else
            {
                Debug.LogError($"{GetType()} State == null");
            }
        }
    }
}