using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public abstract class Projectile : BattleObject
{
    public static Projectile Create(ProjectileConfig config, IExecutorContext context, float startOffset = 0, Unit target = null, float possibleTargetDamage = 0)
    {
        if (!config) { return null; }
        Projectile projectile;
        if (config.Mode == ProjectileConfig.Modes.Direction)
        {
            projectile = new DirectProjectile();
        }
        else
        {
            projectile = new TargetProjectile(target, possibleTargetDamage);
        }
        projectile.Config = config;
        projectile.Context = context;
        if (projectile.Context.Projectile != null)
        {
            Debug.LogError("Context already has projectile need create copy");
            projectile.Context = projectile.Context.Copy();

        }
        projectile.Context.Projectile = projectile;
        projectile.BattleSimulator = context.BattleSimulator;
        var dir = (context.TargetPoint.Position - context.AttackPoint.Position).normalized * startOffset;
        projectile.Position = context.AttackPoint.Position + dir;
        projectile.Speed = new UnitStat(context.Stats.GetStat(StatType.ProjectileSpeed));
        projectile.Init();
        projectile.BattleSimulator.AddProjectile(projectile);
        return projectile;
    }


    protected Projectile() { }
    protected virtual void Init() { }

    public List<IProjectileAction> OnDeathAdditionalActions { get; } = new List<IProjectileAction>();
    public Vector2 Position { get; protected set; }
    public virtual Vector3 PositionForView => Position.ConvertFieldPositionToWorld();
    public UnitStat Speed { get; private set; }
    public ProjectileConfig Config { get; private set; }
    public virtual Vector2 TargetPosition { get { return Context.TargetPoint.Position; } }

    public bool IsDead
    {
        get { return Dead; }
        set
        {

            var dead = Dead;
            Dead = value;
            if (Dead == true && dead == false)
            {
                foreach (var action in OnDeathAdditionalActions)
                {
                    action.Apply(this, Context);
                }
                if (Dead) //Dead can be overrided by on death actions
                {
                    OnDead?.Invoke();
                }
            }
        }
    }
    public event System.Action OnDead;
    private bool Dead;

    public IExecutorContext Context { get; protected set; }
    private float ElapsedPeriodicTime;

    protected bool CanApply(Unit unit)
    {
        if (Context.Battle.FriendlyFire)
        {
            return true;
        }

        if (unit.IsLandscape())
        {
            return false;
        }

        return !Context.Owner.IsAlly(unit);
    }

    protected void UpdatePeriodicAction(float dt)
    {
        if (Config.PeriodicAction == null) { return; }
        ElapsedPeriodicTime += dt;
        if (ElapsedPeriodicTime >= Config.PeriodicInterval)
        {
            ElapsedPeriodicTime -= Config.PeriodicInterval;
            var context = Context.Copy();
            context.TargetPoint = new PositionTargetPoint(Position);
            Config.PeriodicAction.Run(context);
        }
    }

    public void Update(float dt)
    {
        if (IsDead) { return; }
        UpdatePeriodicAction(dt);
        InternalUpdate(dt);
    }

    protected abstract void InternalUpdate(float dt);

}