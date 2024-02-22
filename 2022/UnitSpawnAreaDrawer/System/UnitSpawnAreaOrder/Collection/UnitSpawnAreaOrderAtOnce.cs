using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawnAreaOrderAtOnce : IUnitSpawnAreaOrder
{
    public void Apply(List<SpawnPoint> spawnPoints, float delayStep, int pointCount, Type type)
    {
        foreach (var item in spawnPoints)
        {
            item?.SetStartDelay(delayStep);
        }
    }
}