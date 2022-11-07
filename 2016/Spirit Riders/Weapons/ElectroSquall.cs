/// <summary>
/// Programmed by WMADWAND
/// </summary>

using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

[AddComponentMenu("JNC/Weapons/Electro Squall")]
public class ElectroSquall : NetworkBehaviour
{
    CircleCollider2D _circleCollider2D;
    LayerMask _layerMaskAttack;
    WeaponSetings _weaponSettings;

    SimpleAnimator _simpleAnimator;
    private Sprite[] allSprites;

    int currentPhase = 1;
    float _time;

    Vector2 overlapAreaAllPointA, overlapAreaAllPointB;

    public void SetWeaponSetings(WeaponSetings weaponSetings)
    {
        _weaponSettings = weaponSetings;
    }

    void SetupLayerMask()
    {
        if (_weaponSettings._maskAttack.Count <= 0)
        {
            return;
        }

        foreach (int _maskID in _weaponSettings._maskAttack)
        {
            _layerMaskAttack += 1 << _maskID;
        }
    }

    void Awake()
    {
        _circleCollider2D = GetComponent<CircleCollider2D>();
    }

    void Start()
    {
        transform.localPosition = new Vector2(-4.1f, -1.44f);

        if (isServer)
        {
            SetupLayerMask();
            StartCoroutine(MakeDamage());
        }

        if (isClient)
        {
            //_simpleAnimator = GetComponent<SimpleAnimator>();
            //_simpleAnimator.AnimationTimestep = _simpleAnimator.AnimationTimestep / _weaponSettings._speed;
            //allSprites = new Sprite[_simpleAnimator.Sprites.Count];
            //_simpleAnimator.Sprites.CopyTo(allSprites);

            //SetPhase(currentPhase);
        }
    }

    void Update()
    {
        if (_time < 2)
        {
            overlapAreaAllPointA = new Vector2(transform.position.x + 2, transform.position.y + 2.5f);
            overlapAreaAllPointB = new Vector2(Mathf.Lerp(overlapAreaAllPointA.x, overlapAreaAllPointA.x + 10, _time * 1.2f), overlapAreaAllPointA.y - 2f);

            UnityEngine.Debug.DrawLine(overlapAreaAllPointA, overlapAreaAllPointB, Color.magenta, 4);
        }

        _time += Time.deltaTime;

        switch (currentPhase)
        {
            case 1:
                {
                    if (_time >= _weaponSettings.floatProperty2)
                    {
                        ChangePhase();
                    }
                }; break;

            case 2:
                {
                    if (_time >= _weaponSettings.floatProperty3)
                    {
                        ChangePhase();
                    }
                }; break;

            case 3:
                {
                    if (_time >= _weaponSettings.floatProperty4)
                    {
                        currentPhase++;
                        //_simpleAnimator.DestroyOnEnd = true;
                        Destroy(gameObject);
                    }
                }; break;
        }
    }

    private void ChangePhase()
    {
        //_time = 0;
        currentPhase++;
        //SetPhase(currentPhase);
    }
    
    private void SetPhase(int num)
    {
        _simpleAnimator.Sprites.Clear();

        switch (num)
        {
            case 1:
                {
                    _simpleAnimator.Sprites.Add(allSprites[0]);
                    _simpleAnimator.Sprites.Add(allSprites[1]);
                    _simpleAnimator.Sprites.Add(allSprites[2]);
                }; break;

            case 2:
                {
                    _simpleAnimator.Sprites.Add(allSprites[3]);
                    _simpleAnimator.Sprites.Add(allSprites[4]);
                    _simpleAnimator.Sprites.Add(allSprites[5]);

                    _simpleAnimator.Loop = true;
                }; break;

            case 3:
                {
                    _simpleAnimator.Sprites.Add(allSprites[2]);
                    _simpleAnimator.Sprites.Add(allSprites[1]);
                    _simpleAnimator.Sprites.Add(allSprites[0]);

                    _simpleAnimator.Loop = false;
                }; break;
        }
    }

    IEnumerator MakeDamage()
    {
        while (/*currentPhase < 4*/_time < 2)
        {
            yield return new WaitForSeconds(_weaponSettings.floatProperty1);

            //Vector2 _center = new Vector2(transform.position.x + _circleCollider2D.offset.x, transform.position.y + _circleCollider2D.offset.y);
            //Collider2D[] _colliders2DList = Physics2D.OverlapCircleAll(_center, _circleCollider2D.radius, _layerMaskAttack);
            //Debug.DrawLine(_center, new Vector2(_center.x + _circleCollider2D.radius, _center.y), Color.red, 4);

            Collider2D[] _colliders2DList = Physics2D.OverlapAreaAll(overlapAreaAllPointA, overlapAreaAllPointB, _layerMaskAttack);

            if (_colliders2DList.Length > 0)
            {
                foreach (Collider2D _collider2D in _colliders2DList)
                {
                    GameObject _gameObject = _collider2D.gameObject;

                    if (_gameObject.CompareTag("Player") || _gameObject.CompareTag("Bot"))
                    {
                        if (_gameObject == _weaponSettings.owner)
                        {
                            continue;
                        }

                        Vector3 _hitPoint = GetHitPoint(_gameObject);
                        _gameObject.GetComponent<ClientOnlineController>().TakeWeaponDamage(_weaponSettings.owner, _weaponSettings._weapon, _weaponSettings._damage, _hitPoint, _weaponSettings._weaponType);
                        PlayExplosion(_hitPoint);
                        continue;
                    }
                    else if (Wall.IsWall(_gameObject))
                    {
                        Wall _wall = _gameObject.GetComponent<Wall>();
                        if (_wall.CanWeaponDamage())
                        {
                            Vector3 _hitPoint = GetHitPoint(_gameObject);
                            _wall.TakeWeaponDamage(WeaponSetings.Weapon.ElectroSquall, _weaponSettings._damage, _hitPoint, _weaponSettings.owner);
                            PlayExplosion(_hitPoint);
                        }
                    }
                    else if (MonsterController.IsMonster(_gameObject))
                    {
                        MonsterController _monster = _gameObject.GetComponent<MonsterController>();
                        Vector3 _hitPoint = GetHitPoint(_gameObject);
                        _monster.TakeWeaponDamage(_weaponSettings.owner, _weaponSettings._weapon, _weaponSettings._damage, _hitPoint);
                    }

                    if (_weaponSettings.destroyMask.Contains(_gameObject.layer) && Extension.IsDestroyable(_gameObject) /*&& _gameObject.GetComponent<Wall_repulsion>() == null*/)
                    {
                        PlayExplosion(GetHitPoint(_gameObject));
                        Destroy(_gameObject);
                    }
                }
            }
        }
    }

    static Vector3 GetHitPoint(GameObject target)
    {
        Collider2D _collider2D = target.GetComponent<Collider2D>();

        if (_collider2D != null)
        {
            return _collider2D.bounds.center;
        }

        return target.transform.position;
    }

    private void PlayExplosion(Vector2 explPos)
    {
        float _explosionRadius = 10;
        GameObject _explosionEffect = (GameObject)Instantiate(_weaponSettings.effectPrefab, explPos, Quaternion.identity);
        _explosionEffect.GetComponent<ExplEffect>().SetupEffect(_explosionRadius, true, 4);
        //_explosionEffect.GetComponent<ParticleSystem>().GetComponent<Renderer>().sortingOrder = 20;

        NetworkServer.Spawn(_explosionEffect);
    }
}