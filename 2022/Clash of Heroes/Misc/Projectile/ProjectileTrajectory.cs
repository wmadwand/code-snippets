using UnityEngine;

public interface IProjectileTrajectory
{
    IProjectileTrajectory Clone(IExecutorContext Context);
    Vector3 GetPosition(float dt);
}

public class ArcProjectileTrajectory : IProjectileTrajectory
{
    public float ArcHeight = 10;
    private Vector3 StartPosition;
    private Vector3 TargetPosition;
    private float ArcPointPosX;
    private IExecutorContext Context;

    public IProjectileTrajectory Clone(IExecutorContext Context)
    {
        return new ArcProjectileTrajectory(this, Context);
    }

    private ArcProjectileTrajectory(ArcProjectileTrajectory tr, IExecutorContext context)
    {
        ArcHeight = tr.ArcHeight;
        Context = context;
        StartPosition = Context.Projectile.Position.ConvertFieldPositionToWorld();
        TargetPosition = Context.Projectile.TargetPosition.ConvertFieldPositionToWorld();
    }

    public Vector3 GetPosition(float dt)
    {
        var direction = (TargetPosition - StartPosition).normalized;
        var distance = (TargetPosition - StartPosition).magnitude;
        float nextX = Mathf.MoveTowards(ArcPointPosX, distance, Context.Projectile.Speed * dt);
        float baseY = Mathf.Lerp(StartPosition.y, TargetPosition.y, nextX / distance);
        float arc = ArcHeight * nextX * (nextX - distance) / (-0.25f * distance * distance);

        var nextPos = StartPosition + direction * nextX;
        ArcPointPosX = nextX;

        return nextPos + new Vector3(0, baseY + arc, 0);
    }
}

public class LineProjectileTrajectory : IProjectileTrajectory
{
    private Vector2 TargetPosition;
    private IExecutorContext Context;

    public LineProjectileTrajectory() { }

    public IProjectileTrajectory Clone(IExecutorContext context)
    {
        return new LineProjectileTrajectory(this, context);
    }

    public Vector3 GetPosition(float dt)
    {
        var pos = Vector2.MoveTowards(Context.Projectile.Position, TargetPosition, Context.Projectile.Speed * dt);
        return pos.ConvertFieldPositionToWorld();
    }

    private LineProjectileTrajectory(LineProjectileTrajectory tr, IExecutorContext context)
    {
        Context = context;
        TargetPosition = Context.Projectile.TargetPosition;
    }
}