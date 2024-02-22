using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapConstructor
{
    public class MapLocationSettings : MonoBehaviour
    {
        [SerializeField]
        LevelsMapEditorData locationSettings;
        public LevelsMapEditorData LocationSettings
        {
            get { return locationSettings; }
        }

        [SerializeField]
        GameObject levelMarkers;
        public GameObject LevelMarkers
        {
            get { return levelMarkers; }
        }

        [SerializeField]
        GameObject bonusLevelMarkers;
        public GameObject BonusLevelMarkers
        {
            get { return bonusLevelMarkers; }
        }

        [SerializeField]
        GameObject anims;
        public GameObject Anims
        {
            get { return anims; }
        }
    }
}
