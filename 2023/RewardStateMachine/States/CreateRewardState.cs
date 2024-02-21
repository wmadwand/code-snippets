using Cysharp.Threading.Tasks;
using Project.Controller;
using Project.Data.Google;
using Project.Gameplay.Data.Campaign;
using Project.Network;
using Project.Utils.StateMachineMono;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CreateRewardState : GameState
{
    private object _rewards;
    private RewardStateMachine _agents;

    //--------------------------------------------------------------

    public CreateRewardState(params object[] data) : base(data)
    {
        _rewards = data[0];
    }

    public async override UniTask Enter()
    {
        Debug.Log($"Enter to state: {this.GetType()}");

        _agents = (RewardStateMachine)_stateMachine;
        CreateReward(_rewards);

        await _stateMachine.SetState(new UpdateRewardState(_contextData));
    }

    public async override UniTask Exit() { Debug.Log($"Exit from state: {this.GetType()}"); }

    public async override UniTask Update() { }

    public async override UniTask InputEvent(GameEvent gameEvent, params object[] data) { }

    //--------------------------------------------------------------

    private void CreateReward(object data)
    {
        var allRewards = data as Dictionary<string, IGrouping<string, IGrouping<GroupStruct, RewardData>>>;
        var server = new Server();

        foreach (var brand in allRewards)
        {
            var campaigns = brand.Value.ToArray();
            var campaign = campaigns[0];
            var rewards = campaign.ToArray();

            var agent = CreateAgent(rewards, Game.Config.Meta.CampaignTokenViews, server);
            _agents.Agents.All.Add(agent);
        }
    }

    private CampaignAgent CreateAgent(RewardData[] rewardData, int tokenForCampaignCount, Server server)
    {
        var campaign = new Campaign(rewardData[0]);
        var tokens = new CampaignTokens(rewardData[0], tokenForCampaignCount);
        var rewards = new CampaignRewards(rewardData, new CampaignRewardsServer(server));

        return new CampaignAgent(campaign, tokens, rewards);
    }
}