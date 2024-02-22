/// <summary>
/// Programmed by wmadwand
/// </summary>

using UnityEngine;
using UnityEngine.Networking;

[AddComponentMenu("JNC/Weapons/Electro Whirl")]
public class ElectroWhirl : NetworkBehaviour
{
    [SerializeField]
    Transform groundPoint;
    [SerializeField]
    CircleCollider2D _boulderCollider2D;

    WeaponSetings _weaponSettings;
    private LayerMask _layerMaskAttack;
    private LayerMask _layerMaskObstacle;

    Rigidbody2D _rigidbody2D;
    float _velAngle;
    float _time;

    CircleCollider2D _circleCollider2D;
    Collider2D _startPointTr;
    bool isStuck;
    Vector2 zeroMovementV2 = Vector2.zero;

    public void SetWeaponSetings(WeaponSetings weaponSetings)
    {
        _weaponSettings = weaponSetings;
    }

    void ProcessLayersMaskAttack()
    {
        if (_weaponSettings._maskAttack.Count > 0)
        {
            foreach (int element in _weaponSettings._maskAttack)
            {
                _layerMaskAttack += 1 << element;
            }
        }
    }

    [ServerCallback]
    void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _circleCollider2D = GetComponent<CircleCollider2D>();
    }

    void Start()
    {
        transform.localPosition = new Vector2(transform.localPosition.x, -1.40921f);

        //_startPointTr = gameObject.transform;
        _startPointTr = gameObject.GetComponent<Collider2D>();


        if (isClient)
        {
            ProcessLayersMaskAttack();
        }

        if (isServer)
        {
            GroundLayer();
        }
    }

    void Update()
    {
        _time += Time.deltaTime;
    }

    [ServerCallback]
    void FixedUpdate()
    {
        ////CheckForObstacle();
        CheckForPlatformEdge();

        if (!isStuck)
        {
            if (_time < _weaponSettings.floatProperty2)
            {
                ShellMovement();
            }
            else
            {
                _rigidbody2D.velocity = zeroMovementV2;
            }
        }
    }

    private void ShellMovement()
    {
        float _speed = _weaponSettings._speed;

        float _radiusToGround = _boulderCollider2D.radius * transform.localScale.y + _boulderCollider2D.offset.y + 0.5f;
        int _layerMask = 1; // Default in value
        switch (gameObject.layer)
        {
            case 27:
                _layerMask = Extension.LayerGround0; // Ground0 in value
                break;
            case 28:
                _layerMask = Extension.LayerGround1; // Ground-1 in value
                break;
        }
        RaycastHit2D _rayHit2D = Physics2D.Raycast(groundPoint.position, Vector2.down, _radiusToGround, _layerMask);

        bool _isGrounded = false;

        float _angle = 0f;
        if (_rayHit2D.collider != null)
        {
            _isGrounded = true;
            _angle = Vector2.Angle(Vector2.up, _rayHit2D.normal);
            if (_rayHit2D.normal.x < 0f)
            {
                _angle = -_angle;
            }

            const float _maxAngle = 90f;
            _angle = Mathf.Clamp(_angle, -_maxAngle, _maxAngle);
            float _speedMultiplier = 0f;

            if (_angle > 0f)
            {
                _speedMultiplier = 10f;
            }
            else if (_angle < 0f)
            {
                _speedMultiplier = 5f;
            }

            if (_speedMultiplier != 0f)
            {
                _speed += (_angle / _maxAngle) * _speedMultiplier;
            }
        }

        Vector3 _localEulerAngles = transform.localEulerAngles;
        _localEulerAngles.z = Mathf.SmoothDampAngle(_localEulerAngles.z, -_angle, ref _velAngle, 0.0625f);
        transform.localEulerAngles = _localEulerAngles;

        Vector2 _velocity = _rigidbody2D.velocity;
        _velocity.x = _speed;
        _rigidbody2D.velocity = _velocity;

        if (!_isGrounded)
        {
            GroundLayer();
        }
    }

    private void CheckForObstacle()
    {
        Vector2 _endPoint = new Vector2(_startPointTr.bounds.center.x + _circleCollider2D.radius / 2 - 0.2f, _startPointTr.bounds.center.y);

        RaycastHit2D hit2D = Physics2D.Linecast(_startPointTr.bounds.center, _endPoint, 1 << 15);
        Debug.DrawLine(_startPointTr.bounds.center, _endPoint, Color.cyan, 4);

        if (hit2D.collider != null)
        {
            if (hit2D.collider.gameObject.GetComponent<Wall_repulsion>() != null)
            {
                _rigidbody2D.velocity = zeroMovementV2;
                isStuck = true;
            }
        }
        else
        {
            isStuck = false;
        }
    }

    private void CheckForPlatformEdge()
    {
        Vector2 _startPointPlus = new Vector2(_startPointTr.bounds.center.x + 0.33f, _startPointTr.bounds.center.y);
        RaycastHit2D _rayHit2DGround = Physics2D.Raycast(_startPointTr.bounds.center, Vector2.down, 100f, Extension.LayerWhatIsGround);
        RaycastHit2D _rayHit2DChasm = Physics2D.Raycast(_startPointPlus, Vector2.down, 100f, Extension.LayerWhatIsGround);

        Debug.DrawLine(_startPointTr.bounds.center, Vector2.down, Color.green, 4);
        Debug.DrawLine(_startPointPlus, Vector2.down, Color.blue, 4);

        if (_rayHit2DGround.collider != null && _rayHit2DChasm.collider == null)
        {
            _rigidbody2D.velocity = zeroMovementV2;
            isStuck = true;
        }
    }

    void GroundLayer()
    {
        RaycastHit2D _rayHit2D = Physics2D.Raycast(groundPoint.position, Vector2.down, 100f, Extension.LayerWhatIsGround);
        if (_rayHit2D.collider != null)
        {
            int _layerMask = _rayHit2D.collider.gameObject.layer;
            switch (_layerMask)
            {
                case 9: // Ground0
                    gameObject.layer = 27;
                    break;
                case 11: // Ground-1
                    gameObject.layer = 28;
                    break;
            }
        }
    }

    [ServerCallback]
    void OnTriggerEnter2D(Collider2D other)
    {
        GameObject _gameObject = other.gameObject;

        if (_gameObject.CompareTag("Player") || _gameObject.CompareTag("Bot"))
        {
            if (_gameObject == _weaponSettings.owner)
            {
                return;
            }

            Vector3 _hitPoint = GetHitPoint(_gameObject);
            _gameObject.GetComponent<ClientOnlineController>().TakeWeaponDamage(_weaponSettings.owner, _weaponSettings._weapon, _weaponSettings._damage, _hitPoint);
            PlayExplosion(_hitPoint);

            bool _useStun = _weaponSettings.boolProperty2;
            if (_useStun)
            {
                float _stunTime = _weaponSettings.floatProperty1;
                _gameObject.GetComponent<UnitControllerV2>().Stun(_stunTime);
            }

            Destroy(gameObject);
        }

        else if (Wall.IsWall(_gameObject))
        {
            Wall _wall = _gameObject.GetComponent<Wall>();
            if (_wall.CanWeaponDamage())
            {
                Vector3 _hitPoint = GetHitPoint(_gameObject);
                _wall.TakeWeaponDamage(WeaponSetings.Weapon.ElectroWhirl, _weaponSettings._damage, _hitPoint, _weaponSettings.owner);
                PlayExplosion(_hitPoint);
                Destroy(gameObject);
            }
        }

        else if (MonsterController.IsMonster(_gameObject))
        {
            MonsterController _monster = _gameObject.GetComponent<MonsterController>();
            Vector3 _hitPoint = GetHitPoint(_gameObject);
            _monster.TakeWeaponDamage(_weaponSettings.owner, _weaponSettings._weapon, _weaponSettings._damage, _hitPoint);
            PlayExplosion(_hitPoint);
            Destroy(gameObject);
        }

        else
        {
            if (!_weaponSettings._maskAttack.Contains(_gameObject.layer))
            {
                return;
            }

            if (!Extension.IsDestroyable(_gameObject))
            {
                return;
            }
            else
            {
                _rigidbody2D.velocity = zeroMovementV2;
            }

            if (_weaponSettings.destroyMask.Contains(_gameObject.layer))
            {
                Vector3 _hitPoint = GetHitPoint(_gameObject);
                PlayExplosion(_hitPoint);
                Destroy(_gameObject);
                //Destroy(gameObject);
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