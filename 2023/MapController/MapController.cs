using Cysharp.Threading.Tasks;
using GoMap;
using LocationManagerEnums;
using Project.Gameplay.MainPlayer;
using Project.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Project.Gameplay.Map
{
    public interface IMapController
    {
        UniTask GeneratePoints(int count, CancellationToken cancellationToken, Action<List<Vector2>> callback, bool overlap = false);
        UniTask IsInitialized();
        void RemovePoint(Vector2 value);
    }

    public interface IMapControllerDebug
    {
        void SetLocationManagerMode(MotionMode motionMode);
    }

    public class MapController : MonoBehaviour, IMapController, IMapControllerDebug
    {
        public List<Vector2> PlacedPoints => _mapPointsAround.PlacedPoints;
        [SerializeField] private GameObject _fog;

        [SerializeField] private GOMapWrapper _goMapWrapper;
        private IMapPointsAround _mapPointsAround;
        private Dictionary<Vector2, Bounds[]> _tileBoundsCache = new Dictionary<Vector2, Bounds[]>();

        //--------------------------------------------------------------

        public async UniTask IsInitialized()
        {
            await _goMapWrapper.IsInitialized();
        }

        public async UniTask IsPreInitialized()
        {
            await _goMapWrapper.IsPreInitialized();

        }

        public void SetLocationManagerMode(MotionMode motionMode)
        {
            _goMapWrapper.SetLocationManagerMode(motionMode);
        }

        public async UniTask GeneratePoints(int count, CancellationToken cancellationToken, Action<List<Vector2>> callback, bool overlap = false)
        {
            var nearestTiles = _goMapWrapper.GetNearestTiles().ToList();
            var processedTiles = new List<GOTile>();
            List<UniTask> tasks = new List<UniTask>();
            while (processedTiles.Count < nearestTiles.Count)
            {
                var newTile = nearestTiles.FirstOrDefault(tile => tile.goTile.status == GOTileObj.GOTileStatus.Done && !processedTiles.Contains(tile));
                if (newTile)
                {
                    var bounds = GetAreaBoundsForTile(newTile);
                    var allocateTask = _mapPointsAround.Allocate(count, bounds, cancellationToken, null, callback);
                    tasks.Add(allocateTask);
                    processedTiles.Add(newTile);
                }

                await UniTask.Yield(cancellationToken: cancellationToken);
            }

            await UniTask.WhenAll(tasks);
        }

        public void RemovePoint(Vector2 value)
        {
            _mapPointsAround.Remove(value);
        }

        //--------------------------------------------------------------

        private void Start()
        {
            _mapPointsAround = new MapPointsAround(Player.Instance.transform,
                                                   Game.Config.Meta.PlayerZoneRadius,
                                                   Game.Config.Meta.MinTokensDistance,
                                                   Game.Config.Meta.TokenInitialDistanceToPlayer,
                                                   Game.Config.App.Map.PlacePointAttemptsCount);

            _fog.SetActive(Game.Config.App.Map.showFog);
        }

        private IEnumerable<Bounds> GetAreaBoundsForTile(GOTile tile)
        {
            if (_tileBoundsCache.TryGetValue(tile.goTile.tileCoordinates, out Bounds[] bounds))
            {
                return bounds;
            }

            var mapAreas = tile.GetComponentsInChildren<GOFeatureBehaviour>();
            var goodAreas = mapAreas.Where(ma => Game.Config.Meta.TokenMapArea.Any(ama => ama == ma.goFeature.kind)).ToArray();
            var boundsArray = new Bounds[goodAreas.Length];
            for (int i = 0; i < goodAreas.Length; i++)
            {
                boundsArray[i] = ProjectExtensions.GetBounds(goodAreas[i].goFeature.convertedGeometry);
            }

            _tileBoundsCache[tile.goTile.tileCoordinates] = boundsArray;

            return boundsArray;
        }
    }
}