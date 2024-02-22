using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace MapConstructor
{
    [Serializable]
    public class MapAnimEditorData : MapEditorItem, IMapEditorData, IMapEditorItem<MapAnimSettings>
    {
        #region Properties
        public string ID { get { return id; } }

        [SerializeField]
        GameObject go;
        public GameObject GO { get { return go; } }

        [SerializeField]
        int repeat;
        [SerializeField]
        int mass;
        [SerializeField]
        int fps;
        [SerializeField]
        bool onClick;
        [SerializeField]
        bool cover;
        [SerializeField]
        bool randomize;
        [SerializeField]
        bool xframes;
        [SerializeField]
        int delay;

        [SerializeField]
        List<MapAnimEditorData> subAnims;
        public List<MapAnimEditorData> SubAnims { get { return subAnims; } }
        #endregion

        #region Constructors
        public MapAnimEditorData() { }

        public MapAnimEditorData(MapAnimSettings marker)
        {
            InitByItem(marker);
        }
        #endregion

        #region Init methods
        public void InitByJson(JSONObject jsonObj)
        {
            id = jsonObj.GetField("id").ToStrFromJson();
            fps = jsonObj.GetField("fps").ToIntFromJson();
            repeat = jsonObj.GetField("repeat").ToIntFromJson();
            mass = (int)jsonObj.GetField("mass").ToFloatFromJson();
            randomize = jsonObj.HasField("randomize") ? jsonObj.GetField("randomize").ToBoolFromJson() : false;
            onClick = jsonObj.HasField("onClick") ? jsonObj.GetField("onClick").ToBoolFromJson() : false;
            xframes = jsonObj.HasField("xframes") ? jsonObj.GetField("xframes").ToBoolFromJson() : false;
            delay = jsonObj.HasField("delay") ? jsonObj.GetField("delay").ToIntFromJson() : 0;
            cover = jsonObj.HasField("cover") ? jsonObj.GetField("cover").ToBoolFromJson() : false;

            var _subAnims = jsonObj.GetField("subAnims").list;
            if (_subAnims.Count > 0)
            {
                subAnims = new List<MapAnimEditorData>();
                foreach (var subAnimJson in _subAnims)
                {
                    MapAnimEditorData data = new MapAnimEditorData();
                    data.InitByJson(subAnimJson);
                    subAnims.Add(data);
                }
            }
        }

        public void InitByItem(MapAnimSettings marker)
        {
            id = marker.AnimData.id;
            position = new Vector2(marker.transform.position.x, marker.transform.position.y);
            subAnims = marker.AnimData.subAnims;
        } 
        #endregion
    }
}
