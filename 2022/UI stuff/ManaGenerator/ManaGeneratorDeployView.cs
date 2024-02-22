using UnityEngine;

//TODO REMOVE Duplicate DeployFriendlyUnitsView
public class ManaGeneratorDeployView : DeployView
{
    protected override Vector2 GetDeployPosition()
    {
        var preview = GetComponent<ManaGeneratorDeployPreviewModel>();
        var position = preview.TargetPoint;
        
        var field = BattleSimulator.State.Field;
        if (field.HasBounceObstacle(position))
            position = field.GetNearestObstacleFreePosition(position);
        
        return position;
    }
}
