using System;
using UnityEngine;

namespace Project.Data
{
    [CreateAssetMenu(fileName = "PlayerCameraSettings", menuName = "Project/PlayerCameraSettings")]
    public class PlayerCameraSettings : ScriptableObject
    {
        [Serializable]
        public struct FloatPair
        {
            public float min;
            public float max;

            public FloatPair(float min, float max)
            {
                this.min = min;
                this.max = max;
            }
        }

        public float rotationSpeed = 45;
        public float zoomSpeed = 2;
        public float zoomHeightRate = 500;
        public AnimationCurve zoomCurve = null;
        public FloatPair zoomClamp = new FloatPair(20, 60);
    }
}