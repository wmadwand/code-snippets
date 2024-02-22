using System.Collections.Generic;

public class UnitSpawnAreaOrderFactory
{
    private readonly Dictionary<UnitSpawnAreaOrder, IUnitSpawnAreaOrder> Collection = new Dictionary<UnitSpawnAreaOrder, IUnitSpawnAreaOrder>();

    public IUnitSpawnAreaOrder Get(UnitSpawnAreaOrder order)
    {
        if (Collection.ContainsKey(order))
        {
            return Collection[order];
        }

        switch (order)
        {
            case UnitSpawnAreaOrder.AtOnce: default: { Collection[order] = new UnitSpawnAreaOrderAtOnce(); } break;
            case UnitSpawnAreaOrder.LeftToRight: { Collection[order] = new UnitSpawnAreaOrderLeftToRight(); } break;
            case UnitSpawnAreaOrder.BottomToTop: { Collection[order] = new UnitSpawnAreaOrderBottomToTop(); } break;
            case UnitSpawnAreaOrder.EdgeToCenter: { Collection[order] = new UnitSpawnAreaOrderEdgeToCenter(); } break;
            case UnitSpawnAreaOrder.Random: { Collection[order] = new UnitSpawnAreaOrderRandom(); } break;
        }

        return Collection[order];
    }
}