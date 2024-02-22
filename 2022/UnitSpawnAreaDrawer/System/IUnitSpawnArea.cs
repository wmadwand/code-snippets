using System;
using System.Collections.Generic;

public interface IUnitSpawnArea
{
    //TODO: extract another one interface
    void InputHandler();
    void DragHandler();    
    //

    void Refresh(UnitSpawnAreaOrderFactory spawnOrderFactory, bool withBasePoints = true);
    void Init(SpawnPoint prefab, UnitSpawnAreaOrderFactory spawnOrderFactory);
    void Destroy();
}

public interface IUnitSpawnAreaOrder
{
    void Apply(List<SpawnPoint> spawnPoints, float delayStep, int pointCount, Type type);
}

public enum UnitSpawnAreaOrder
{
    AtOnce = 10,
    LeftToRight = 20,
    BottomToTop = 30,
    EdgeToCenter = 40,
    Random = 50
}