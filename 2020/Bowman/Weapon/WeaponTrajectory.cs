using UnityEngine;

namespace MiniGames.Games.Bowman.Shooting
{
    public class WeaponTrajectory
    {
        private readonly WeaponHelper _helper;
        private readonly LineRenderer _lineRenderer;
        private readonly BowmanSettings _gameSettings;
        private readonly int _segmentCount = 15;

        private readonly Vector2[] _segments;
        //---------------------------------------------------

        public WeaponTrajectory(LineRenderer lineRenderer, WeaponHelper helper, BowmanSettings gameSettings)
        {
            _lineRenderer = lineRenderer;
            _helper = helper;
            _gameSettings = gameSettings;
            _segments = new Vector2[_segmentCount];
        }

        public void Display(Vector3 direction, float distance)
        {
            Draw(direction, distance);
            Animate();
        }

        //---------------------------------------------------

        private void Draw(Vector3 direction, float distance)
        {
            _lineRenderer.enabled = true;

            Vector2[] segments = _segments;
            segments[0] = _helper.ProjectileShotPoint.position;

            Vector2 segVelocity = direction * _gameSettings.projectileSpeed * distance;



            var resCount = _segmentCount;

            for (int i = 1; i < _segmentCount; i++)
            {
                float time2 = i * Time.fixedDeltaTime * 5;
                segments[i] = segments[0] + segVelocity * time2 + Physics2D.gravity * (0.5f * Mathf.Pow(time2, 2));

                var startPoint = segments[i - 1];
                var endPoint = segments[i];
                var hit = Physics2D.Linecast(startPoint, endPoint);

                Debug.DrawLine(startPoint, endPoint);

                if (hit.collider != null && hit.collider.GetComponent<Target>())
                {
                    segments[i] = hit.point;
                    resCount = i + 1;
                    break;
                }
            }

            _lineRenderer.positionCount = resCount;

            for (int i = 0; i < resCount; i++)
            {
                _lineRenderer.SetPosition(i, segments[i]);
            }
        }

        private void Animate()
        {
            var xPos = 1 - Time.time * 0.5f;
            _lineRenderer.material.SetTextureOffset("_MainTex", new Vector2(xPos, 0));
        }
    }
}