using DG.Tweening;
using MiniGames.Games.SpaceDefence.Core;
using MiniGames.Games.SpaceDefence.Data;
using System;
using UnityEngine;
using Utils;

namespace MiniGames.Games.SpaceDefence.Hazard.Enemy
{
    public class EnemyView : MonoBehaviour
    {
        [SerializeField] private Material[] _materials;
        [SerializeField] private MeshRenderer[] _meshRenderers;
        [SerializeField] private Vector3 _laserOffset;
        [SerializeField] private EnemyLights _enemyLights;

        private EnemyModel _enemyModel;
        private LineRenderer _lineRenderer;
        private bool _isShowLaserBeam;
        private Ray _ray;

        //-------------------------------------------------

        public void ShowLaserBeam(bool value)
        {
            _isShowLaserBeam = value;
        }

        public void Init(EnemyModel enemyModel)
        {
            _enemyModel = enemyModel;
            _lineRenderer.enabled = false;
            _enemyLights.Hide();

            ChangeSkin();
        }

        public Tween ShowPrewarmLights()
        {
            return _enemyLights.Show(_enemyModel.prewarmTime.Random());
        }

        public void HidePrewarmLights()
        {
            _enemyLights.Hide();
        }

        //-------------------------------------------------

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
        }

        private void Update()
        {
            LaserBeamUpdate();
        }

        private void LaserBeamUpdate()
        {
            if (!_isShowLaserBeam)
            {
                _lineRenderer.enabled = false;
                return;
            }

            _lineRenderer.enabled = true;
            _lineRenderer.SetPosition(0, transform.position);

            _ray = new Ray(transform.position, -transform.up);

            if (Physics.Raycast(_ray, out RaycastHit hit, Mathf.Infinity, (int)Layer.Characters))
            {
                if (hit.collider.GetComponent<Player.Player>() || hit.collider.GetComponent<Shield.Shield>())
                {
                    _lineRenderer.SetPosition(1, hit.point + _laserOffset);
                }
                else
                {
                    _lineRenderer.SetPosition(1, -transform.up * 5000);
                }
            }
            else
            {
                _lineRenderer.SetPosition(1, -transform.up * 5000);
            }
        }

        private void ChangeSkin()
        {
            _meshRenderers[0].material = _materials.GetRandomItem();
        }
    }
}