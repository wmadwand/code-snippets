using Cysharp.Threading.Tasks;
using Project.Controller;
using Project.Gameplay.Data.Campaign;
using Project.Utils.StateMachineMono;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class GenerateRewardTokensState : GameState
{
    private RewardStateMachine campaignAgents;
    private bool _isRequiredExtraTokens = false;
    private CancellationTokenSource _cts;
    private int RequiredPointsCount => Game.Config.Meta.MaxCampaigns * Game.Config.Meta.CampaignTokenViews;

    //--------------------------------------------------------------

    public GenerateRewardTokensState(params object[] data) : base(data)
    {
        if (data.Length > 0)
        {
            try
            {
                _isRequiredExtraTokens = (bool)data[0];
            }
            catch (Exception)
            {
                Debug.LogError($"{ GetType()} {_isRequiredExtraTokens}");
                //throw;
            }
        }
    }

    public async override UniTask Enter()
    {
        Debug.Log($"Enter to state: {GetType().FullName}");

        campaignAgents = (RewardStateMachine)_stateMachine;

        Cancel();
        _cts = new CancellationTokenSource();

        await Generate(_cts.Token);
        await _stateMachine.SetState(new IdleState());
    }

    public async override UniTask Exit() { Debug.Log($"Exit from state: {this.GetType()}"); }

    public async override UniTask Update() { }

    public async override UniTask InputEvent(GameEvent gameEvent, params object[] data)
    {
        if (gameEvent == GameEvent.GettingReward)
        {
            Cancel();
            await _stateMachine.SetState(new GettingRewardState(data));
        }
    }

    //--------------------------------------------------------------

    private async UniTask Generate(CancellationToken cancellationToken)
    {
        ITokenViewSpawner tokenViewSpawner = campaignAgents.TokenViewSpawner;
        var allAgents = campaignAgents.Agents.All;
        CampaignAgent[] resultAgents = null;

        // When calling from CheckEnoughTokensAroundPlayer
        if (_isRequiredExtraTokens)
        {
            resultAgents = campaignAgents.Agents.Active.ToArray();
        }
        // When calling from UpdateRewardState
        else if (tokenViewSpawner.Collection.Count < 1)
        {
            var completedAgents = campaignAgents.Agents.Used.Where(agent => agent.IsCompleted).ToList();
            Shuffle(allAgents, completedAgents.Count > 0);
            var availableAgents = allAgents.Except(completedAgents).ToList();

            var brandCountRes = Math.Min(availableAgents.Count, Game.Config.Meta.MaxCampaigns);
            brandCountRes = Mathf.Abs(brandCountRes - campaignAgents.Agents.Active.Count);
            for (int i = 0; i < brandCountRes; i++)
            {
                var agent = availableAgents.ElementAt(i);
                campaignAgents.RegisterAgent(agent);
            }

            resultAgents = campaignAgents.Agents.Active.ToArray();
        }
        // When calling from GettingRewardState
        else
        {
            var unusedAgents = allAgents.Except(campaignAgents.Agents.Used).ToList();
            if (unusedAgents.Count > 0)
            {
                var randomIndex = UnityEngine.Random.Range(0, unusedAgents.Count);
                var agent = unusedAgents.ElementAtOrDefault(randomIndex);
                campaignAgents.RegisterAgent(agent);

                resultAgents = new[] { agent };
            }
        }

        if (resultAgents == null) { return; }

        await tokenViewSpawner.Spawn(count: RequiredPointsCount, isNeedExtraView: _isRequiredExtraTokens, cancellationToken: cancellationToken, agents: resultAgents);
    }

    private void Shuffle(List<CampaignAgent> agents, bool hasGotCompletedAgents)
    {
        var random = new System.Random();
        if (hasGotCompletedAgents)
        {
            agents.Shuffle(random);
        }
        else
        {
            var topCampaign = campaignAgents.Agents.All[0];
            agents.Remove(topCampaign);
            agents.Shuffle(random);
            agents.Insert(0, topCampaign);
        }
    }

    private void Cancel()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
    }

    ~GenerateRewardTokensState()
    {
        Cancel();
    }
}