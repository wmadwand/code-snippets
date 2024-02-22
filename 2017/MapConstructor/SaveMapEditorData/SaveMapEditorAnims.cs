namespace MapConstructor
{
    public class SaveMapEditorAnims : MapEditorDataIO<SaveMapEditorAnims, MapAnimEditorData>
    {
        protected override void Get(MapLocationSettings currMapLocSettings)
        {
            MapAnimSettings[] mapAnimSettings = currMapLocSettings.Anims.GetComponentsInChildren<MapAnimSettings>();

            foreach (MapAnimSettings item in mapAnimSettings)
            {
                MapAnimEditorData data = new MapAnimEditorData(item);
                dict[item.AnimData.ID] = data;
            }
        }
    }
}
