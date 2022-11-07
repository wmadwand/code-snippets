using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleEndModule : BattleModuleBase
{
    public class UnitTarget
    {
        public UnitConfig Unit;
        public int RemainCount;
    }

    public List<UnitTarget> UnitTargets = new List<UnitTarget>();

    bool BattleEnded;
    HashSet<Unit> DeadUnits = new HashSet<Unit>();

    void UnitDead(Unit unit)
    {
        if (!DeadUnits.Add(unit))
        {
            Debug.LogError("Unit already dead");
            return;
        }

        if (BattleState.Deck.IsEnemyFraction(unit.Fraction))
        {
            float gold = unit.Stats[StatType.Gold];
            if (gold > 0)
            {
                var reward = BattleState.Field.LevelConfig.CollectedGold.TryGetOrCreate(unit.Config);
                reward.Unit = unit.Config;
                reward.Amount += gold;
                reward.Boss = unit.Has<UnitTags.Boss>();
                reward.Count++;
            }
        
        }
        foreach (var t in UnitTargets)
        {
            if (t.Unit == unit.Config)
            {
                t.RemainCount--;
                break;
            }
        }
    }

    public override void Activate()
    {
        BattleSimulator.OnDeathUnit += UnitDead;
        var skipUnits = new List<UnitConfig>();
        var takeOnlyUnits = new List<UnitConfig>();

        if (BattleState.Field.LevelConfig.EndConditions != null)
        {
            foreach (var end in BattleState.Field.LevelConfig.EndConditions)
            {
                if (end is BattleEndConditions.ConcreteUnitTypesAsTarget concrete)
                {
                    takeOnlyUnits.AddRange(concrete.Units);
                }
                else if (end is BattleEndConditions.SkipUnitTypesAsTarget skip)
                {
                    skipUnits.AddRange(skip.Units);
                }
                else if (end is BattleEndConditions.KillUnits kill)
                {
                    if (kill.Unit != null)
                    {
                        var target = UnitTargets.FirstOrDefault(t => t.Unit == kill.Unit);
                        if (target == null)
                        {
                            target = new UnitTarget { Unit = kill.Unit, RemainCount = kill.Count };
                            UnitTargets.Add(target);
                        }
                        target.RemainCount = Mathf.Min(kill.Count, target.RemainCount);
                    }
                }
            }
        }

        foreach(var u in BattleState.Field.LevelConfig.UnitSpawns)
        {
            if (u.Fraction == BattleState.Deck.Fraction) { continue; }
            if (skipUnits.Contains(u.Config)) { continue; }
            if (takeOnlyUnits.Count > 0 && !takeOnlyUnits.Contains(u.Config)) { continue; }
            var target = UnitTargets.FirstOrDefault(t => t.Unit == u.Config);
            if (target == null)
            {
                target = new UnitTarget { Unit = u.Config, RemainCount = u.Count };
                UnitTargets.Add(target);
            }
            target.RemainCount = Mathf.Min(u.Count, target.RemainCount);
        }
    }

    public override void ApplyTick()
    {
        if (BattleState.IsEnded) { return; }

        if (BattleState.Deck.MainHero != null && BattleState.Deck.MainHero.IsDead && !BattleState.Units.Contains(BattleState.Deck.MainHero))
        {
            if (!BattleEnded)
            {
                BattleEnded = true;
                //Wait 0.5 seconds for complete death routines, but less than 1 second because WaveModule wait 1 second when wave ends
                BattleSimulator.GetModule<ExecutorsUpdaterModule>().Schedule(WaitNextTick.Wait(0.5f, BattleState.SecondsInTick), () =>
                {
                    BattleSimulator.End(new BattleResult { Win = false });
                    BattleEnded = false;
                });
            }
            return;
        }

        if (UnitTargets.Count > 0)
        {
            var allCompleted = true;
            foreach(var t in UnitTargets)
            {
                if (t.RemainCount > 0)
                {
                    allCompleted = false;
                    break;
                }
            }
            if (allCompleted)
            {
                KillAllEnemies();
                BattleSimulator.End(new BattleResult { Win = true });
                return;
            }
        }

        var condictions = BattleState.Field.LevelConfig.EndConditions;
        if (condictions == null) { return; }

        foreach (var c in BattleState.Field.LevelConfig.EndConditions)
        {
            var result = c.IsEnd(BattleSimulator);
            if (result != null)
            {
                if (c.KillAllEnemiesWhenWaveEnd)
                {
                    KillAllEnemies();
                }
                BattleSimulator.End(result);

                return;
            }
        }

        return;

    }

    void KillAllEnemies()
    {
        foreach (var u in BattleState.Units)
        {
            if (!u.IsDead && BattleState.Deck.IsEnemyFraction(u.Fraction))
            {
                u.Stats.Health.Set(0, null, silent: true);
                u.Die();
            }
        }
    }

    [DropdownName("Battle End")]
    public class Creator : CreatorBase
    {
        protected override BattleModuleBase DoCreate(BattleSimulator battleSimulator, BattleStartConfig startConfig)
        {
            return new BattleEndModule();
        }
    }

}

public interface IBattleEndCondition
{
    BattleResult IsEnd(BattleSimulator battleSimulator);
    bool KillAllEnemiesWhenWaveEnd { get; }
    Sprite GoalIcon { get; }
    LocalizedString GoalText { get; }
    bool HideTargetsUI { get; }
}

namespace BattleEndConditions
{
    public abstract class BattleEndConditionBase : IBattleEndCondition
    {
        public Sprite GoalIcon;
        public LocalizedString GoalText;

