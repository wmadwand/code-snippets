using System;
using System.Collections.Generic;
using UnityEngine;

namespace MapConstructor
{
    [Serializable]
    public class MapLevelMarkerEditorData : MapEditorItem, IMapEditorItem<MapLevelMarker>
    {
        #region Properties
        public string type;
        [SerializeField]
        Vector2 friendsBeaconGroupPos; 
        #endregion

        #region Constructors
        public MapLevelMarkerEditorData() { }

        public MapLevelMarkerEditorData(string _name, string _type, Vector2 _position, Vector2 _friendsBeaconGroupPos)
        {
            id = _name;
            type = _type;
            position = _position;
            friendsBeaconGroupPos = _friendsBeaconGroupPos;
        }

        public MapLevelMarkerEditorData(string _name, MapLevelType _type, Vector2 _position, Vector2 _friendsBeaconGroupPos)
        {
            id = _name;
            type = _type.ToString();
            position = _position;
            friendsBeaconGroupPos = _friendsBeaconGroupPos;
        }
        #endregion

        #region Init methods
        public virtual void InitByItem(MapLevelMarker marker)
        {
            id = marker.name;
            type = marker.Type.ToString();
            position = marker.GetComponent<RectTransform>().anchoredPosition;
            friendsBeaconGroupPos = marker.FriendsBeaconGroup;
        } 
        #endregion
    }
}