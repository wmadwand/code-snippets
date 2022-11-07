using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapConstructor
{
    public class MapLevelStatistics
    {
        [SerializeField]
        LevelsMapEditorData locationSettings;
        public LevelsMapEditorData LocationSettings
        {
            get { return locationSettings; }
        }
    }
}
