using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapConstructor
{
    public class MapLevelMarkerCollection
    {
        public static MapLevelMarker[] collection;

        public static void Process()
        {
            foreach (MapLevelMarker item in GameObject.FindObjectsOfType<MapLevelMarker>())
            {
                MapLevelFactory.Construct(item.Type, item);
            }
        }

        public static MapLevelMarker[] Get()
        {
            return collection;
        }
    }
}