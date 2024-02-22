using System;
using System.Collections.Generic;

public class UnitSpawnAreaOrderRandom : IUnitSpawnAreaOrder
{
    public void Apply(List<SpawnPoint> spawnPoints, float delayStep, int pointCount, Type type)
    {
        var delayStepBase = 0f;
        var delays = new float[spawnPoints.Count];

        for (int i = 0; i < delays.Length; i++)
        {
            delays[i] = delayStepBase;
            delayStepBase += delayStep;
        }

        delays.Shuffle();

        for (int i = 0; i < spawnPoints.Count; i++)
        {
            spawnPoints[i].SetStartDelay(delays[i]);
        }
    }
}