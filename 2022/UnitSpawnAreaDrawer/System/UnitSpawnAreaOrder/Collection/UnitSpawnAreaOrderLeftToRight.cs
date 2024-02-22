using System;
using System.Collections.Generic;

public class UnitSpawnAreaOrderLeftToRight : IUnitSpawnAreaOrder
{
    public void Apply(List<SpawnPoint> spawnPoints, float delayStep, int pointCount, Type type)
    {
        float delayStepBase = 0f;

        if (type == typeof(UnitSpawnAreaRectangle))
        {
            for (int i = 0; i < spawnPoints.Count; i++)
            {
                spawnPoints[i].SetStartDelay(delayStepBase);
                delayStepBase += delayStep;

                if ((i + 1) % pointCount == 0)
                {
                    delayStepBase = 0f;
                }
            }
        }
        else
        {
            foreach (var item in spawnPoints)
            {
                item.SetStartDelay(delayStepBase);
                delayStepBase += delayStep;
            }
        }
    }
}