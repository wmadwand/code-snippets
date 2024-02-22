using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MapConstructor
{
    public static class MapLevelFactory
    {
        public static MapLevelHandler Construct(MapLevelType type, MapLevelMarker mapLvlObj)
        {
            MapLevelHandler handler = null;

            switch (type)
            {
                case MapLevelType.Circle:
                case MapLevelType.Arrow:
                case MapLevelType.Palm:
                    {
                        Debug.Log("cirle");
                        handler = new MapLevelHandler();

                        break;
                    }
                case MapLevelType.Bonus:
                    {
                        Debug.Log("Special");
                        handler = new MapLevelHandlerSpecial();

                        break;
                    }
                default:
                    {
                        Debug.Log("cirle");
                        handler = new MapLevelHandler();

                        break;
                    }
            }

            if (handler != null && mapLvlObj)
            {
                handler.SetMapLvlObj(mapLvlObj);
                mapLvlObj.SetHandler(handler);
            }

            return handler;
        }
    } 
}