using Cysharp.Threading.Tasks;
using Project.Controller;
using Project.Gameplay.Data.Campaign;
using Project.Utils.StateMachineMono;
using System.Linq;
using UnityEngine;

public class GettingRewardState : GameState
{
    private RewardStateMachine _campaignAgents;
    private CampaignAgent _agent;
    private int _waitBeforeAddNewCamapign = 1;

    //--------------------------------------------------------------

    public GettingRewardState(params object[] data) : base(data)
    {
        _agent = (CampaignAgent)data[0];
    }

    public async override UniTask Enter()
    {
        Debug.Log($"Enter to state: {this.GetType()}");

        _campaignAgents = (RewardStateMachine)_stateMachine;

        await UniTask.WaitUntil(() => _agent.IsCompleted);

        _campaignAgents.Analytics.FinishCampaign(_agent);
        _campaignAgents.Agents.Active.Remove(_agent);

        var completedAgents = _campaignAgents.Agents.Used.Where(agent => agent.IsCompleted).ToList();
        if (completedAgents.Count < _campaignAgents.Agents.All.Count)
        {
            await UniTask.Delay(_waitBeforeAddNewCamapign * 1000);
            await _stateMachine.SetState(new GenerateRewardTokensState());
        }
        else
        {
            await _stateMachine.SetState(new GameCompletedState());
        }
    }

    public async override UniTask Exit()
    {
        Debug.Log($"Exit from state: {this.GetType()}");
    }

    public async override UniTask Update() { }

    public async override UniTask InputEvent(GameEvent eventt, params object[] data) { }
}