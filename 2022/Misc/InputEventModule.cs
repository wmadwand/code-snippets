using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputEventModule : BattleModuleBase
{
    public event Action OnNotEnoughtManna;
    public event Action<BattleInputEvent> OnApplyEvent;

    private ExecutorsUpdaterModule ExecutorsUpdater;

    class AppliedEvent
    {
        public int Tick;
        public BattleInputEvent Event;
    }

    private List<AppliedEvent> EventsLog = new List<AppliedEvent>();

    public override void ApplyTick()
    {
    }

    public bool AddInputEvent(BattleInputEvent inputEvent)
    {
        var friendlyUnits = inputEvent.Card.Config.DeployView?.GetComponent<DeployFriendlyUnitsView>();
        if (friendlyUnits == null)
        {
            if (!BattleState.Field.LevelConfig.IsInsideFieldForSpell(inputEvent.Position))
            {
                return false;
            }
        }
        else
        {
            if (!BattleState.Field.LevelConfig.IsInsideField(inputEvent.Position))
            {
                return false;
            }
        }

        if (BattleState.Deck.UseCard(inputEvent.Card))
        {
            if (ExecutorsUpdater == null)
            {
                ExecutorsUpdater = BattleSimulator.GetModule<ExecutorsUpdaterModule>();
            }
            ExecutorsUpdater.Schedule(inputEvent.Card.Config.Activate(BattleSimulator, inputEvent));
            EventsLog.Add(new AppliedEvent { Tick = BattleState.Tick, Event = inputEvent });
            OnApplyEvent?.Invoke(inputEvent);
            return true;
        }
        else
        {
            OnNotEnoughtManna?.Invoke();
            Logger.LogError($"Fail add event {inputEvent}");
            return false;
        }

    }

    [DropdownName("Input Event")]
    public class Creator : CreatorBase
    {
        protected override BattleModuleBase DoCreate(BattleSimulator battleSimulator, BattleStartConfig startConfig)
        {
            return new InputEventModule();
        }
    }

}
