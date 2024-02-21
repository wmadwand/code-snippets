using Cysharp.Threading.Tasks;
using Project.Controller;
using Project.Data;
using Project.Utils.StateMachineMono;
using System.Linq;
using UnityEngine;

public class UpdateRewardState : GameState
{
    private readonly ITreasureLocalStorage _storage;
    private RewardStateMachine _campaignAgents;

    //--------------------------------------------------------------

    public UpdateRewardState(params object[] data) : base(data)
    {
        _storage = (ITreasureLocalStorage)data[1];
    }

    public async override UniTask Enter()
    {
        Debug.Log($"Enter to state: {this.GetType()}");

        _campaignAgents = (RewardStateMachine)_stateMachine;
        UpdateFromStorage(_storage);

        await _stateMachine.SetState(new GenerateRewardTokensState());
    }

    public async override UniTask Exit() { Debug.Log($"Exit from state: {this.GetType()}"); }

    public async override UniTask Update() { }

    public async override UniTask InputEvent(GameEvent gameEvent, params object[] data) { }

    //--------------------------------------------------------------    

    private bool UpdateFromStorage(ITreasureLocalStorage storage)
    {
        var data = storage.Load();
        if (data != null)
        {
            var query = from dataAgent in data.usedAgents
                        join masterAgent in _campaignAgents.Agents.All
                        on dataAgent.Campaign.id equals masterAgent.Campaign.id
                        select new { dataAgent, masterAgent };

            foreach (var couple in query)
            {
                couple.masterAgent.LoadState(couple.dataAgent);
                _campaignAgents.RegisterAgent(couple.masterAgent);
            }
        }

        return data != null;
    }
}