        [SerializeField] protected bool KillEnemiesWhenWaveEnd;
        public bool KillAllEnemiesWhenWaveEnd { get { return KillEnemiesWhenWaveEnd; } }

        Sprite IBattleEndCondition.GoalIcon => GoalIcon;
        LocalizedString IBattleEndCondition.GoalText => GoalText;

        bool IBattleEndCondition.HideTargetsUI => HideTargetsUI;
        protected virtual bool HideTargetsUI => true;

        public abstract BattleResult IsEnd(BattleSimulator battleSimulator);
    }

    public abstract class BattleEndConditionFake : IBattleEndCondition
    {
        public bool KillAllEnemiesWhenWaveEnd => false;
        Sprite IBattleEndCondition.GoalIcon => null;
        LocalizedString IBattleEndCondition.GoalText => new LocalizedString();
        public abstract BattleResult IsEnd(BattleSimulator battleSimulator);

        bool IBattleEndCondition.HideTargetsUI => HideTargetsUI;
        protected virtual bool HideTargetsUI => true;

    }

    public class List : BattleEndConditionBase
    {
        [SerializeField] List<IBattleEndCondition> Condutions;
        [SerializeField] Type Match;
        enum Type
        {
            Any,
            AllLose,
            AllWin,
        }

        public override BattleResult IsEnd(BattleSimulator simulator)
        {
            if (Match == Type.Any)
            {
                foreach(var c in Condutions)
                {
                    var result = c.IsEnd(simulator);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }
            else
            {
                BattleResult lastResult = null;
                foreach (var c in Condutions)
                {
                    var result = c.IsEnd(simulator);
                    if (result == null)
                    {
                        return null;
                    }
                    if (lastResult == null)
                    {
                        lastResult = result;
                    }
                    else if (lastResult.Win != result.Win)
                    {
                        return null;
                    }
                }
                if ((Match == Type.AllLose && !lastResult.Win) || (Match == Type.AllWin && lastResult.Win))
                {
                    return lastResult;
                }
            }
            return null;
        }
        protected override bool HideTargetsUI => Condutions.Any(c => c.HideTargetsUI);
    }

    public class JustDescription : BattleEndConditionBase
    {
        public override BattleResult IsEnd(BattleSimulator simulator) => null;
    }

    public class ConcreteUnitTypesAsTarget : BattleEndConditionFake
    {
        public List<UnitConfig> Units = new List<UnitConfig>();
        public override BattleResult IsEnd(BattleSimulator simulator) => null;
        protected override bool HideTargetsUI => false;
    }

    public class KillUnits : BattleEndConditionFake
    {
        public UnitConfig Unit;
        public int Count;
        public override BattleResult IsEnd(BattleSimulator simulator) => null;
    }

    public class SkipUnitTypesAsTarget : BattleEndConditionFake
    {
        public List<UnitConfig> Units = new List<UnitConfig>();
        public override BattleResult IsEnd(BattleSimulator simulator) => null;
        protected override bool HideTargetsUI => false;
    }

    public class EndByTime : BattleEndConditionBase
    {
        [SerializeField] float Time;
        [SerializeField] bool Win;

        public float GetRemainTime(BattleSimulator simulator)
        {
            return Time - simulator.State.Time;
        }

        public override BattleResult IsEnd(BattleSimulator simulator)
        {
            var time = simulator.State.Time;
            if (Time <= time)
            {
                return new BattleResult
                {
                    Win = Win,
                    ByCondition = this
                };
            }
            return null;
        }
    }

    public class UnitNotFound : BattleEndConditionBase
    {
        public UnitConfig UnitConfig => Unit;

        [SerializeField] UnitConfig Unit;
        [SerializeField] bool Win;
        [SerializeField] float DelayFromStart = 1;
        [SerializeField] SpawnPoint FromSpawnPoint;

        public override BattleResult IsEnd(BattleSimulator simulator)
        {
            if (simulator.State.TimeSinceLastWaveStarted < DelayFromStart) { return null; }

            if (FromSpawnPoint && !simulator.State.Units.Any(u => u.Config == Unit && u.Get<SpawnPoint>() == FromSpawnPoint))
            {
                return new BattleResult
                {
                    Win = Win,
                    ByCondition = this
                };
            }
            else if (!simulator.State.Units.Any(u => u.Config == Unit))
            {
                return new BattleResult
                {
                    Win = Win,
                    ByCondition = this
                };
            }
            return null;
        }
    }

    public class UnitReachPoint : BattleEndConditionBase
    {
        [SerializeField] Transform Point;
        [SerializeField] float Distance = 0.5f;
        [SerializeField] UnitConfig Unit;
        [SerializeField] bool Win;
        [SerializeField] SpawnPoint FromSpawnPoint;

        [System.NonSerialized]
        BattleResult CachedResult;

        public override BattleResult IsEnd(BattleSimulator simulator)
        {
            if (CachedResult == null)
            {
                foreach (var unit in simulator.State.Units.Where(u => u.Config == Unit))
                {
                    if ((Point.position.ConvertWorldPositionToField() - unit.Position).sqrMagnitude < Distance * Distance &&
                        (!FromSpawnPoint || FromSpawnPoint == unit.Get<SpawnPoint>()))
                    {
                        CachedResult = new BattleResult
                        {
                            Win = Win,
                            ByCondition = this
                        };
                        break;
                    }
                }
            }
            return CachedResult;
        }
    }
}
