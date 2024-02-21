using UnityEngine;

namespace Project.IO.Camera
{
    public abstract class PlayerCameraInputBase : MonoBehaviour
    {
        public abstract float ZoomValue { get; }
        public abstract float RotationValue { get; }
        protected abstract void UpdateHandler();

        private void Update()
        {
            UpdateHandler();
        }
    }
}