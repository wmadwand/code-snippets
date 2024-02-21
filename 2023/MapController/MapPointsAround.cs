using Cysharp.Threading.Tasks;
using GoMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Project.Gameplay.Map
{
    public interface IMapPointsAround
    {
        UniTask<List<Vector2>> Allocate(int count, IEnumerable<Bounds> boundsCollection, CancellationToken cancellationToken, GOTile tile, Action<List<Vector2>> callback);
        void Remove(Vector2 point);
        List<Vector2> PlacedPoints { get; }
    }

    [Serializable]
    public class MapPointsAround : IMapPointsAround
    {
        public List<Vector2> PlacedPoints => _placedPoints;

        private readonly List<Vector2> _placedPoints = new List<Vector2>();
        private readonly Transform _center;
        private readonly float _radius = 500;
        private readonly float _distanceBetweenPoints = 150;
        private readonly float _distanceFromPointToCenter = 15;
        private readonly int _placePointAttemptsCount = 100;

        //--------------------------------------------------------------

        public MapPointsAround(Transform center, float radius, float distanceBetween, float distanceToPlayer, int placePointAttemptsCount)
        {
            _center = center;
            _distanceBetweenPoints = distanceBetween;
            _radius = radius;
            _distanceFromPointToCenter = distanceToPlayer;
            _placePointAttemptsCount = placePointAttemptsCount;
        }

        //TODO: DRY
        public async UniTask<List<Vector2>> Allocate(int count, IEnumerable<Bounds> boundsCollection, CancellationToken cancellationToken, GOTile tile, Action<List<Vector2>> callback)
        {
            // This is for Debug Repositioning
            //if (awayFromPoints == null)
            //{
            //    _placedPoints.Clear();
            //    awayFromPoints ??= new List<Vector2>();
            //}

            var center = new Vector2(_center.position.x, _center.position.z);
            var center3D = _center.position;
            var chosenAreas = boundsCollection.Where(bounds => IsPointInDonut(bounds.center, center3D)).ToList();
            if (chosenAreas.Count < 1)
            {
                return null;
            }

            var allocateResult = new List<Vector2>();
            var attempts = 0;
            //count -= _placedPoints.Count;
            for (int i = 0; i < count; i++)
            {
                Vector3 resultPoint = default;
                while (attempts < _placePointAttemptsCount)
                {
                    var randIndex = UnityEngine.Random.Range(0, chosenAreas.Count);
                    var randomBounds = chosenAreas[randIndex];
                    if (randomBounds == default)
                    {
                        attempts++;
                        continue;
                    }

                    var point3 = GetRandomPointInBounds(randomBounds);
                    if (point3 == default)
                    {
                        attempts++;
                        continue;
                    }

                    var point = new Vector2(point3.x, point3.z);
                    if (IsPointFarEnough(point, center, _placedPoints))
                    {
                        resultPoint = point;
                        break;
                    }

                    attempts++;

                    await UniTask.Yield(cancellationToken: cancellationToken);
                }

                if (resultPoint != default)
                {
                    _placedPoints.Add(resultPoint);
                    allocateResult.Add(resultPoint);
                }

                await UniTask.Yield(cancellationToken: cancellationToken);
            }

            callback?.Invoke(allocateResult);

            return allocateResult;
        }

        public void Remove(Vector2 point)
        {
            _placedPoints.Remove(point);
        }

        //--------------------------------------------------------------

        private bool IsPointFarEnough(Vector2 point, Vector2 center, IEnumerable<Vector2> awayFromPointsResult)
        {
            var isPointFarEnough = (point - center).sqrMagnitude >= _distanceFromPointToCenter * _distanceFromPointToCenter;
            if (isPointFarEnough)
            {
                foreach (var pp in awayFromPointsResult)
                {
                    isPointFarEnough = (point - pp).sqrMagnitude >= _distanceBetweenPoints * _distanceBetweenPoints;
                    if (!isPointFarEnough)
                    {
                        break;
                    }
                }
            }

            return isPointFarEnough;
        }

        private Vector3 GetRandomPointInBounds(Bounds value)
        {
            var rndX = UnityEngine.Random.Range(value.min.x, value.max.x);
            var rndZ = UnityEngine.Random.Range(value.max.z, value.max.z);
            var rndPoint = new Vector3(rndX, 3, rndZ);
            Ray ray = new Ray(rndPoint, Vector3.down);

            var raycast = Physics.Raycast(ray, out RaycastHit raycastHit, 100);
            if (raycast)
            {
                var feature = raycastHit.transform.GetComponentInParent<GOFeatureBehaviour>();
                if (feature)
                {
                    var kind = feature.goFeature.kind;
                    if (Game.Config.Meta.TokenMapArea.Contains(kind))
                    {
                        return rndPoint;
                    }
                }
            }

            return default;
        }

        private bool IsPointInCircle(Vector3 point, Vector3 center)
        {
            return (point - center).sqrMagnitude <= _radius * _radius;
        }

        private bool IsPointInDonut(Vector3 point, Vector3 center)
        {
            return IsPointInCircle(point, center)
                && (point - center).sqrMagnitude >= _distanceFromPointToCenter * _distanceFromPointToCenter;
        }
    }
}