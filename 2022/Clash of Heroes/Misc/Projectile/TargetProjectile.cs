using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetProjectile : Projectile
{
    private float HitRadiusSqr;
    private int HitCount;
    private HashSet<Unit> HittedUnits = new HashSet<Unit>();
    private Unit Target;

    private IProjectileTrajectory Trajectory;
    private Vector3 PosForView;
    public override Vector3 PositionForView => PosForView;

    public TargetProjectile(Unit target, float possibleTargetDamage)
    {
        Target = target;
        if (Target != null)
        {
            Target.Stats.Health.AddPossibleDamage(this, possibleTargetDamage);
        }
    }

    protected override void Init()
    {
        if (Config.Target.HitRadius != null)
        {
            HitRadiusSqr = Config.Target.HitRadius.Get(Context);
            HitRadiusSqr *= HitRadiusSqr;
        }

        var trajectory = Config.Target.Trajectory ?? new LineProjectileTrajectory();
        Trajectory = trajectory.Clone(Context);
        PosForView = Position.ConvertFieldPositionToWorld();
    }

    protected override void InternalUpdate(float dt)
    {
        if (Config.Target.OnHitUnit != null && Config.Target.HitCount > 0)
        {
            foreach (var u in Context.Battle.Units)
            {
                if (HittedUnits.Contains(u) || !CanApply(u)) continue;
                var isHit = (Position - u.Position).sqrMagnitude <= u.Stats.CollisionRadiusSqr + HitRadiusSqr;
                if (isHit)
                {
                    HittedUnits.Add(u);
                    var context = ExecutorContext.Create(Context.Owner, u, Context);
                    Config.Target.OnHitUnit?.Run(context);
                    HitCount++;
                    if (HitCount >= Config.Target.HitCount)
                    {
                        Config.OnDestroy?.Run(context);
                        IsDead = true;
                        return;
                    }
                    else if (Config.Target.NextTargetAfterHit != null)
                    {
                        var nextTarget = Config.Target.NextTargetAfterHit.Select(Context).Where(t => !HittedUnits.Contains(t)).FirstOrDefault();
                        if (nextTarget != null)
                        {
                            Context = ExecutorContext.Create(Context.Owner, nextTarget, Context);
                        }

                    }
                }
            }
        }

        var target = TargetPosition;

        Position = Vector2.MoveTowards(Position, target, Speed * dt);
        PosForView = Trajectory.GetPosition(dt);

        IsDead = (Position - target).sqrMagnitude < Mathf.Epsilon;
        if (IsDead)
        {
            Config.OnDestroy?.Run(Context);
            if (Target != null)
            {
                Target.Stats.Health.RemovePossibleDamage(this);
            }
        }
    }
}
