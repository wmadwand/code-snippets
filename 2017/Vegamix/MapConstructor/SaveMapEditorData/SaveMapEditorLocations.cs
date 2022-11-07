using System.Collections.Generic;

namespace MapConstructor
{
    public class SaveMapEditorLocations : MapEditorDataIO<SaveMapEditorLocations, LevelsMapEditorData>
    {
        protected override void Get(MapLocationSettings currMapLocSettings)
        {
            LevelsMapEditorData levelsMap = currMapLocSettings.LocationSettings;
            List<MapLevelMarkerEditorData> levelMarkersList = GetList<MapLevelMarker, MapLevelMarkerEditorData>(currMapLocSettings);
            List<MapBonusLevelMarkerData> bonusLevelMarkersList = GetList<MapLevelMarker, MapBonusLevelMarkerData>(currMapLocSettings);
            levelsMap.SetLevels(levelMarkersList, bonusLevelMarkersList);

            dict[levelsMap.ID] = levelsMap;
        }

        private List<T2> GetList<T1, T2>(MapLocationSettings currMapLocSettings) where T1 : MapLevelMarker where T2 : MapLevelMarkerEditorData, new()
        {
            T1[] items = currMapLocSettings.LevelMarkers.GetComponentsInChildren<T1>();
            List<T2> list = new List<T2>();

            foreach (T1 item in items)
            {
                T2 t2 = new T2();
                t2.InitByItem(item);
                list.Add(t2);
            }

            return list;
        }
    }
}
