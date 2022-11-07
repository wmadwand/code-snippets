namespace MapConstructor
{
    public interface IMapEditorData
    {
        string ID { get; }
        void InitByJson(JSONObject jsonObj);
    }

    public interface IMapEditorItem<T1>
    {
       void InitByItem(T1 item);
    }
}