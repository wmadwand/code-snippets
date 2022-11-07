using System;
using UnityEngine;

namespace MiniGames.Games.SpaceDefence.Hazard.Projectile
{
    public interface IProjectile
    {
        bool IsCollectible { get; }
        bool IsReflected { get; }

        void Init(Vector3 direction, float speed, Action<ProjectileBase, bool> callback);
        void SetStartPosition(Vector3 value);
        void Destroy();
        void SendOnDestroy(bool byPlayer);
        void Reflect(float speed);
        void SetReflected();
        void Stop();
        void Explosion();
    }
}