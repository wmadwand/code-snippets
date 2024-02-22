﻿using UnityEngine;

namespace PathCreationEditor
{
    public enum PathSpace { XYZ, XY, XZ };

    public static class MouseUtility
    {
        /// <summary>
		/// Determines mouse position in world. If PathSpace is xy/xz, the position will be locked to that plane.
		/// If PathSpace is xyz, then depthFor3DSpace will be used as distance from scene camera.
		/// </summary>
        public static Vector3 GetMouseWorldPosition(PathSpace space, float depthFor3DSpace = 10)
        {
            Ray mouseRay = new Ray(Vector3.zero, Vector3.zero);
#if UNITY_EDITOR
            mouseRay = UnityEditor.HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
#endif
            Vector3 worldMouse = mouseRay.GetPoint(depthFor3DSpace);

            // Mouse can only move on XY plane
            if (space == PathSpace.XY)
            {
                float zDir = mouseRay.direction.z;
                if (zDir != 0)
                {
                    float dstToXYPlane = Mathf.Abs(mouseRay.origin.z / zDir);
                    worldMouse = mouseRay.GetPoint(dstToXYPlane);
                }
            }
            // Mouse can only move on XZ plane 
            else if (space == PathSpace.XZ)
            {
                float yDir = mouseRay.direction.y;
                if (yDir != 0)
                {
                    float dstToXZPlane = Mathf.Abs(mouseRay.origin.y / yDir);
                    worldMouse = mouseRay.GetPoint(dstToXZPlane);
                }
            }

            return worldMouse;
        }

    }
}