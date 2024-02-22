using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeechTypeConfig : SerializedScriptableObject
{
    public Dictionary<string, TimeConfig> Pauses { get; private set; }
    public TimeConfig DefaultPause { get; private set; }

    public class TimeConfig
    {
        public float Pause;
        public string[] SkipNext;
    }
}
