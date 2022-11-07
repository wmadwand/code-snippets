using System;
using System.Collections.Generic;

public class UnitSpawnAreaOrderEdgeToCenter : IUnitSpawnAreaOrder
{
    public void Apply(List<SpawnPoint> spawnPoints, float delayStep, int pointCount, Type type)
    {
        if (type == typeof(UnitSpawnAreaLine))
        {
            float delayStepBase = 0f;

            for (int i = 0; i < spawnPoints.Count; i++)
            {
                spawnPoints[i].SetStartDelay(delayStepBase);

                if (i < spawnPoints.Count / 2)
                {
                    delayStepBase += delayStep;

                    if (spawnPoints.Count % 2 == 0 && (i + 1) == spawnPoints.Count / 2)
                    {
                        delayStepBase -= delayStep;
                    }
                }
                else if (i >= spawnPoints.Count / 2)
                {
                    delayStepBase -= delayStep;
                }
            }

            spawnPoints[spawnPoints.Count - 1].SetStartDelay(0);
        }
        else if (type == typeof(UnitSpawnAreaRectangle))
        {
            float delayStepBase = 0f;
            var i = 0;

            foreach (var sPoint in spawnPoints)
            {
                sPoint.SetStartDelay(delayStepBase);

                if (i < pointCount / 2)
                {
                    delayStepBase += delayStep;

                    if (pointCount % 2 == 0 && (i + 1) == pointCount / 2)
                    {
                        delayStepBase -= delayStep;
                    }
                }
                else if (i >= pointCount / 2)
                {
                    delayStepBase -= delayStep;
                }

                if ((i + 1) % pointCount == 0)
                {
                    delayStepBase = 0f;
                    sPoint.SetStartDelay(0);
                    i = 0;
                }
                else
                {
                    i++;
                }
            }
        }
    }
}