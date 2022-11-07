using System;
using System.Collections.Generic;
using System.Linq;
using MiniGames.Games.SpaceDefence.Core;
using UnityEngine;

using Random = UnityEngine.Random;

namespace MiniGames.Games.SpaceDefence.SpawnSystem.SpawnSystemBuilder
{
    public class SpawnSystemHelper
    {
        private const float XMin = 0.1f;
        private const float XMax = 0.9f;
        private const float YMin = 1.1f;
        private const float YMax = 1.2f;
        private const float ZPos = -8.06f;
        private const float PathStepInit = .085f;

        private Vector3 _startPosition;
        private Vector3 _endPosition;
        private Vector3 _direction;
        private List<Path> _paths;

        //-------------------------------------------------

        public Vector3 GetRandomPoint(PointPosition position)
        {
            var xCoord = Random.Range(XMin, XMax);
            var yCoord = position == PointPosition.Top ? Random.Range(YMin, YMax) : 0;

            return GetWorldPosition(xCoord, yCoord);
        }

        public Path GetRandomPath()
        {
            return _paths.GetRandomItem();
        }

        public void InitializeHazardPaths(float _sphereCastRadius)
        {
            _paths = new List<Path>();

            for (float i = XMin; i < XMax;)
            {
                _startPosition = GetWorldPosition(i, YMin);

                for (float j = XMin; j < XMax;)
                {
                    _endPosition = GetWorldPosition(j, 0);
                    _direction = _endPosition - _startPosition;

                    var sphereCastAll = Physics.SphereCastAll(_startPosition, _sphereCastRadius, _direction, Mathf.Infinity);
                    var enemySpot = sphereCastAll.FirstOrDefault(item => item.collider.GetComponent<EnemySpot>());

                    if (enemySpot.collider == null)
                    {
                        _paths.Add(new Path(_startPosition, _endPosition));

                        DebugExtension.DebugCylinder(_startPosition, _endPosition, Color.green, _sphereCastRadius, 5);
                    }
                    else
                    {
                        DebugExtension.DebugCylinder(_startPosition, _endPosition, Color.red, _sphereCastRadius, 5);
                    }

                    j += PathStepInit;
                }

                i += PathStepInit;
            }

            Debug.Log($"TOTAL COUNT DIRECTIONS: {_paths.Count}");
        }

        //-------------------------------------------------

        private Vector3 GetWorldPosition(float xCoord, float yCoord)
        {
            var point = new Vector2(xCoord, yCoord);
            var plane = new Plane(Vector3.forward, new Vector3(0, 0, ZPos));
            var ray = Camera.main.ViewportPointToRay(point);
            plane.Raycast(ray, out float distance);

            return ray.GetPoint(distance);
        }

        //-------------------------------------------------

        public struct Path
        {
            public Vector3 start;
            public Vector3 end;

            public Vector3 Direction => start - end;

            public Path(Vector3 start, Vector3 end)
            {
                this.start = start;
                this.end = end;
            }
        }

        public enum PointPosition { Top, Bottom }
    }
}