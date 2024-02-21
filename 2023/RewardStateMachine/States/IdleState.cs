using Cysharp.Threading.Tasks;
using Project.Utils.StateMachineMono;
using UnityEngine;

public class IdleState : GameState
{
    public IdleState(params object[] data) : base(data) { }

    public async override UniTask Enter()
    {
        Debug.Log($"Enter to state: {this.GetType()}");
    }

    public async override UniTask Exit()
    {
        Debug.Log($"Exit from state: {this.GetType()}");
    }

    public async override UniTask Update() { }

    public async override UniTask InputEvent(GameEvent gameEvent, params object[] data)
    {
        if (gameEvent == GameEvent.GettingReward)
        {
            await _stateMachine.SetState(new GettingRewardState(data));
        }
        else if (gameEvent == GameEvent.GenerateTokens)
        {
            await _stateMachine.SetState(new GenerateRewardTokensState(data));
        }
    }
}