using Cysharp.Threading.Tasks;
using GoShared;
using Project.Analytics;
using Project.Gameplay.Data;
using Project.Gameplay.Data.Campaign;
using Project.Gameplay.Map;
using Project.Gameplay.MainPlayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Project.Controller
{
    public interface ITokenViewSpawner
    {
        List<MapToken> Collection { get; }

        UniTask Spawn(int count, CancellationToken cancellationToken, bool overlap = false, bool isNeedExtraView = false, params CampaignAgent[] agents);
    }

    public interface ITokenViewSpawnerDebug
    {
        UniTaskVoid DebugRebuildPositions();
    }

    public class TokenViewSpawner : MonoBehaviour, ITokenViewSpawner, ITokenViewSpawnerDebug
    {
        public static event Action<List<MapToken>> OnTokensUpdated;
        public List<MapToken> Collection => _collection;

        [SerializeField] private MapController _map;
        [SerializeField] private TokenViewFactory _factory;
        private List<MapToken> _collection;
        private TokenViewSpawnerAnalytics _analytics;

        //--------------------------------------------------------------

        public async UniTaskVoid DebugRebuildPositions()
        {
            //TODO: use #if or move to another class

            //CancellationTokenSource cancellationToken = new CancellationTokenSource();
            //var allRandomPos = await _map.GeneratePoints(_collection.Count, cancellationToken.Token, null, true);

            //for (var i = 0; i < _collection.Count; i++)
            //{
            //    if (i >= allRandomPos.Count)
            //    {
            //        break;
            //    }

            //    var coordinates = Coordinates.convertVectorToCoordinates(new Vector3(allRandomPos[i].x, 0, allRandomPos[i].y));
            //    _collection[i].view.DebugRebuildPositions(coordinates);
            //}
        }

        public async UniTask Spawn(int count, CancellationToken cancellationToken, bool overlap = false, bool isNeedExtraView = false, params CampaignAgent[] agents)
        {
            if (agents.Length < 1) { return; }

            await _map.GeneratePoints(count, cancellationToken, overlap: overlap, callback: (points) =>
            {
                UniTask.Void(async () =>
                {
                    if (points == null || points?.Count < 1) { return; }

                    foreach (var point in points)
                    {
                        var index = UnityEngine.Random.Range(0, agents.Length);
                        var agent = agents[index];

                        if (isNeedExtraView && agent.ActiveTokens.Count < 1) { agent.AddExtraTokens(1); }

                        CreateView(agent, point);

                        await UniTask.Yield(cancellationToken: cancellationToken);
                    }

                    OnTokensUpdated?.Invoke(_collection);
                });
            });
        }

        //--------------------------------------------------------------

        private void Awake()
        {
            _collection = new List<MapToken>();
            _analytics = new TokenViewSpawnerAnalytics();

            CampaignAgent.OnRewardEarn += OnRewardEarn;
            GameController.OnGameStart += OnGameStart;
        }

        private void OnDestroy()
        {
            CampaignAgent.OnRewardEarn -= OnRewardEarn;
            GameController.OnGameStart -= OnGameStart;

            for (int i = 0; i < _collection.Count; i++)
            {
                var item = _collection.ElementAt(i);
                if (item != null && item.view != null)
                {
                    Destroy(item.view.gameObject);
                }
            }
        }

        private void CreateView(CampaignAgent campaignAgent, Vector2 point)
        {
            var activeTokens = campaignAgent.ActiveTokens;

            var i = 0;
            if (i > activeTokens.Count - 1) { return; }

            var token = activeTokens[i];
            token.OnTokenCollected += OnTokenCollected;
            token.coordinates = Coordinates.convertVectorToCoordinates(new Vector3(point.x, 0, point.y));
            token.SetWorldPosition(point);
            var tokenView = _factory.Get();
            tokenView.Init(token, campaignAgent);

            _collection.Add(new MapToken(token, tokenView, campaignAgent, point));
        }

        private void OnGameStart(TimeSpan gameLoadedDuringTime)
        {
            var playerCoord = Coordinates.convertVectorToCoordinates(Player.Instance.transform.position);
            _analytics.UpdatePreviousTokenCoordinates(playerCoord);
            _analytics.UpdateTimerForTokens();
        }

        private void OnRewardEarn(CampaignAgent campaignAgent)
        {
            var tokensOnMap = _collection.FindAll(item => item.agent == campaignAgent);
            for (int i = 0; i < tokensOnMap.Count; i++)
            {
                var mtd = tokensOnMap.ElementAt(i);
                mtd.Collect(true);
            }
        }

        private void OnTokenCollected(Token token, bool silently)
        {
            var mtd = _collection.Find(item => item.token == token);

            if (!silently)
            {
                _analytics.CollectToken(mtd);
                _analytics.UpdatePreviousTokenCoordinates(token.coordinates);
            }

            _map.RemovePoint(mtd.point);
            _collection.Remove(mtd);
            _factory.Return(mtd.view);
            mtd.view = null;
        }
    }
}