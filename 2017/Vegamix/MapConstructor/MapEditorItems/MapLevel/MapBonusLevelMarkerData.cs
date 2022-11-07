using System;
using System.Collections.Generic;
using UnityEngine;

namespace MapConstructor
{
    [Serializable]
    public class MapBonusLevelMarkerData : MapLevelMarkerEditorData
    {
        public MapBonusLevelType subType;
        public string after;

        public MapBonusLevelMarkerData() { }

        public MapBonusLevelMarkerData(string _name, string _type, Vector2 _pos, Vector2 _fBeaconPos, string _after) : base(_name, _type, _pos, _fBeaconPos)
        {
            after = _after;
        }

        public override void InitByItem(MapLevelMarker marker)
        {
            id = marker.name;
            type = marker.Type.ToString();
            subType = marker.SubType;
            after = marker.After;
            position = marker.GetComponent<RectTransform>().anchoredPosition;
        }
    }
}

