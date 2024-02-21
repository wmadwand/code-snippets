using Cysharp.Threading.Tasks;
using Project.Analytics;
using Project.Data;
using Project.Gameplay.Data.Campaign;
using Project.Utils.StateMachineMono;
using System;
using UnityEngine;

namespace Project.Controller
{
    public interface IRewardStateMachine
    {
        CampaignAgentCollection Agents { get; }
        UniTask Init(object rewards, ITreasureLocalStorage storage);
        UniTask GenerateMoreTokens(); //TODO: remove it from here
    }

    public class RewardStateMachine : MonoStateMachine, IRewardStateMachine
    {
        public CampaignAgentCollection Agents => _agents;
        public TokenViewSpawner TokenViewSpawner => _tokenViewSpawner;
        public CampaignAgentsAnalytics Analytics { get; private set; }

        [SerializeField] private TokenViewSpawner _tokenViewSpawner;
        [SerializeField] private CampaignAgentCollection _agents;

        //--------------------------------------------------------------

        public async UniTask Init(object rewards, ITreasureLocalStorage storage)
        {
            _agents = new CampaignAgentCollection();

            //TODO: State factory
            await SetState(new CreateRewardState(rewards, storage));
        }

        public void RegisterAgent(CampaignAgent agent)
        {
            Agents.Used.Add(agent);
            if (!agent.IsCompleted) { Agents.Active.Add(agent); }

            if (!agent.Tokens.GotCollected)
            {
                Analytics.StartCampaign(agent.Campaign);
            }
        }

        //--------------------------------------------------------------

        private void Start()
        {
            Analytics = new CampaignAgentsAnalytics();

            CampaignAgent.OnRewardRequest += CampaignAgent_OnRewardRequest;
            CheckEnoughTokensAroundPlayer.OnCheckEnoughTokensAroundPlayer += OnCheckEnoughTokensAroundPlayer;
        }

        public async UniTask GenerateMoreTokens()
        {
            var isRequiredExtraTokens = true;
            await InputEvent(GameEvent.GenerateTokens, isRequiredExtraTokens);
        }

        private void OnCheckEnoughTokensAroundPlayer()
        {
            UniTask.Void(async () =>
            {
                var isRequiredExtraTokens = true;
                await InputEvent(GameEvent.GenerateTokens, isRequiredExtraTokens);
            });
        }

        private void CampaignAgent_OnRewardRequest(CampaignAgent agent)
        {
            UniTask.Void(async () =>
            {
                await InputEvent(GameEvent.GettingReward, agent);
            });
        }

        private void OnDestroy()
        {
            CampaignAgent.OnRewardRequest -= CampaignAgent_OnRewardRequest;
            CheckEnoughTokensAroundPlayer.OnCheckEnoughTokensAroundPlayer -= OnCheckEnoughTokensAroundPlayer;
        }
    }
}