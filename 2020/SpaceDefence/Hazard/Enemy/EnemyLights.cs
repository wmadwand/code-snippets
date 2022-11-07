using DG.Tweening;
using UnityEngine;

public class EnemyLights : MonoBehaviour
{
    public MeshRenderer[] Meshes => _meshes;

    [SerializeField] private MeshRenderer[] _meshes;
    [SerializeField] private GameObject _collection;
    [SerializeField] private Material _lightMaterial;
    [SerializeField] private MeshRenderer _cabinMesh;
    [SerializeField] private ParticleSystem _particle;

    private Material _cabinBaseMaterial;
    private Sequence _sequence;

    //-------------------------------------------------

    public Tween Show(float time)
    {
        _sequence = DOTween.Sequence();

        _sequence
            .AppendCallback(() =>
            {
                _collection.SetActive(true);
                _particle.Play();
            })
             .Join(_cabinMesh.material.DOFade(.25f, time))
            ;

        return _sequence;
    }

    public void Hide()
    {
        _collection.SetActive(false);
        _cabinMesh.material.DOFade(1, 0.25f);
        _particle.Stop();
    }

    //-------------------------------------------------

    private void Awake()
    {
        _cabinBaseMaterial = _cabinMesh.material;
    }
}