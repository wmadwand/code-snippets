using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace TutorialConditions
{
	public class HasActiveBattle : ITutorialCondition
	{
		public bool IsMet(TutorialContext context)
		{
			return BattleSimulator.Active != null;
		}
	}

	public class BattleIsPlaying : ITutorialCondition
	{
		public bool IsPlaying;

		public bool IsMet(TutorialContext context)
		{
			return BattleSimulator.Active != null && BattleSimulator.Active.IsPlaying == IsPlaying;
		}
	}

	public class BattleWave : ITutorialCondition
	{
		public int WaveIndex;

		public bool IsMet(TutorialContext context)
		{
			return context.BattleSimulator?.State.Wave == WaveIndex;
		}
	}

	public class HasCardInHand : ITutorialCondition
	{
		[SerializeField] CardConfig Card;

		public bool IsMet(TutorialContext context)
		{
			return context.BattleSimulator != null && context.BattleSimulator.State.Deck.CardsInHand.Any(c => c?.Config == Card);
		}
	}

	public class CardsCountInHand : ITutorialCondition
	{
		[SerializeField] Comparator<int> Count;

		public bool IsMet(TutorialContext context)
		{
			return context.BattleSimulator != null && Count.IsMet(context.BattleSimulator.State.Deck.CardsInHand.Count());
		}
	}

	public class ExecutorContextTrigger : ITutorialCondition
	{
		[SerializeField] Selectors.IUnitSelector Unit;
		[SerializeField] IExecutorContextTrigger Trigger;

		public bool IsMet(TutorialContext context)
		{
			var c = ExecutorContext.Create(context.BattleSimulator);
			return Trigger.CanTrigger(Unit?.Select(c).FirstOrDefault(), c);
		}
	}

	public class UnitCondiction : ITutorialCondition
	{
		[SerializeField] Selectors.IUnitSelector Unit;
		[SerializeField] IUnitCondition Condition;

		public bool IsMet(TutorialContext context)
		{
			var c = ExecutorContext.Create(context.BattleSimulator);
			return Condition.IsMet(Unit?.Select(c).FirstOrDefault(), c);
		}
	}
}
