using System;
using System.Collections.Generic;

public class UnitSpawnAreaOrderBottomToTop : IUnitSpawnAreaOrder
{
    public void Apply(List<SpawnPoint> spawnPoints, float delayStep, int pointCount, Type type)
    {
        if (type == typeof(UnitSpawnAreaRectangle))
        {
            float delayStepBase = 0f;

            for (int i = spawnPoints.Count - 1; i >= 0; i--)
            {
                spawnPoints[i].SetStartDelay(delayStepBase);

                if (i % pointCount == 0)
                {
                    delayStepBase += delayStep;
                }
            }
        }
        else if (type == typeof(UnitSpawnAreaTriangle))
        {
            float delayStepBase = 0f;
            var rate = pointCount;
            var counter = 0;

            //TODO: fix bug with >15 points
            for (int i = 0; i < spawnPoints.Count; i++)
            {
                spawnPoints[i].SetStartDelay(delayStepBase);

                counter++;
                if (counter % rate == 0)
                {
                    rate -= 2;
                    delayStepBase += delayStep;
                    counter = 0;
                }
            }
        }
    }
}