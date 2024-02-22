using UnityEngine;

namespace MapConstructor
{
    public abstract class MapEditorItem
    {
        [SerializeField]
        protected string id;
        [SerializeField]
        protected Vector2 position;
    }
}