using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public interface IProjectileAction
{
    void Apply(Projectile projectile, IExecutorContext context);
}


namespace ProjectileActions
{
    public class Empty : IProjectileAction
    {
        public void Apply(Projectile projectile, IExecutorContext context)
        {
        }

    }

    [DropdownName("List Of Actions")]
    public class List : IProjectileAction
    {
        [TypesDropdown]
        [SerializeField] List<IProjectileAction> Actions;

        public void Apply(Projectile projectile, IExecutorContext context)
        {
            foreach (var a in Actions)
            {
                a.Apply(projectile, context);
            }
        }

    }

    public class IfElse : IProjectileAction
    {
        [SerializeField] IExecutorContextTrigger Condition;

        [TypesDropdown, SerializeField] IProjectileAction IfAction;
        [TypesDropdown, SerializeField] IProjectileAction ElseAction;

        public void Apply(Projectile projectile, IExecutorContext context)
        {
            Debug.LogError("TODO");
            /*
            if (Condition.CanTrigger(projectile, context))
            {
                IfAction?.Apply(projectile, context);
            }
            else
            {
                ElseAction?.Apply(projectile, context);
            }*/
        }

    }

    public class ChangeSpeedMultiplayerWithContextHolder : IProjectileAction
    {
        public IValueGetter Multiplayer;

        public void Apply(Projectile projectile, IExecutorContext context)
        {
            projectile.Speed.AddMultiplayer(Multiplayer.Get(context), context);
        }
    }

    public class RemoveSpeedMultiplayerWithContextHolder : IProjectileAction
    {
        public void Apply(Projectile projectile, IExecutorContext context)
        {
            projectile.Speed.RemoveMultiplayer(context);
        }
    }

    public class AddOnDeathExecutor : IProjectileAction
    {
        [SerializeField, TypesDropdown] IExecutor Executor;
        public void Apply(Projectile projectile, IExecutorContext context)
        {
            projectile.OnDeathAdditionalActions.Add(new RunExecutor { ContextFromProjectile = true, Executor = Executor });
        }
    }

    public class AddOnDeathAction : IProjectileAction
    {
        [SerializeField, TypesDropdown] IProjectileAction Action;
        public void Apply(Projectile projectile, IExecutorContext context)
        {
            projectile.OnDeathAdditionalActions.Add(Action);
        }
    }

    public class RunExecutor : IProjectileAction
    {
        [TypesDropdown]
        public IExecutor Executor;
        public bool ContextFromProjectile;

        public void Apply(Projectile projectile, IExecutorContext context)
        {
            Executor.Run(ContextFromProjectile ? projectile.Context : context);
        }
    }

    public class FindNextTarget : IProjectileAction
    {
        [SerializeField, TypesDropdown] Selectors.IUnitSelector NextTarget;
        [SerializeField] bool ContextFromProjectile;
        [SerializeField] int MaxTargets;
        [SerializeField] float DamageDecrease;
        public void Apply(Projectile projectile, IExecutorContext context)
        {
            if (projectile.Context.Targets is List<Unit> list)
            {
                if (list.Count < MaxTargets)
                {
                    var target = NextTarget.Select(ContextFromProjectile ? projectile.Context : context).FirstOrDefault();
                    if (target != null)
                    {
                        if (projectile.IsDead)
                        {
                            var stats = new TempStatGetter
                            {
                                Force = projectile.Context.Stats?.Force,
                                Stats = new UnitStats(projectile.Context.Owner, projectile.Context.Stats?.GetUnitStats())
                            };
                            stats.Stats.SetBase(StatType.Damage, stats.Stats.Damage * Mathf.Max(0, (1 - DamageDecrease * list.Count)));
                            var c = projectile.Context.Copy();
                            c.Stats = stats;
                            projectile.Config.OnDestroy?.Run(c);
                        }
                        list.Add(target);
                        projectile.IsDead = false;
                        projectile.Context.TargetPoint = new UnitTargetPoint(target);
                    }
                }
            }
        }
    }

    public class SetInt : IProjectileAction
    {
        [SerializeField] string Name;
        [SerializeField] IValueGetter Value;

        public void Apply(Projectile projectile, IExecutorContext context)
        {
            projectile.Add<int>((int)Value.Get(context), Name);
        }
    }

}
