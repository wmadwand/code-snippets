using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static UnitConfig;
using Sirenix.OdinInspector;
using UnitActions;

namespace Selectors
{

    public interface IUnitSelector : IBattleObjectSelector<Unit>
    {
        IEnumerable<Unit> Select(IExecutorContext context);
    }

    namespace Units
    {

        public abstract class UnitSelectorBase : IUnitSelector
        {
            public abstract IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets);

            IEnumerable<Unit> IUnitSelector.Select(IExecutorContext context)
            {
                return Select(context, context?.Targets);
            }
        }

        public class Targets : UnitSelectorBase
        {
            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                return targets;
            }
        }

        public class Owner : UnitSelectorBase
        {
            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                yield return context.Owner;
            }
        }

        public class NotOwner : UnitSelectorBase
        {
            public static readonly NotOwner Instance = new NotOwner();

            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                foreach (var c in targets)
                {
                    if (c != context.Owner)
                    {
                        yield return c;
                    }
                }
            }
        }

        public class NotInContextTargets : UnitSelectorBase
        {
            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                foreach (var c in targets)
                {
                    if (!context.Targets.Any(t => t == c))
                    {
                        yield return c;
                    }
                }
            }
        }

        public class All : UnitSelectorBase
        {
            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                return context.Battle.Units;
            }
        }

        public class Hero : UnitSelectorBase
        {
            public Fraction Fraction;

            public Unit GetHero(BattleSimulator battleSimulator)
            {
                return battleSimulator.State.Units.Where(u => u.Has<UnitTags.Hero>() && u.Fraction == Fraction).FirstOrDefault();
            }

            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                foreach (var t in targets)
                {
                    if (t.Has<UnitTags.Hero>() && t.Fraction == Fraction)
                    {
                        yield return t;
                    }
                }
            }
        }

        public class InRange : UnitSelectorBase
        {
            public IPositionSelector Position;
            public IValueGetter Range;

            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                var position = Position.GetPosition(context);
                var sqrDist = Range.Get(context);
                sqrDist *= sqrDist;
                var units = targets;
                foreach (var u in targets)
                {
                    var sqr = (u.Position - position).sqrMagnitude;
                    if (sqr < sqrDist)
                    {
                        yield return u;
                    }
                }
            }
        }

        public class InLine : UnitSelectorBase
        {
            public IPositionSelector Origin;
            public IPositionSelector Destination;
            public IValueGetter Length;
            public IValueGetter Distance;

            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                var origin = Origin.GetPosition(context);
                var destination = Destination.GetPosition(context);
                var distance = Distance.Get(context);
                var length = Length.Get(context);

                var direction = destination - origin;
                direction.Normalize();
                destination = origin + length * direction;

                var sqrDist = distance * distance;

                foreach (var u in targets)
                {
                    var sqrOrigin = (u.Position - origin).sqrMagnitude;
                    var sqrDestination = (u.Position - destination).sqrMagnitude;

                    if (sqrOrigin < sqrDist || sqrDestination < sqrDist)
                    {
                        yield return u;
                    }

                    var projectedPoint = origin + Vector2.Dot(u.Position - origin, direction) * direction;
                    var onSegment = Vector2.Dot(projectedPoint - origin, projectedPoint - destination) < 0f;
                    var sqrLine = (u.Position - projectedPoint).sqrMagnitude;

                    if (onSegment && sqrLine < sqrDist)
                    {
                        yield return u;
                    }
                }
            }
        }


        public class NearestInRange : UnitSelectorBase
        {
            public IPositionSelector Position;
            public IValueGetter Range;

            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                var position = Position.GetPosition(context);

                var sqrDist = Range.Get(context);
                sqrDist *= sqrDist;
                Unit unit = null;

                foreach (var u in targets)
                {
                    if (u.BehaviourType == UnitBehaviourTypes.Neutral)
                        continue;

                    var sqr = (u.Position - position).sqrMagnitude;
                    if (sqr < sqrDist)
                    {
                        sqrDist = sqr;
                        unit = u;
                    }
                }
                if (unit != null)
                {
                    yield return unit;
                }
            }
        }

        public class NearestEnemy : UnitSelectorBase
        {
            public IPositionSelector Position;
            public IValueGetter Range;

            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                var position = Position.GetPosition(context);                
                var zone = context.Owner.Stats.Zone;                
                Unit resultTarget = null;

                float visibiltyRangeSqr = context.Owner.Stats.VisibilityRangeSqr;
                var priority = 0f;
                var minSqr = visibiltyRangeSqr;

                foreach (var u in targets)
                {
                    if (u.BehaviourType == UnitBehaviourTypes.Neutral) { continue; }
                    if (!context.Owner.IsEnemy(u)) { continue; }

                    //TODO: if (Filter != null && !Filter.IsMet(unit, null)) { continue; }

                    var sqr = (u.Position - position).sqrMagnitude;
                    if (sqr > visibiltyRangeSqr) { continue; }

                    if (u.Stats.DetectRangeSqr > 0 && sqr > u.Stats.DetectRangeSqr)
                    {
                        continue;
                    }

                    if (u.BehaviourType == UnitBehaviourTypes.Bait)
                    {
                        resultTarget = u;
                        break;
                    }

                    if (u.Stats.Zone != zone && sqr > context.Owner.Stats.AttackRangeSqr)
                    {
                        continue;
                    }

                    if (sqr < minSqr && u.Stats.TargetAttackPriority >= priority)
                    {
                        if (u.Has<UnitTags.Hidden>()) { continue; }

                        priority = u.Stats.TargetAttackPriority;
                        minSqr = sqr;
                        resultTarget = u;
                    }
                }

                if (resultTarget != null)
                {
                    yield return resultTarget;
                }
            }
        }

        public class IsDetectable : UnitSelectorBase
        {
            public IPositionSelector Position;

            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                var position = Position.GetPosition(context);
                foreach (var target in targets)
                {
                    if (target.Has<UnitTags.Hidden>()) { continue; }
                    var range = target.Stats[StatType.DetectRange];
                    if (range == null)
                    {
                        yield return target;
                    }
                    var distanceSqr = (position - target.Position).sqrMagnitude;
                    if (range > 0f && distanceSqr < range * range)
                    {
                        yield return target;
                    }
                }
            }
        }

        public class NearestToPosition : UnitSelectorBase
        {
            [SerializeField] IPositionSelector Position;
            [SerializeField] bool RestrictCount;
            [SerializeField, Range(1, 100), ShowIf("RestrictCount")] int NearestUnitsCount = 1;

            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                var position = Position.GetPosition(context);
                var units = targets;
                units = units.OrderBy(u => (u.Position - position).sqrMagnitude);
                int index = NearestUnitsCount;
                foreach (var unit in units)
                {
                    yield return unit;
                    if (RestrictCount)
                    {
                        index--;
                        if (index <= 0) yield break;
                    }
                }
            }
        }

        public class NestedFromAll : Selectors.Nested<Unit, IUnitSelector>, IUnitSelector
        {
            public NestedFromAll()
            {
            }

            public NestedFromAll(List<IUnitSelector> selectors)
            {
                Selectors = selectors.ConvertAll<IBattleObjectSelector<Unit>>(s => s);
            }

            public IEnumerable<Unit> Select(IExecutorContext context)
            {
                return Select(context, context.Battle.Units);
            }
        }

        public class Or : Selectors.Or<Unit, IUnitSelector>, IUnitSelector
        {
            public IEnumerable<Unit> Select(IExecutorContext context)
            {
                return Select(context, context.Targets);
            }
        }

        public class OrFromAll : Selectors.Or<Unit, IUnitSelector>, IUnitSelector
        {
            public IEnumerable<Unit> Select(IExecutorContext context)
            {
                return Select(context, context.Battle.Units);
            }
        }

        public class And : Selectors.And<Unit, IUnitSelector>, IUnitSelector
        {
            public IEnumerable<Unit> Select(IExecutorContext context)
            {
                return Select(context, context.Targets);
            }
        }

        public class AndFromAll : Selectors.And<Unit, IUnitSelector>, IUnitSelector
        {
            public IEnumerable<Unit> Select(IExecutorContext context)
            {
                return Select(context, context.Battle.Units);
            }
        }

        public class WithFraction : UnitSelectorBase
        {
            public List<Fraction> Fractions;
            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                foreach (var c in targets)
                {
                    if (Fractions != null && Fractions.Contains(c.Fraction))
                        yield return c;
                }
            }
        }

        public class MetCondition : UnitSelectorBase
        {
            public IUnitCondition Condition;

            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                foreach (var c in targets)
                {
                    if (Condition.IsMet(c, context))
                    {
                        yield return c;
                    }
                }
            }
        }

        public class EnemiesForOwner : UnitSelectorBase
        {
            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                foreach (var c in targets)
                {
                    if (context.Owner.IsEnemy(c))
                    {
                        yield return c;
                    }
                }
            }
        }

        public class InSameZoneWithPoint : UnitSelectorBase
        {
            public IPositionSelector Position;

            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                var point = Position.GetPosition(context);
                foreach (var c in targets)
                {
                    var map = c.NavigationMap;
                    if (map.GetCell(c.Position).Zone == map.GetCell(point).Zone)
                    {
                        yield return c;
                    }
                }
            }
        }

        public class EnemiesForCaster : UnitSelectorBase
        {
            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                if (context.Caster == null) { yield break; }

                foreach (var c in targets)
                {
                    if (context.Caster.IsEnemy(c))
                    {
                        yield return c;
                    }
                }
            }
        }

        public class AlliesForOwner : UnitSelectorBase
        {
            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                foreach (var c in targets)
                {
                    if (context.Owner.IsAlly(c, sameIsAlly: false))
                    {
                        yield return c;
                    }
                }
            }
        }

        public class ByConfig : UnitSelectorBase
        {
            [SerializeField] UnitConfig Unit;

            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                foreach (var c in targets)
                {
                    if (c.Config == Unit)
                    {
                        yield return c;
                    }
                }
            }
        }

        public class Random : UnitSelectorBase
        {
            [SerializeField] int Amount = 1;

            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                return targets.ToArray().GetRandomList(Amount, context.Battle.Random);
            }
        }

        public class AliveHeroFromDeck : UnitSelectorBase
        {
            [SerializeField] UnitConfig Hero;

            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                var unit = context.Battle.Deck.Heroes.FirstOrDefault(h => h.Config == Hero);
                if (unit == null || unit.IsDead)
                {
                    unit = context.Battle.Deck.Heroes.FirstOrDefault(h => !h.IsDead);
                }

                if (unit == null)
                {
                    unit = context.Battle.Deck.Heroes.FirstOrDefault();
                }

                yield return unit;
            }
        }


        //Library of various selector
        //TODO refactoring FullInspector to Odin
        /*
        [FullInspector.InspectorDropdownName("Tutorial/By Name")]
        public class ByName : UnitSelectorBase
        {
            [SerializeField] string Name;
            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                return context.Session.Battle.Units.Where(t => t.Name == Name);
            }
        }

        [FullInspector.InspectorDropdownName("Complex/By condition")]
        public class ByCondition : UnitSelectorBase
        {
            [SerializeField] ITriggerCondition[] Conditions;
            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                foreach (var t in targets)
                {
                    if (Conditions.All(r => r.CanTrigger(t)))
                    {
                        yield return t;
                    }
                }
            }
        }

        [FullInspector.InspectorDropdownName("Vision/Stels")]
        public class InStelsMode : UnitSelectorBase
        {
            [SerializeField] IUnitSelector ForStelsMode;
            [SerializeField] IUnitSelector ForRevealMode;
            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                if (context.Battle.InStelsMode)
                {
                    return ForStelsMode.Select(context, targets);
                }
                return ForRevealMode.Select(context, targets);
            }
        }

        [FullInspector.InspectorDropdownName("")]
        public class FromDelegate : UnitSelectorBase
        {
            public FromDelegate(System.Func<Unit, bool> canSelect)
            {
                CanSelect = canSelect;
            }
            System.Func<Unit, bool> CanSelect;

            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                return targets.Where(c => CanSelect.SafeInvoke(c));
            }
        }


        [FullInspector.InspectorDropdownName("Logic/None")]
        public class None : UnitSelectorBase
        {
            public static readonly None Instance = new None();

            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                yield break;
            }
        }

        [FullInspector.InspectorDropdownName("Logic/If")]
        public class If : UnitSelectorBase
        {
            [SerializeField] IExecutorTriggerCondition Condition;
            [SerializeField] IUnitSelector True;
            [SerializeField] IUnitSelector False;

            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                if (Condition.CanTrigger(context))
                {
                    return True.Select(context, targets);
                }
                else
                {
                    return False.Select(context, targets);
                }
            }
        }

        [FullInspector.InspectorDropdownName("Logic/True")]
        public class True : UnitSelectorBase
        {
            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                return targets;
            }
        }

        [FullInspector.InspectorDropdownName("Context/In target point (from alive)")]
        public class BySendPosition : UnitSelectorBase
        {

            public static readonly BySendPosition Instance = new BySendPosition();

            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                foreach (var c in context.Battle.Units)
                {
                    if (c.Position.IsSameCell(context.TargetPoint.Position))
                    {
                        yield return c;
                    }
                }
            }
        }

        [FullInspector.InspectorDropdownName("Context/In target point (from targets)")]
        public class BySendPositionTargetsFromContext : UnitSelectorBase
        {

            public static readonly BySendPositionTargetsFromContext Instance = new BySendPositionTargetsFromContext();

            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                foreach (var c in targets)
                {
                    if (c.Position.IsSameCell(context.TargetPoint.Position))
                    {
                        yield return c;
                    }
                }
            }
        }

        [FullInspector.InspectorDropdownName("Positions/From single")]
        public class FromPosition : UnitSelectorBase
        {
            [SerializeField] IPositionSelector Position;

            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                var p = Position.GetPosition(context);
                foreach (var c in targets)
                {
                    if (c.Position.IsSameCell(p))
                    {
                        yield return c;
                    }
                }
            }
        }

        [InspectorDropdownName("Positions/From multi")]
        public class FromPositions : UnitSelectorBase
        {
            [SerializeField] IMultiplePositionSelector Position;

            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                var positions = Position.GetPositions(context);
                foreach (var c in targets)
                {
                    if (positions.Any(p => c.Position.IsSameCell(p)))
                    {
                        yield return c;
                    }
                }
            }
        }

        [FullInspector.InspectorDropdownName("All/Units/Alive")]
        public class AllUnits : UnitSelectorBase
        {
            public static readonly AllUnits Instance = new AllUnits();

            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                return context.Battle.Units;
            }
        }

        [InspectorDropdownName("All/Units/Alive + Dead")]
        public class AllDeadAndAliveUnits : UnitSelectorBase
        {

            public IEnumerable<Unit> Select(IExecutorContext context)
            {
                return Select(context, context.Targets);
            }

            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                return context.Battle.Units.Concat(context.Battle.DeadUnits);
            }

            public override string ToString()
            {
                return string.Format("All dead and alive units");
            }
        }

        [FullInspector.InspectorDropdownName("WorldObjects/WorldObjectUnits By Description")]
        public class WorldObjectUnitsByDescription : UnitSelectorBase
        {
            [SerializeField] WorldObjectUnitBridgeDescription WorldObjectUnitBridgeDescription;
            [SerializeField] bool ConcatDeadUnits;

            public static readonly WorldObjectUnitsByDescription Instance = new WorldObjectUnitsByDescription();

            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                var units = context.Battle.Units;

                if (ConcatDeadUnits)
                {
                    units = units.Concat(context.Battle.DeadUnits);
                }

                foreach (var unit in units.Where(u => u is WorldObjectUnitBridgeEntity))
                {
                    if ((unit as WorldObjectUnitBridgeEntity).Description == WorldObjectUnitBridgeDescription)
                        yield return unit;
                }
            }
        }

        [FullInspector.InspectorDropdownName("WorldObjects/Current worldObject")]
        public class CurrentWorldObject : UnitSelectorBase
        {

            public static readonly CurrentWorldObject Instance = new CurrentWorldObject();

            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                var woContext = context as WorldObjectEffectorContext;
                if (woContext == null)
                {
                    Log.e("[WorldObjects/Current worldObject] context is not WorldObjectEffectorContext");
                    yield break;
                }

                foreach (var unit in context.Battle.Units)
                {
                    var woUnit = unit as WorldObjectUnitBridgeEntity;
                    if (woUnit != null && woContext.WorldObject == woUnit.WorldObject)
                    {
                        yield return unit;
                        break;
                    }
                }

            }
        }

        [FullInspector.InspectorDropdownName("All/Units/Alive + Hidden")]
        public class AliveOrHiddenUnits : UnitSelectorBase
        {
            public static readonly AliveOrHiddenUnits Instance = new AliveOrHiddenUnits();

            public IEnumerable<Unit> Select(IExecutorContext context) { return Select(context, context.Targets); }

            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                return context.Battle.Units.Concat(((DoNotUseDirectly.IHiddenUnitsGetter)context.Battle).HiddenUnits);
            }
        }

        [FullInspector.InspectorDropdownName("All/Units/Hidden")]
        public class HiddenUnits : UnitSelectorBase
        {
            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                return ((DoNotUseDirectly.IHiddenUnitsGetter)context.Battle).HiddenUnits;
            }
        }

        [FullInspector.InspectorDropdownName("All/Characters/Alive")]
        public class All : UnitSelectorBase
        {
            public static readonly All Instance = new All();

            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                return context.Battle.Units.Where(u => u is CharacterEntity);
            }
        }

        [FullInspector.InspectorDropdownName("All/Characters/Dead")]
        public class AllDead : UnitSelectorBase
        {
            public static readonly AllDead Instance = new AllDead();

            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                return context.Battle.DeadUnits.Where(u => u is CharacterEntity);
            }

            public override string ToString()
            {
                return string.Format("All Dead Characters");
            }
        }

        [FullInspector.InspectorDropdownName("All/Characters/Alive + Dead")]
        public class AllDeadAndAlive : UnitSelectorBase
        {
            public static readonly AllDeadAndAlive Instance = new AllDeadAndAlive();

            public IEnumerable<Unit> Select(IExecutorContext context) { return Select(context, context.Targets); }

            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                return All.Instance.Select(context, targets).Concat(AllDead.Instance.Select(context, targets));
            }

            public override string ToString()
            {
                return string.Format("All Dead and Alive Characters");
            }
        }


        [FullInspector.InspectorDropdownName("Complex/Nested (Targets from contexts!)")]
        public class Nested : CommonSelectors.Nested<Unit, IUnitSelector>, IUnitSelector
        {
            protected Nested() { }
            public Nested(List<IUnitSelector> selectors)
            {
                Selectors = selectors.ConvertAll<IBattleObjectSelector<Unit>>(s => s);
            }

            public IEnumerable<Unit> Select(IExecutorContext context)
            {
                return Select(context, context.Targets);
            }
        }

        [InspectorDropdownName("Complex/And (Dead Units)")]
        public class AndFromAllDead : CommonSelectors.And<Unit, IUnitSelector>, IUnitSelector
        {
            public IEnumerable<Unit> Select(IExecutorContext context)
            {
                return Select(context, context.Battle.DeadUnits);
            }
        }

        [FullInspector.InspectorDropdownName("Complex/And (All Alive + Dead Units)")]
        public class AndFromAllAlliveAndDead : CommonSelectors.And<Unit, IUnitSelector>, IUnitSelector
        {
            public IEnumerable<Unit> Select(IExecutorContext context)
            {
                var units = context.Battle.Units.Concat(context.Battle.DeadUnits);
                return Select(context, units);
            }
        }

        [FullInspector.InspectorDropdownName("Complex/Or (all alive + dead units)")]
        public class OrFromAllAndDead : CommonSelectors.Or<Unit, IUnitSelector>, IUnitSelector
        {
            public IEnumerable<Unit> Select(IExecutorContext context)
            {
                var units = context.Battle.Units.Concat(context.Battle.DeadUnits);
                return Select(context, units);
            }
        }

        [FullInspector.InspectorDropdownName("Complex/Or (All Alive Units)")]
        public class OrFromAll : CommonSelectors.Or<Unit, IUnitSelector>, IUnitSelector
        {
            public IEnumerable<Unit> Select(IExecutorContext context)
            {
                return Select(context, context.Battle.Units);
            }
        }

        [InspectorDropdownName("Complex/Or (all dead units)")]
        public class OrFromAllDead : CommonSelectors.Or<Unit, IUnitSelector>, IUnitSelector
        {
            public IEnumerable<Unit> Select(IExecutorContext context)
            {
                return Select(context, context.Battle.DeadUnits);
            }
        }

        [FullInspector.InspectorDropdownName("Complex/Random by fraction")]
        public class RandomByFraction : UnitSelectorBase
        {
            [SerializeField] List<KeyValuePair<CharacterFraction, float>> FractionProbability;

            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                var rnd = new System.Random(context.Session.Seed);
                foreach (var kv in FractionProbability)
                {
                    if (rnd.NextFloat() < kv.Value)
                    {
                        var t = targets.Where(u => u.OriginFraction == kv.Key).RandomChoice(rnd);
                        yield return t;
                        yield break;
                    }
                }
                yield return targets.RandomChoice(rnd);
            }
        }

        [FullInspector.InspectorDropdownName("Relations/Fraction/Origin/Is")]
        public class OriginFraction : UnitSelectorBase
        {
            [SerializeField] CharacterFraction Origin;
            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                foreach (var c in targets)
                {
                    if (c.OriginFraction == Origin)
                    {
                        yield return c;
                    }
                }
            }

            public override string ToString()
            {
                return string.Format("Origin Fraction = {0}", Origin.Name);
            }
        }

        [FullInspector.InspectorDropdownName("Relations/Fraction/Origin/Look like monster")]
        public class LookLikeMonster : UnitSelectorBase
        {
            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                foreach (var c in targets)
                {
                    if (c.LookLikeMonsterOrigin())
                    {
                        yield return c;
                    }
                }
            }

            public override string ToString()
            {
                return string.Format("Look like Monster(Origin)");
            }
        }

        [FullInspector.InspectorDropdownName("Relations/Fraction/Origin/Not")]
        public class NotOriginFraction : UnitSelectorBase
        {
            [SerializeField] CharacterFraction Origin;
            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                foreach (var c in targets)
                {
                    if (c.OriginFraction != Origin)
                    {
                        yield return c;
                    }
                }
            }
        }

        [FullInspector.InspectorDropdownName("Relations/Fraction/Current/Not")]
        public class NotCurrentFraction : UnitSelectorBase
        {
            [SerializeField] CharacterFraction Fraction;
            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                foreach (var c in targets)
                {
                    if (c.CurrentFraction != Fraction)
                    {
                        yield return c;
                    }
                }
            }
        }


        [FullInspector.InspectorDropdownName("Relations/Ally for owner")]
        public class AllyForOwner : UnitSelectorBase
        {
            [SerializeField] bool UseOriginFraction;
            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                foreach (var c in targets)
                {
                    if (context.Battle.Level.FractionRelashionship.IsAlly(context.Owner, c, UseOriginFraction))
                    {
                        yield return c;
                    }
                }
            }
        }

        [FullInspector.InspectorDropdownName("Relations/Fraction/Current/Not owner")]
        public class NotMyFraction : UnitSelectorBase
        {
            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                foreach (var c in targets)
                {
                    if (c.CurrentFraction != context.Owner.CurrentFraction)
                    {
                        yield return c;
                    }
                }
            }
        }

        [FullInspector.InspectorDropdownName("Relations/Fraction/Origin/Not owner")]
        public class NotMyFractionOrigin : UnitSelectorBase
        {
            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                foreach (var c in targets)
                {
                    if (c.OriginFraction != context.Owner.OriginFraction)
                    {
                        yield return c;
                    }
                }
            }
        }

        [FullInspector.InspectorDropdownName("Relations/Enemies for owner")]
        public class Enemies : UnitSelectorBase
        {
            [SerializeField] bool UseOriginFraction;
            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                foreach (var c in targets)
                {
                    if (context.Battle.Level.FractionRelashionship.IsEnemy(context.Owner, c, UseOriginFraction))
                    {
                        yield return c;
                    }
                }
            }
        }

        [FullInspector.InspectorDropdownName("Weapon/From weapon")]
        public class FromWeapon : UnitSelectorBase
        {
            public static readonly FromWeapon Instance = new FromWeapon();

            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                var fromWeaponSelector = context.ActiveWeapon.ActivationMode as IFromWeaponSelector;
                if (fromWeaponSelector == null)
                {
                    Log.e("Active weapon {0} has invalid activation mode. Should implement interface IFromWeaponSelector", context.ActiveWeapon);
                    yield break;
                }
                var selector = fromWeaponSelector.GetSelector();
                if (selector == null)
                {
                    Log.e("Active weapon {0} not contains target selector", context.ActiveWeapon);
                    yield break;
                }
                foreach (var c in selector.Select(context, targets))
                {
                    yield return c;
                }
            }
        }

        [FullInspector.InspectorDropdownName("Weapon/Possible Targets")]
        public class PossibleTargetsFromWeapon : UnitSelectorBase
        {
            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                context.Targets = targets;
                return context.ActiveWeapon.PossibleTargets(context);
            }
        }

        [FullInspector.InspectorDropdownName("Weapon/Is target")]
        public class IsWeaponTarget : UnitSelectorBase
        {
            [SerializeField] IUnitSelector WeaponOwners;

            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                var weaponOwners = WeaponOwners.Select(context);
                foreach (var c in targets)
                {
                    if (context.Session.ActionPlayer.AnyWeaponTarget(c, weaponOwners))
                    {
                        yield return c;
                    }
                }
            }
        }

        [FullInspector.InspectorDropdownName("Weapon/Targets by character (Work only in Play phase!)")]
        public class SelectedByCharacter : UnitSelectorBase
        {
            [SerializeField] IUnitSelector Character;

            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                var characters = Character.Select(context).ToList();
                var usedInTurnWeapons = context.Session.ActionPlayer.UsedInTurnWeapons;

                foreach (var character in characters)
                {
                    var weapon = usedInTurnWeapons.FirstOrDefault(w => w.Owner == character);
                    if (weapon != null)
                    {
                        foreach (var target in weapon.Targets)
                        {
                            yield return target;
                        }
                    }
                }
            }
        }

        [FullInspector.InspectorDropdownName("Vision/Visible units on start turn (determenistic)")]
        public class VisibleUnitsForOwnerOnStartTurn : IUnitSelector
        {

            [SerializeField] IUnitSelector Selector = new Owner();

            public IEnumerable<Unit> Select(IExecutorContext context)
            {
                return Select(context, context.Targets);
            }

            public IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                var characters = Selector.Select(context);
                var visible = new HashSet<Unit>();
                foreach (var character in characters)
                {
                    var visibleUnitsInfo = context.Session.Battle.CurrentTurnState.VisibleUnitsOnBeginTurnDetermenistic.TryGetOrDefault(character);
                    if (visibleUnitsInfo == null)
                    {
                        Log.e("No info about Visible units on begin turn for unit = {0}", characters);
                        continue;
                    }
                    foreach (var t in targets)
                    {
                        if (visibleUnitsInfo.Units.Contains(t))
                        {
                            visible.Add(t);
                        }
                    }
                }

                return visible;
            }
        }

        [InspectorDropdownName("Vision/Fog of war is visible (determenistic)")]
        public class UnitsFogIsVisible : IUnitSelector
        {
            [SerializeField] bool IsVisible;

            public IEnumerable<Unit> Select(IExecutorContext context)
            {
                return Select(context, context.Targets);
            }

            public IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                foreach (var t in targets)
                {
                    if (t.View != null && t.View.FogRevealer && t.View.FogRevealer.IsVisible == IsVisible)
                    {
                        yield return t;
                    }
                }
            }
        }

        [FullInspector.InspectorDropdownName("Distance/Line")]
        public class Line : CommonSelectors.Line<Unit>, IUnitSelector
        {
            public bool AllowOwnerAsTarget;

            public IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                if (AllowOwnerAsTarget)
                {
                    var endPoint = EndPositionSelector.GetPosition(context);
                    if (endPoint.IsSameCell(context.Owner.Position) && targets.Contains(context.Owner))
                    {
                        yield return context.Owner;
                    }
                }
                foreach (var u in LineSelect(context, targets))
                {
                    yield return u;
                }
            }

            public IEnumerable<Unit> Select(IExecutorContext context) { return Select(context, context.Targets); }
        }

        [FullInspector.InspectorDropdownName("Distance/In range")]
        public class InRange : CommonSelectors.InRange<Unit>, IUnitSelector
        {
            public IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                return RangeSelect(context, targets);
            }

            public IEnumerable<Unit> Select(IExecutorContext context) { return Select(context, context.Targets); }
        }

        [FullInspector.InspectorDropdownName("AI/Same Group")]
        public class SameGroup : IUnitSelector
        {
            public IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                var battle = context.Session.Battle;
                var aisOwner = battle.UnitAIs.Where(ai => ai.Unit == context.Owner);
                foreach (var character in targets)
                {
                    if (character == context.Owner)
                        continue;
                    var aisChar = battle.UnitAIs.Where(ai => ai.Unit == character);
                    bool isTheSame = false;
                    foreach (var ai in aisOwner)
                    {
                        if (aisChar.Any(c => c.GroupId.Equals(ai.GroupId)))
                        {
                            isTheSame = true;
                            break;
                        }
                    }
                    if (isTheSame)
                        yield return character;
                }
            }

            public IEnumerable<Unit> Select(IExecutorContext context) { return Select(context, context.Targets); }
        }

        [FullInspector.InspectorDropdownName("AI/All Group Members")]
        public class AllGroupMembers : IUnitSelector
        {
            public IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                var battle = context.Session.Battle;


                HashSet<string> groups = new HashSet<string>();
                foreach (var unit in targets)
                {
                    var aiUnit = battle.UnitAIs.Where(ai => ai.Unit == unit);
                    foreach (var u in aiUnit)
                    {
                        groups.Add(u.GroupId);
                    }
                }

                List<Unit> result = new List<Unit>();
                foreach (var character in battle.UnitAIs)
                {
                    if (groups.Contains(character.GroupId) && character.Unit != null)
                    {
                        result.Add(character.Unit);
                    }
                }

                foreach (var character in result)
                {
                    yield return character;
                }
            }

            public IEnumerable<Unit> Select(IExecutorContext context) { return Select(context, context.Targets); }
        }

        [FullInspector.InspectorDropdownName("AI/In group")]
        public class InGroup : IUnitSelector
        {
            [SerializeField] AiGroupTag AiGroup;

            public IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                var battle = context.Session.Battle;
                foreach (var character in targets)
                {
                    var aisChar = battle.UnitAIs.Where(ai => ai.Unit == character);
                    if (aisChar.Any(c => c.GroupId.Equals(AiGroup)))
                    {
                        yield return character;
                        continue;
                    }
                }
            }

            public IEnumerable<Unit> Select(IExecutorContext context) { return Select(context, context.Targets); }
        }

        [InspectorDropdownName("Context/Owner")]
        public class Owner : UnitSelectorBase
        {
            public static readonly Owner Instance = new Owner();

            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                yield return context.Owner;
            }
        }


        [InspectorDropdownName("Context/Is owner")]
        public class IsOwner : UnitSelectorBase
        {
            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                foreach (var c in targets)
                {
                    if (c == context.Owner)
                    {
                        yield return c;
                    }
                }
            }
        }

        [FullInspector.InspectorDropdownName("Units/Units belong character")]
        public class UnitsMemoryBelongCharacter : UnitSelectorBase
        {
            [SerializeField] IUnitSelector Character = new Owner();
            [SerializeField] IUnitSelector Units = new AllUnits();

            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                var characters = Character.Select(context);
                var units = Units.Select(context);

                foreach (var c in characters)
                {
                    foreach (var u in units)
                    {
                        var unit = u as UnitMemoryEntity;
                        if (unit != null && unit.OriginContext.Owner == c)
                        {
                            yield return unit;
                        }
                    }
                }
            }
        }

        [FullInspector.InspectorDropdownName("Units/Units By Description")]
        public class UnitsByDescription : UnitSelectorBase
        {
            [SerializeField] UnitMemoryDescription UnitDescription;

            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                foreach (var unit in targets.Where(u => u is UnitMemoryEntity))
                {
                    if ((unit as UnitMemoryEntity).Description == UnitDescription)
                        yield return unit;
                }
            }
        }

        //TODO REMOVE
        [FullInspector.InspectorDropdownName("Effects/Visible")]
        public class Visibles : UnitSelectorBase
        {
            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                throw new System.NotImplementedException();
            }


        }

        [FullInspector.InspectorDropdownName("Effects/Locks")]
        public class Locks : UnitSelectorBase
        {
            public LockTag Lock;
            public bool HasLock;

            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                return targets.Where(u => HasLock == u.HasLock(Lock));
            }
        }


        [FullInspector.InspectorDropdownName("Effects/Has Effect")]
        public class WithEffect : UnitSelectorBase
        {
            [SerializeField] StatusEffect StatusEffect;

            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                foreach (var c in targets)
                {
                    if (c.Effects.Any(e => e.Effect == StatusEffect))
                    {
                        yield return c;
                    }
                }
            }

            public override string ToString()
            {
                return string.Format("With Effect: {0}", StatusEffect.name);
            }
        }

        [FullInspector.InspectorDropdownName("Effects/Has Effect (Remain Turns)")]
        public class WithEffectRemainTurns : UnitSelectorBase
        {
            [SerializeField] StatusEffect StatusEffect;
            [SerializeField] Comparator<int> RemainTurns;

            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                foreach (var c in targets)
                {
                    if (c.Effects.Any(e => e.Effect == StatusEffect && RemainTurns.IsMet(e.RemainTurns)))
                    {
                        yield return c;
                    }
                }
            }

            public override string ToString()
            {
                return string.Format("With Effect: {0}", StatusEffect.name);
            }
        }


        [FullInspector.InspectorDropdownName("Effects/Has NO Effect")]
        public class WithoutEffect : UnitSelectorBase
        {
            [SerializeField] StatusEffect StatusEffect;

            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                foreach (var c in targets)
                {
                    if (!c.Effects.Any(e => e.Effect == StatusEffect))
                    {
                        yield return c;
                    }
                }
            }
        }

        [FullInspector.InspectorDropdownName("Tags/With character tags")]
        public class WithTags : UnitSelectorBase
        {
            [SerializeField] List<UnitTag> Tags = new List<UnitTag>();
            enum Type
            {
                AnyOfStatuses,
                AllStatuses
            }
            [SerializeField] Type MatchType;

            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                foreach (var c in targets)
                {
                    var intersected = c.Tags.Intersect(Tags);
                    if (MatchType == Type.AllStatuses && intersected.Count() == Tags.Count)
                    {
                        yield return c;
                    }
                    else if (MatchType == Type.AnyOfStatuses && intersected.Any())
                    {
                        yield return c;
                    }
                }
            }
        }

        [FullInspector.InspectorDropdownName("Tags/With spawn tag")]
        public class WithSpawnTag : UnitSelectorBase
        {
            [SerializeField] string Tag;

            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                foreach (var c in targets)
                {
                    if (c.SpawnTag == Tag)
                    {
                        yield return c;
                    }
                }
            }
        }

        [FullInspector.InspectorDropdownName("Area/Inside")]
        public class InArea : UnitSelectorBase
        {
            [SerializeField] AreaTag Area;

            static LevelAreasLayer LevelAreasLayer;

            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                if (!LevelAreasLayer)
                {
                    LevelAreasLayer = LevelAreasLayer.SceneInstance;
                }
                foreach (var c in targets)
                {
                    if (LevelAreasLayer != null && LevelAreasLayer.IsInsideArea(c.Position, Area))
                    {
                        yield return c;
                    }
                    else if (WorldStaticZone.IsInsideArea(c.Position, Area))
                    {
                        yield return c;
                    }
                }
            }
        }

        [FullInspector.InspectorDropdownName("Area/Inside (by name)")]
        public class InAreaByName : UnitSelectorBase
        {
            [SerializeField] string AreaName;

            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                foreach (var c in targets)
                {
                    if (WorldStaticZone.IsInsideAreaByName(c.Position, AreaName))
                    {
                        yield return c;
                    }
                }
            }
        }

        [InspectorDropdownName("Damage/Can be damaged by")]
        public class CanBeDamagedBy : UnitSelectorBase
        {

            public static readonly CanBeDamagedBy Instance = new CanBeDamagedBy();

            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                return targets.Where(c => c.CanBeDamaged(context.ActiveWeapon));
            }
        }

        [InspectorDropdownName("Context/Nested data")]
        public class FromNestedData : UnitSelectorBase
        {

            [SerializeField] int NestedIndex = 0;
            [SerializeField] bool ReturnTargets = true;

            public FromNestedData(int nestedIndex, bool returnTargets = true)
            {
                NestedIndex = nestedIndex;
                ReturnTargets = returnTargets;
            }

            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                if (context.Nested == null || NestedIndex < 0 || NestedIndex >= context.Nested.Count)
                {
                    Log.e("Incorrect using of FromNestedData Selector");
                    yield break;
                }
                if (ReturnTargets)
                {
                    foreach (var target in context.Nested[NestedIndex].Targets)
                    {
                        yield return target;
                    }
                }
            }
        }

        [FullInspector.InspectorDropdownName("Damage/Owner victims (current turn)")]
        public class DamagedByOwnerInTurn : UnitSelectorBase
        {

            [SerializeField] bool filterNotDamaged;

            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {

                if (filterNotDamaged)
                {
                    foreach (var target in targets)
                    {
                        if (!context.Session.Battle.CurrentTurnState.HasHealthRecord(target, context.Owner))
                        {
                            yield return target;
                        }
                    }
                }
                else
                {
                    foreach (var target in targets)
                    {
                        if (context.Session.Battle.CurrentTurnState.HasHealthRecord(target, context.Owner))
                        {
                            yield return target;
                        }
                    }
                }
            }
        }


        [FullInspector.InspectorDropdownName("Regimes/Citizen Salvation/With Citizen")]
        public class CitizenSalvationWithCitizens : UnitSelectorBase
        {
            public IEnumerable<Unit> Select(IExecutorContext context) { return Select(context, context.Targets); }

            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                var regime = context.Battle.Get<CitizenSalvationRegimeEntity>();
                return targets.Where(t => regime.GetCitizenFor(t) != null);
            }
        }

        [FullInspector.InspectorDropdownName("Regimes/Citizen Salvation/Can Escape")]
        public class CitizenSalvationCanEscape : UnitSelectorBase
        {
            public IEnumerable<Unit> Select(IExecutorContext context) { return Select(context, context.Targets); }

            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                var regime = context.Battle.Get<CitizenSalvationRegimeEntity>();
                return targets.Where(t => regime.CanAutoEscape(t));
            }
        }

        [FullInspector.InspectorDropdownName("Regimes/Citizen Salvation/Without Citizen")]
        public class CitizenSalvationWithoutCitizen : UnitSelectorBase
        {
            public IEnumerable<Unit> Select(IExecutorContext context) { return Select(context, context.Targets); }

            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                var regime = context.Battle.Get<CitizenSalvationRegimeEntity>();
                return targets.Where(t => regime.GetCitizenFor(t) == null);
            }
        }

        [FullInspector.InspectorDropdownName("Units/From owner memory (if exists)")]
        public class FromOwnerMemory : UnitSelectorBase
        {
            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                var memory = context.Owner as UnitMemoryEntity;
                if (memory != null)
                {
                    memory.RefreshMemory();
                    foreach (var u in memory.UnitsInMemory)
                    {
                        yield return u;
                    }
                }
            }
        }

        [FullInspector.InspectorDropdownName("Vitality/Is dead")]
        public class IsDead : UnitSelectorBase
        {
            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                foreach (var t in targets)
                {
                    if (t.IsDead)
                    {
                        yield return t;
                    }
                }
            }
        }

        [FullInspector.InspectorDropdownName("Vitality/Is alive")]
        public class IsAlive : UnitSelectorBase
        {
            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                foreach (var t in targets)
                {
                    if (!t.IsDead)
                    {
                        yield return t;
                    }
                }
            }
        }

        [FullInspector.InspectorDropdownName("Heroes/Is hero")]
        public class IsHero : UnitSelectorBase
        {
            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                if (!context.Session.Battle.Has<CampaignEntity>()) { yield break; }
                var campaign = context.Session.Battle.Get<CampaignEntity>();
                foreach (var t in targets)
                {
                    if (campaign.Heroes.Any(h => h.CharacterEntity == t))
                    {
                        yield return t;
                    }
                }
            }
        }

        [FullInspector.InspectorDropdownName("Heroes/Selected hero")]
        public class SelectedHero : UnitSelectorBase
        {
            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                if (!context.Session.Battle.Has<CampaignEntity>()) { yield break; }
                yield return BattleSession.SelectedCharacter;
            }
        }

        [FullInspector.InspectorDropdownName("Heroes/Is passive hero")]
        public class IsPassiveHero : UnitSelectorBase
        {
            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                if (!context.Session.Battle.Has<CampaignEntity>()) { yield break; }
                var campaign = context.Session.Battle.Get<CampaignEntity>();
                foreach (var t in targets)
                {
                    if (campaign.PassiveHeroes.Any(h => h.CharacterEntity == t))
                    {
                        yield return t;
                    }
                }
            }
        }


        [FullInspector.InspectorDropdownName("Player/NOT controlled by player")]
        public class NotPlayerCharacter : UnitSelectorBase
        {
            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                foreach (var t in targets)
                {
                    if (!context.Battle.Players.Any(x => x.CharacterEntity == t))
                    {
                        yield return t;
                    }
                }
            }
        }

        [FullInspector.InspectorDropdownName("Unit type/Character")]
        public class IsCharacterEntity : UnitSelectorBase
        {
            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                foreach (var t in targets)
                {
                    if (t is CharacterEntity)
                    {
                        yield return t;
                    }
                }
            }
        }

        [InspectorDropdownName("Unit type/Memory")]
        public class IsMemoryEntity : UnitSelectorBase
        {
            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                foreach (var t in targets)
                {
                    if (t is UnitMemoryEntity)
                    {
                        yield return t;
                    }
                }
            }
        }

        [InspectorDropdownName("Unit type/World object (bridge)")]
        public class IsWorldObjectBridgeEntity : UnitSelectorBase
        {
            [SerializeField] List<WorldObjectUnitBridgeDescription> Descriptions;
            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                if (Descriptions == null || Descriptions.Count == 0)
                {
                    foreach (var t in targets)
                    {
                        if (t is WorldObjectUnitBridgeEntity)
                        {
                            yield return t;
                        }
                    }
                }
                else
                {
                    foreach (var t in targets)
                    {
                        var bridge = t as WorldObjectUnitBridgeEntity;
                        if (bridge != null && Descriptions.Contains(bridge.Description))
                        {
                            yield return t;
                        }
                    }
                }
            }
        }

        [FullInspector.InspectorDropdownName("Characters/By character description")]
        public class ByCharacterDescription : UnitSelectorBase
        {
            [SerializeField] CharacterDescription Description;
            [SerializeField] bool UnitsFromTargets;
            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                var units = UnitsFromTargets ? targets : context.Session.Battle.Units;
                foreach (var t in units)
                {
                    var character = t as CharacterEntity;
                    if (character != null && character.ModelDescription == Description)
                    {
                        yield return t;
                    }
                }
            }
        }

        [FullInspector.InspectorDropdownName("Logic/Not")]
        public class Not : UnitSelectorBase
        {
            [SerializeField] IUnitSelector Selector;
            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                var characters = Selector.Select(context, targets);
                foreach (var c in targets)
                {
                    if (!characters.Contains(c))
                    {
                        yield return c;
                    }
                }
            }
        }

        [InspectorDropdownName("Logic/Sum")]
        public class Sum : UnitSelectorBase
        {
            [SerializeField] IUnitSelector[] Selectors;

            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                var result = new List<Unit>();
                foreach (var s in Selectors)
                {
                    result.AddRange(s.Select(context));
                }
                foreach (var r in result)
                {
                    yield return r;
                }
            }
        }

        [FullInspector.InspectorDropdownName("Units/Is Revealed")]
        public class IsRevealed : UnitSelectorBase
        {
            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                foreach (var c in targets)
                {
                    if (c.Revealed)
                    {
                        yield return c;
                    }
                }
            }
        }

        [FullInspector.InspectorDropdownName("Shared States/Move Target")]
        public class SharedStatedMoveTarget : UnitSelectorBase
        {

            [SerializeField] CharacterFraction Fraction;

            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                var gameAI = context.Session.Battle.Get<AI.GameAIEntity>();
                var moveTargets = gameAI.FractionMoveTargets.TryGetOrDefault(Fraction);
                return moveTargets == null ? Enumerable.Empty<Unit>() : moveTargets.Targets;
            }
        }

        [FullInspector.InspectorDropdownName("Player/Is controlled by player")]
        public class IsControlledByPlayerControlled : UnitSelectorBase
        {

            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                foreach (var c in targets)
                {
                    if (context.Session.Battle.Players.Any(p => !p.IsBot && p.CharacterEntity == c))
                    {
                        yield return c;
                    }
                }
            }
        }

        [FullInspector.InspectorDropdownName("Move/Range from owner")]
        public class MovePathRange : UnitSelectorBase
        {

            public IValueGetter Range;

            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                var owner = context.Owner;
                var map = context.Session.Battle.Map;
                var path = new List<Vector3>();
                var maxDistance = Range.Get(context);
                foreach (var target in targets)
                {
                    path.Clear();
                    if (!map.PreparePath(owner.Position, target.Position, path))
                    {
                        continue;
                    }
                    var distance = NavigationExtensions.CalculateDistanceOfPhasePoints(path);
                    if (distance < maxDistance)
                    {
                        yield return target;
                    }

                }
            }
        }

        [InspectorDropdownName("Move/Area (from owner)")]
        public class MoveAreaOwner : UnitSelectorBase
        {

            public IValueGetter Range;

            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                var owner = context.Owner;
                var map = context.Session.Battle.Map;
                var range = Range.Get(context);
                range += 0.5f; //VU. Done to sync with old logic
                var roundRange = (int)(10f * range);
                var depth = (int)(range);
                map.PrepareDitstanceField(owner.Position, (int)range);
                foreach (var target in targets)
                {
                    var delta = owner.Position - target.Position;
                    if (-depth <= delta.x && delta.x <= depth && -depth <= delta.y && delta.y <= depth)
                    {
                        var cell = map.GetCell(target.Position);
                        if (cell != null && (cell as Pathfinding.INode).Distance <= roundRange)
                        {
                            yield return target;
                        }
                    }
                }
            }
        }

        [FullInspector.InspectorDropdownName("Distance/Nearest to any hero")]
        public class NearestTargetToAnyHero : UnitSelectorBase
        {

            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                if (context.Session.Type == WorldLoadingState.Types.Campaign)
                {
                    Unit nearest = null;
                    float distanceSqr = float.MaxValue;
                    foreach (var c in context.Battle.Get<CampaignEntity>().GetValidHeroesCharacters())
                    {

                        var unit = context.Session.Battle.Map.NearestUnitTo(c.Position, targets);
                        if (unit != null)
                        {
                            var sqr = Vector3.SqrMagnitude(unit.Position - c.Position);
                            if (sqr < distanceSqr)
                            {
                                nearest = unit;
                                distanceSqr = sqr;
                            }
                        }
                    }
                    if (nearest != null)
                    {
                        yield return nearest;
                    }
                }
            }
        }


        [FullInspector.InspectorDropdownName("Distance/Nearest Target(deteministic)")]
        public class NearestTarget : UnitSelectorBase
        {

            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                var nearest = context.Session.Battle.Map.NearestUnitTo(context.Owner.Position, targets);
                if (nearest != null)
                {
                    yield return nearest;
                }
            }
        }

        [FullInspector.InspectorDropdownName("Distance/Farthest Target(deteministic)")]
        public class FarthestTarget : UnitSelectorBase
        {
            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                var fatherst = context.Session.Battle.Map.FatherstUnitTo(context.Owner.Position, targets);
                if (fatherst != null)
                {
                    yield return fatherst;
                }
            }
        }

        [FullInspector.InspectorDropdownName("Units/MemoryEntity/Targets Contains In Memory")]
        public class UnitMemoryEntityUnitsInMemory : UnitSelectorBase
        {

            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                var unit = context.Owner as UnitMemoryEntity;
                if (unit == null)
                {
                    Log.e("Can't use UnitMemoryEntityUnitsInMemory when Context Owner = {0} is Null or not Memory Entity", context.Owner);
                    yield break;
                }
                foreach (var t in targets)
                {
                    if (unit.UnitsInMemory.Contains(t))
                    {
                        yield return t;
                    }
                }
            }
        }

        [FullInspector.InspectorDropdownName("Units/MemoryEntity/Owner")]
        public class UnitMemoryEntityOwner : UnitSelectorBase
        {

            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                var unit = context.Owner as UnitMemoryEntity;
                if (unit == null)
                {
                    Log.e("Can't use UnitMemoryEntityUnitsInMemory when Context Owner = {0} is Null or not Memory Entity", context.Owner);
                    yield break;
                }
                yield return unit.OriginContext.Owner;
            }
        }


        [FullInspector.InspectorDropdownName("Heroes/All")]
        public class CampaingAllHeroes : UnitSelectorBase
        {

            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                if (!context.Session.Battle.Has<CampaignEntity>()) { yield break; }
                foreach (var c in context.Session.Battle.Get<CampaignEntity>().Heroes)
                {
                    if (c.CharacterEntity != null)
                    {
                        yield return c.CharacterEntity;
                    }
                }
            }
        }

        [FullInspector.InspectorDropdownName("Heroes/All with passive")]
        public class CampaingAllHeroesWithPassive : UnitSelectorBase
        {

            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                if (!context.Session.Battle.Has<CampaignEntity>()) { yield break; }
                var campaign = context.Session.Battle.Get<CampaignEntity>();
                foreach (var c in campaign.Heroes.Concat(campaign.PassiveHeroes))
                {
                    if (c.CharacterEntity != null)
                    {
                        yield return c.CharacterEntity;
                    }
                }
            }
        }

        [InspectorDropdownName("Positions/From Spawn Area")]
        public class FromSpawnArea : UnitSelectorBase
        {
            public string SpawnAreaCaption;
            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                var area = context.Battle.Get<CharacterSpawnControllerEntity>().AllAreas.FirstOrDefault(a => a.SpawnArea.Caption == SpawnAreaCaption);
                if (area == null)
                {
                    Log.e("Invalid using of FromSpawnArea (UnitSelector). No spawn area with caption = {0}", SpawnAreaCaption);
                    yield break;
                }

                foreach (var t in targets)
                {
                    if (area.Contains(t.Position))
                    {
                        yield return t;
                    }
                }
            }
        }

        [InspectorDropdownName("Damage/Last damager")]
        public class LastDamager : UnitSelectorBase
        {

            [SerializeField] ITriggerCondition Condition;

            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                if (Condition == null)
                {
                    yield return context.Battle.Get<HealthLogEntity>().GetLastDamager(context.Owner);
                }
                var orderedDamagers = context.Battle.Get<HealthLogEntity>().GetDamagers(context.Owner, Condition);
                var nearestDamager = orderedDamagers.Where(o => targets.Contains(o))
                                                   .OrderBy(o => Vector3.SqrMagnitude(context.Owner.Position - o.Position))
                                                   .FirstOrDefault();
                if (nearestDamager != null)
                {
                    yield return nearestDamager;
                }
            }
        }

        [InspectorDropdownName("Damage/Do damage owner (current turn)")]
        public class DamageDoDamageOwner : UnitSelectorBase
        {

            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                return context.Battle.CurrentTurnState.DamagersOf(context.Owner);
            }
        }

        [InspectorDropdownName("Heroes/Description (find in targets)")]
        public class FromHeroSelector : UnitSelectorBase
        {

            [SerializeField] HeroSettings Hero;

            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                foreach (var t in targets)
                {
                    var character = t as CharacterEntity;
                    if (Hero.IsMe(character))
                    {
                        yield return t;
                    }
                }
            }
        }


        [InspectorDropdownName("Heroes/Description (active)")]
        public class GetHeroSelector : UnitSelectorBase
        {

            [SerializeField] HeroSettings Hero;

            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                if (!context.Session.Battle.Has<CampaignEntity>()) { yield break; }
                var hero = context.Session.Battle.Get<CampaignEntity>().Heroes.FirstOrDefault(h => Hero.IsMe(h.CharacterEntity));
                if (hero != null)
                {
                    yield return hero.CharacterEntity;
                }
            }
        }

        [InspectorDropdownName("Heroes/Description (passive)")]
        public class GetHeroSelectorPassive : UnitSelectorBase
        {

            [SerializeField] HeroSettings Hero;

            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                if (!context.Session.Battle.Has<CampaignEntity>()) { yield break; }
                var hero = context.Session.Battle.Get<CampaignEntity>().PassiveHeroes.FirstOrDefault(h => Hero.IsMe(h.CharacterEntity));
                if (hero != null)
                {
                    yield return hero.CharacterEntity;
                }
            }
        }

        [InspectorDropdownName("Move/Reachable owner")]
        public class IsWalkReachable : UnitSelectorBase
        {
            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                var map = context.Battle.Map;
                return targets.Where(t => map.IsReachable(context.Owner.Position, t.Position));
            }
        }

        [InspectorDropdownName("Weapons/Target (for melee)")]
        public class TargetForMelee : UnitSelectorBase
        {
            public override IEnumerable<Unit> Select(IExecutorContext context, IEnumerable<Unit> targets)
            {
                var attacker = context.Owner;
                var map = context.Battle.Map;
                return targets.Where(t => CanBeTargetForMelee(t, attacker, map));
            }

            bool CanBeTargetForMelee(Unit unit, Unit attacker, MapEntity map)
            {
                var unitPosition = unit.Position;
                foreach (var d in MapEntity.NeighbourDirections)
                {
                    var checkPos = d + unitPosition;
                    var cell = map.GetCell(checkPos);
                    if (cell != null && (!cell.HasUnit || cell.Unit == attacker) && !cell.HasObstacle)
                    {
                        var distance = d.magnitude;
                        if (!map.HasObstacleCast(unitPosition, d / distance, distance))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }
        */
    }
}
