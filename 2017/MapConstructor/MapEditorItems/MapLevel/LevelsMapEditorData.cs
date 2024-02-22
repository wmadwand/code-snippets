using System;
using System.Collections.Generic;
using UnityEngine;

namespace MapConstructor
{
    [Serializable]
    public class LevelsMapEditorData : MapEditorItem, IMapEditorData
    {
        #region Properties
        public string ID { get { return id; } }

        [SerializeField]
        BirdType bird;
        [SerializeField]
        string tutorialHead;
        [SerializeField]
        string navigationIcon;
        [SerializeField]
        List<MapLevelMarkerEditorData> levels;
        [SerializeField]
        List<MapBonusLevelMarkerData> bonusLevels;
        #endregion

        #region Constructors
        public LevelsMapEditorData() { }

        public LevelsMapEditorData(string _id, BirdType _bird, string _tutorialHead, string _navigationIcon, List<MapLevelMarkerEditorData> _levels, List<MapBonusLevelMarkerData> _bonusLevels)
        {
            id = _id;
            bird = _bird;
            tutorialHead = _tutorialHead;
            navigationIcon = _navigationIcon;
            levels = _levels;
        }
        #endregion

        #region Init methods
        public void InitByJson(JSONObject jsonObj)
        {
            id = jsonObj.GetField("id").ToStrFromJson();
            tutorialHead = jsonObj.GetField("tutorialHead").ToStrFromJson();
            bird = (BirdType)(jsonObj.GetField("bird").ToIntFromJson());
            navigationIcon = jsonObj.GetField("navigationIcon").ToStrFromJson();

            if (jsonObj.HasField("levels") && jsonObj.GetField("levels").list.Count > 0)
            {
                levels = new List<MapLevelMarkerEditorData>();

                foreach (JSONObject item in jsonObj.GetField("levels").list)
                {
                    object[] basicFields = GetBasicFields(item);
                    float _fBeaconPosX = item.GetField("friendsBeaconGroupPos").list[0].ToFloatFromJson();
                    float _fBeaconPosY = item.GetField("friendsBeaconGroupPos").list[1].ToFloatFromJson();
                    levels.Add(new MapLevelMarkerEditorData((string)basicFields[0], (string)basicFields[1], new Vector2((float)basicFields[2], (float)basicFields[3]), new Vector2(_fBeaconPosX, _fBeaconPosY)));
                }
            }

            if (jsonObj.HasField("bonusLevels") && jsonObj.GetField("bonusLevels").list.Count > 0)
            {
                bonusLevels = new List<MapBonusLevelMarkerData>();

                foreach (JSONObject item in jsonObj.GetField("bonusLevels").list)
                {
                    object[] basicFields = GetBasicFields(item);
                    string _after = item.GetField("after").ToStrFromJson();
                    bonusLevels.Add(new MapBonusLevelMarkerData((string)basicFields[0], (string)basicFields[1], new Vector2((float)basicFields[2], (float)basicFields[3]), Vector2.zero, _after));
                }
            }
        }

        private object[] GetBasicFields(JSONObject item)
        {
            string _name = item.GetField("id").ToStrFromJson();
            string _type = item.GetField("type").ToStrFromJson();
            float _posX = item.GetField("position").list[0].ToFloatFromJson();
            float _posY = item.GetField("position").list[1].ToFloatFromJson();

            return new object[] { _name, _type, _posX, _posY };
        }

        public void SetLevels(List<MapLevelMarkerEditorData> _leves, List<MapBonusLevelMarkerData> _bonusLeves)
        {
            levels = _leves;
            bonusLevels = _bonusLeves;
        } 
        #endregion
    }
}

