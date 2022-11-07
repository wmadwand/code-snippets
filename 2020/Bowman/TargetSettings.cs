using System;
using UnityEngine;

[CreateAssetMenu(fileName = "TargetSettings", menuName = "MiniGames/Bowman/TargetSettings")]
public class TargetSettings : ScriptableObject
{
    public Harmonic[] harmonics;
}

[Serializable]
public struct Harmonic
{
    public float phase;
    public float period;
    public float amplitude;
}