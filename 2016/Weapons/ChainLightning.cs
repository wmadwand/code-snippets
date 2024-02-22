/// <summary>
/// Programmed by WMADWAND
/// </summary>

/// <summary>
/// THE ALGORITHM BRIEFLY: 
/// 
/// Start() -> LaunchFirstAttack() -> Now looking for the nearest enemy for the first attack to realize -> Got it, the nearest enemy has beeen found! ->
/// -> LaunchChainAttack() -> Damage the found nearest enemy -> MakeBounce() from the found nearest enemy to the next one according to the sequence of all the found enemies (BounceCount times).
/// 
/// </summary>

using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;

[AddComponentMenu("JNC/Weapons/Chain Lightning")]
public class ChainLightning : NetworkBehaviour
{
    #region Variables
    WeaponSetings _weaponSettings;

    public float _boltScaleTest; //TODO WMADWAND: remove this variable

    private LayerMask _layerMaskAttack;
    List<GameObject> damagedTargets = new List<GameObject>();
    float damageValue;

    GameObject chosenTarget;
    GameObject prevDamagedTarget;

    Vector2 currBounceAreaCenter;
    Vector2 prevBounceAreaCenter;
    GameObject _dynamicBoltPref;
    GameObject _animBoltPref;
    GameObject _explosionEffect;
    GameObject animBoltGO;

    GameObject prevTargetGO, currTargetGO;
    GameObject currTargetDuplicate;
    GameObject theWallTargetDuplicate;
    Dictionary<GameObject, Vector2> theWallTargetDuplicateDict = new Dictionary<GameObject, Vector2>();
    GameObject[] _keys;
    bool showChLit = false;

    private const string _theWallTargetDuplicateName = "_theWallTargetDuplicate";
    float _offsetX, _offsetY;
    GameObject theWallGO;

    float boltLength = 8f;
    float boltScaleOriginX = 0.5f;
    #endregion

    #region Service methods
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

    void ClearCache()
    {
        Destroy(_dynamicBoltPref);
        Destroy(currTargetDuplicate);
        Destroy(theWallTargetDuplicate);
        showChLit = false;
    }

    GameObject CreateTargetDuplicateGO(string nameGO)
    {
        GameObject containerGO = new GameObject(nameGO);
        BoxCollider2D _colw = containerGO.AddComponent<BoxCollider2D>();
        NetworkIdentity _netId = containerGO.AddComponent<NetworkIdentity>();

        containerGO.layer = LayerMask.NameToLayer("Enemy");
        _colw.isTrigger = true;
        containerGO.transform.SetParent(gameObject.transform);

        return containerGO;
    }

    GameObject GenerateWallObjectsForBounces(RaycastHit2D _raycastHit2D)
    {
        Wall _wall = _raycastHit2D.collider.gameObject.GetComponent<Wall>();
        if (_wall.CanWeaponDamage())
        {
            if (theWallTargetDuplicate == null)
            {
                Vector2 firstDuplicatePos = _raycastHit2D.point; //_raycastHit2D.transform.localPosition;
                float _pointX = 0;
                float _pointY = 0;

                int _wallObjectsCount = damagedTargets.Count() > 0 ? _weaponSettings.intProperty1 - (damagedTargets.Count() - 1) : _weaponSettings.intProperty1 + 1;

                for (int i = 0; i < _wallObjectsCount; i++)
                {
                    GameObject _currDuplicate = CreateTargetDuplicateGO(_theWallTargetDuplicateName + i);

                    //float _posY = Mathf.Clamp(firstDuplicatePos.y + _offsetY, 424, 437); //424,437
                    //float _posX = Mathf.Clamp(firstDuplicatePos.x + _offsetX, -13, -4); //-13, -4

                    // Set the area borders here to spread objects for bounces in.
                    _pointX = UnityEngine.Random.Range(theWallGO.transform.position.x + 2f, theWallGO.transform.position.x + 12f);
                    _pointY = UnityEngine.Random.Range(theWallGO.transform.position.y - 7f, theWallGO.transform.position.y + 7f);

                    // Set coordinates for the current object
                    _currDuplicate.transform.position = new Vector2(_pointX, _pointY);

                    // Calculate the offsets so that objects could move with the Wall appropriately (In FixedUpdate())
                    //_offsetX = Math.Abs(_raycastHit2D.collider.gameObject.GetComponent<Collider2D>().bounds.center.x - _currDuplicate.GetComponent<Collider2D>().bounds.center.x);
                    //_offsetY = Math.Abs(_raycastHit2D.collider.gameObject.GetComponent<Collider2D>().bounds.center.y - _currDuplicate.GetComponent<Collider2D>().bounds.center.y);
                    _offsetX = _currDuplicate.GetComponent<Collider2D>().bounds.center.x - _raycastHit2D.collider.gameObject.GetComponent<Collider2D>().bounds.center.x;
                    _offsetY = _currDuplicate.GetComponent<Collider2D>().bounds.center.y - _weaponSettings.owner.transform.position.y;

                    theWallTargetDuplicateDict[_currDuplicate] = new Vector2(_offsetX, _offsetY);
                }
            }
        }


        _keys = theWallTargetDuplicateDict.Keys.ToArray();
        GameObject[] theLowestGOs = (from _key in _keys
                                     orderby _key.gameObject.transform.localPosition.y ascending
                                     select _key.gameObject).ToArray();
        GameObject theLowestGO = theLowestGOs[0];

        //List < GameObject > _gos = theWallTargetDuplicateDict.Keys.ToList();
        //_gos.Sort((GameObject go1, GameObject go2) => { return go1.transform.position.y.CompareTo(go2.transform.position.y); });
        //GameObject _go = _gos.First();

        return theLowestGO;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red; // new Color(1,1,1,.5f);

        foreach (KeyValuePair<GameObject, Vector2> item in theWallTargetDuplicateDict)
        {
            if (item.Key != null)
            {
                Gizmos.DrawSphere(item.Key.transform.position, .5f);
            }
        }
    }
#endif
    #endregion

    #region Base methods
    void FixedUpdate()
    {
        if (theWallTargetDuplicate != null && theWallGO != null)
        {
            float x1 = theWallGO.transform.position.x;
            float y1 = _weaponSettings.owner.transform.position.y;

            foreach (KeyValuePair<GameObject, Vector2> currWallDuplicate in theWallTargetDuplicateDict)
            {
                //currWallDuplicate.Key.transform.position = new Vector2(x1 + currWallDuplicate.Value.x, y1 + currWallDuplicate.Value.y);
                currWallDuplicate.Key.transform.position = new Vector2(x1 + currWallDuplicate.Value.x, y1 + currWallDuplicate.Value.y);
            }
        }

        if (showChLit && prevTargetGO != null && currTargetGO != null)
        {
            ////Vector3 prevTargetPos = prevTargetGO.GetComponent<Collider2D>().bounds.center;
            ////Vector3 currTargetPos = currTargetGO.GetComponent<Collider2D>().bounds.center;
            Vector2 prevTargetPos = new Vector2(prevTargetGO.GetComponent<Collider2D>().bounds.center.x + prevTargetGO.GetComponent<Collider2D>().offset.x,
                                                prevTargetGO.GetComponent<Collider2D>().bounds.center.y + prevTargetGO.GetComponent<Collider2D>().offset.y);
            Vector2 currTargetPos = new Vector2(currTargetGO.GetComponent<Collider2D>().bounds.center.x + currTargetGO.GetComponent<Collider2D>().offset.x,
                                                currTargetGO.GetComponent<Collider2D>().bounds.center.y + currTargetGO.GetComponent<Collider2D>().offset.y);

            ////_dynamicBoltPref.transform.Find("LightningStart").GetComponent<Transform>().position = prevTargetPos;
            ////_dynamicBoltPref.transform.Find("LightningEnd").GetComponent<Transform>().position = currTargetPos;

            if (animBoltGO != null)
            {
                Vector2 currBoltDirection = currTargetPos - prevTargetPos;
                animBoltGO.transform.right = currBoltDirection;
                animBoltGO.transform.position = prevTargetPos;

                float scaleFactor = 0;

                ////if (animBoltGO.transform.localScale.x <= boltScaleOriginX)
                ////{
                float distance = Vector2.Distance(prevTargetPos, currTargetPos);

                //if (distance >= boltLength)
                //{
                ////int countFullLengths = (int)(distance / boltLength);
                ////scaleFactor = distance - (float)Math.Truncate(distance);
                ////scaleFactor = scaleFactor > boltScaleOriginX ? 0.5f : scaleFactor;

                //scaleFactor = 0.5f;
                //}

                //else
                //{
                scaleFactor = distance / boltLength / _boltScaleTest/*2*/;
                //}

                Vector3 currScale = animBoltGO.transform.localScale;
                animBoltGO.transform.localScale = new Vector3(scaleFactor, .5f/*scaleFactor*/, 1);
                ////}



                Debug.DrawLine(prevTargetPos, currTargetPos, Color.blue, 2);
            }
        }
    }

    void Start()
    {
        LaunchFirstAttack();
    }

    void OnDestroy()
    {
        ClearCache();
    }
    #endregion

    #region Attack methods
    private void LaunchFirstAttack()
    {
        _animBoltPref = _weaponSettings.extraPrefab;

        theWallGO = GameObject.FindGameObjectWithTag("Wall");
        damageValue = _weaponSettings._damage;
        ProcessLayersMaskAttack();

        Vector2 playerPos = new Vector2(_weaponSettings.owner.GetComponent<Collider2D>().bounds.center.x + _weaponSettings.owner.GetComponent<Collider2D>().offset.x,
                                        _weaponSettings.owner.GetComponent<Collider2D>().bounds.center.y + _weaponSettings.owner.GetComponent<Collider2D>().offset.y);
        //; _weaponSettings.owner.GetComponent<CapsuleCollider2D>().bounds.center;
        Collider2D[] targetsInTheFirstAttackArea = Physics2D.OverlapCircleAll(playerPos, _weaponSettings.floatProperty1, _layerMaskAttack);

        Debug.DrawLine(playerPos, new Vector2(playerPos.x + _weaponSettings.floatProperty1, playerPos.y), Color.white, 2);

        if (targetsInTheFirstAttackArea.Length > 0)
        { //got someone in the first attack radius... so shoot the lightning straight forward
            Debug.Log("Got someone in the radius...");

            Vector2 attackRayEndPoint = new Vector2(playerPos.x + _weaponSettings.floatProperty1, playerPos.y);

            #region Solutions for facing animChLit's bolt towards the enemy
            //GameObject currStateGO = _weaponSettings.owner.GetComponent<UnitSettingsV2>().currentInstallState[_weaponSettings.owner.GetComponent<UnitSettingsV2>().currentState];
            //Vector2 attackRayEndPoint = currStateGO.transform.forward;

            //Vector3 attackRayEndPoint = _weaponSettings.owner.GetComponent<Rigidbody2D>().;
            //Quaternion qqq = _weaponSettings.owner.transform.rotation;
            //Vector3 attackRayEndPoint = new Vector3(Mathf.Cos(qqq.x), Mathf.Sin(qqq.y), 0f);

            //Quaternion rotation = Quaternion.AngleAxis(45.0f, transform.right);
            //Vector3 attackRayEndPoint = currStateGO.transform.TransformDirection(Vector3.forward)*10;
            #endregion

            RaycastHit2D _raycastHit2D = Physics2D.Linecast(playerPos, attackRayEndPoint, _layerMaskAttack);

            Debug.DrawLine(playerPos, attackRayEndPoint, Color.green, 2);

            if (_raycastHit2D.collider != null && _raycastHit2D.collider.gameObject != _weaponSettings.owner)
            { //shot someone
                Debug.Log("The first attack shot!");

                if (Wall.IsWall(_raycastHit2D.collider.gameObject))
                {
                    theWallTargetDuplicate = GenerateWallObjectsForBounces(_raycastHit2D)/*.First().Key*/; // use SortedList instead ??? OR SortedDictionary !!!
                                                                                                           //theWallTargetDuplicate.transform.position = _raycastHit2D.point;
                    chosenTarget = theWallTargetDuplicate;
                    //_differenceX = Math.Abs(_raycastHit2D.collider.gameObject.GetComponent<Collider2D>().bounds.center.x - theWallTargetDuplicate.GetComponent<Collider2D>().bounds.center.x);
                }
                else
                {
                    chosenTarget = _raycastHit2D.collider.gameObject;

                    if (currTargetDuplicate == null)
                    {
                        currTargetDuplicate = CreateTargetDuplicateGO("_currTargetDuplicate");
                    }
                    currTargetDuplicate.transform.position = chosenTarget.transform.position;
                }

                StartCoroutine(LaunchChainAttack(_weaponSettings.owner, chosenTarget));
            }
        }
        else
        {
            Debug.LogWarning("Have no targets for the first attack which depends on the radius = _weaponSettings.floatProperty1");
        }
    }

    IEnumerator LaunchChainAttack(GameObject _prevTargetGO, GameObject _currTargetGO)
    {
        Debug.DrawLine(_prevTargetGO.GetComponent<Collider2D>().bounds.center, _currTargetGO.GetComponent<Collider2D>().bounds.center, Color.magenta, 5); //cast the lightning to the center of the object

        StartCoroutine(PlayAnimLightning(_prevTargetGO, _currTargetGO));
        ////PlayDynamicLightning(_prevTargetGO, _currTargetGO);
        yield return new WaitForSeconds(_weaponSettings._speed);

        if (_currTargetGO != null)
        {
            MakeDamage(_currTargetGO, damageValue);
        }

        yield return null;

        prevTargetGO = _currTargetGO != null ? _currTargetGO : currTargetDuplicate;

        for (int i = 0; i < _weaponSettings.intProperty1; i++) //make bounces to the other nearest objects
        {
            GameObject chosenBounceTarget = MakeBounce(prevTargetGO.GetComponent<Collider2D>().bounds.center, i);

            if (chosenBounceTarget != null)
            {
                Vector2 _tempChosenBounceTargetPos = chosenBounceTarget.transform.position;
                //currTargetDuplicate.transform.position = chosenBounceTarget.transform.position; //put this line only right here!!!
                StartCoroutine(PlayAnimLightning(prevTargetGO, chosenBounceTarget));
                ////PlayDynamicLightning(prevTargetGO, chosenBounceTarget);
                yield return new WaitForSeconds(_weaponSettings._speed);

                if (currTargetDuplicate == null)
                {
                    currTargetDuplicate = CreateTargetDuplicateGO("_currTargetDuplicate");
                }

                if (chosenBounceTarget != null)
                {
                    currTargetDuplicate.transform.position = chosenBounceTarget.transform.position; //put this line only right here!!!
                    MakeDamage(chosenBounceTarget, damageValue);
                }
                else
                {
                    currTargetDuplicate.transform.position = _tempChosenBounceTargetPos;
                }

                ////else
                ////{
                ////    yield break;
                ////}

                yield return null;

                prevTargetGO = chosenBounceTarget != null ? chosenBounceTarget : currTargetDuplicate;
                Debug.Log("--- damage has been taken: " + i);
            }
        }

        ClearCache();
    }

    GameObject MakeBounce(Vector2 _currBounceAreaCenter, int g)
    {
        Collider2D[] targetsInTheBounceArea = Physics2D.OverlapCircleAll(_currBounceAreaCenter, _weaponSettings.floatProperty2, _layerMaskAttack);
        Vector2 chosenBounceTargetCenter = Vector2.zero;

        if (targetsInTheBounceArea.Length > 0)
        {
            float minDistance = 100000f;
            GameObject nearestObject = null;
            bool gotSuitableTarget = false;

            foreach (Collider2D _col in targetsInTheBounceArea)
            {
                if (_col.gameObject == _weaponSettings.owner ||
                    _col.gameObject == null ||
                    (_col.gameObject.GetComponent<NetworkIdentity>() == null /*|| _col.gameObject.name != _theWallTargetDuplicateName*/) ||
                    _col.gameObject == (prevTargetGO == theWallTargetDuplicate ? theWallGO : prevTargetGO) ||
                     (Wall.IsWall(_col.gameObject) && (theWallTargetDuplicate != null)))
                {
                    continue;
                }

                if (!_weaponSettings.boolProperty2)
                {
                    if (damagedTargets.Find(item => item == _col.gameObject) != null)
                    {
                        continue;
                    }
                }

                RaycastHit2D[] _hits2D = Physics2D.LinecastAll(_currBounceAreaCenter, _col.bounds.center, _layerMaskAttack);
                RaycastHit2D[] allRightHits2D = (from hit in _hits2D
                                                 where hit.collider.gameObject != _weaponSettings.owner &&
                                                 hit.collider.gameObject != (theWallTargetDuplicate != null ? theWallGO : null)

                                                 orderby hit.distance ascending
                                                 select hit).ToArray();
                if (allRightHits2D.Length > 0)
                {
                    RaycastHit2D _hit2D = allRightHits2D[0];
                    float _distance = Vector2.Distance(_currBounceAreaCenter, _col.bounds.center);

                    if (_distance < minDistance)
                    {
                        minDistance = _distance;

                        if (Wall.IsWall(_hit2D.collider.gameObject) && theWallTargetDuplicate == null)
                        {
                            theWallTargetDuplicate = GenerateWallObjectsForBounces(_hit2D)/*.First().Key*/; // use SortedList instead ??? OR SortedDictionary !!!
                            //theWallTargetDuplicate.transform.position = _hit2D.point;
                            nearestObject = theWallTargetDuplicate;

                            //Wall _wall = _hit2D.collider.gameObject.GetComponent<Wall>();
                            //if (_wall.CanWeaponDamage())
                            //{
                            //    if (theWallTargetDuplicate == null)
                            //    {
                            //        theWallTargetDuplicate = CreateTargetDuplicateGO(_theWallTargetDuplicateName);
                            //    }

                            //    theWallTargetDuplicate.transform.position = _hit2D.point;
                            //    nearestObject = theWallTargetDuplicate;
                            //}


                        }
                        else if (Wall.IsWall(_hit2D.collider.gameObject) && theWallTargetDuplicate != null)
                        {
                            nearestObject = allRightHits2D[1].collider.gameObject;
                        }

                        else
                        {
                            nearestObject = _hit2D.collider.gameObject;
                        }

                        gotSuitableTarget = true;
                    }
                }
            }

            if (gotSuitableTarget)
            {
                Debug.DrawLine(prevTargetGO.GetComponent<Collider2D>().bounds.center, nearestObject.GetComponent<Collider2D>().bounds.center, Color.red, 5); //cast the lightning to the center of the object
                damageValue -= damageValue / 5;
                Debug.Log(string.Format("The #{0} bounce shot with damage = {1}!", g++, damageValue));

                return nearestObject;
            }
        }

        Debug.Log("Haven't got targets for the bounces");
        return null;
    }

    void MakeDamage(GameObject _chosenTarget, float _damageValue)
    {
        if (_chosenTarget != null)
        {
            if (Extension.IsDestroyable(_chosenTarget))
            { //just single Enemy OR level Wall
                if (_weaponSettings.destroyMask.Contains(_chosenTarget.layer))
                {
                    damagedTargets.Add(_chosenTarget);
                    PlayExplosion(_chosenTarget, false);
                    Destroy(_chosenTarget);
                    animBoltGO.Recycle();
                    showChLit = false;
                }
            }
            else if (_chosenTarget.CompareTag("Player") || _chosenTarget.CompareTag("Bot"))
            { //either the Player or any Bot

                if (_chosenTarget == _weaponSettings.owner)
                {
                    return;
                }

                _chosenTarget.GetComponent<ClientOnlineController>().SetAttack(DamageType.Shot, _damageValue);

                damagedTargets.Add(_chosenTarget);
                PlayExplosion(_chosenTarget);
                animBoltGO.Recycle();
                showChLit = false;
            }
            else if (_keys != null && _keys.Count() > 0 && Array.Find(_keys, item => item == _chosenTarget))
            { //the MonsterWall               
                Wall _wall = GameObject.FindGameObjectWithTag("Wall").GetComponent<Wall>();
                if (_wall.CanWeaponDamage())
                {
                    Vector2 _hitPoint = _chosenTarget.transform.position;
                    _wall.TakeWeaponDamage(WeaponSetings.Weapon.ChainLightning, _damageValue, _hitPoint, _weaponSettings.owner);
                    damagedTargets.Add(_chosenTarget);
                    PlayExplosion(_chosenTarget);
                    animBoltGO.Recycle();
                    showChLit = false;
                }
            }
            else
            {
                Debug.LogError("SKIPPED -> " + _chosenTarget);
            }

            if (_chosenTarget != null)
            {
                if (!_weaponSettings._maskAttack.Contains(_chosenTarget.layer))
                {
                    showChLit = false;
                    return;
                }
            }
        }
    }
    #endregion

    #region Play lightning
    void PlayDynamicLightning(GameObject _prevTargetGO, GameObject _currTargetGO)
    {
        if (_dynamicBoltPref == null)
        {
            _dynamicBoltPref = (GameObject)Instantiate(_weaponSettings.abilityPrefab, _prevTargetGO.transform.position, Quaternion.identity);
            LineRenderer lineRenderer = _dynamicBoltPref.GetComponent<LineRenderer>();
            lineRenderer.sortingOrder = 10000;
            _dynamicBoltPref.transform.SetParent(gameObject.transform /*_weaponSettings.owner.transform*/);
        }

        //prevTargetGO = _prevTargetGO;
        //currTargetGO = _currTargetGO;
        //showChLit = true;
    }

    IEnumerator PlayAnimLightning(GameObject _prevTargetGO, GameObject _currTargetGO)
    {
        Vector2 _prevTargetPos = _prevTargetGO.GetComponent<Collider2D>().bounds.center;
        Vector2 _currTargetPos = _currTargetGO.GetComponent<Collider2D>().bounds.center;

        //Vector2 offset = _prevTargetGO.GetComponent<Collider2D>().bounds.center - _currTargetGO.GetComponent<Collider2D>().bounds.center;
        //float sqrDistance = offset.sqrMagnitude;
        float distance = Vector2.Distance(_prevTargetPos, _currTargetPos);

        Vector2 currPosRight = _prevTargetPos;

        Vector2 boltDirection = _currTargetPos - _prevTargetPos;
        //float angle = Mathf.Atan2(boltDirection.y, boltDirection.x) * Mathf.Rad2Deg;
        //Quaternion _rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        Quaternion _rotation = Quaternion.identity;

        prevTargetGO = _prevTargetGO;
        currTargetGO = _currTargetGO;
        showChLit = true;

        float delay = 2f;
        float remain = 0;

        if (distance >= boltLength)
        {
            int countFullLengths = (int)(distance / boltLength);
            remain = distance - (float)Math.Truncate(distance);

            //for (int i = 0; i < countFullLengths; i++)
            //{
            SpawnBoltItem(currPosRight, _rotation, boltDirection);
            //currPosRight.x += boltLength;
            //yield return new WaitForSeconds(delay);
            //Destroy(_boltGO);
            //}

            ////if (remain > 0)
            ////{
            ////    GameObject _boltGO = SpawnBoltItem(currPosRight, /*normalVec,*/ _currTargetGO, remkain / 2);
            ////    yield return new WaitForSeconds(1f);
            ////    Destroy(_boltGO);
            ////}
        }

        else
        {
            remain = distance / boltLength / _boltScaleTest;
            SpawnBoltItem(currPosRight, _rotation, boltDirection, remain);
        }

        yield return new WaitForSeconds(0);
    }

    private void SpawnBoltItem(Vector2 point, Quaternion _rotation, Vector2 _boltDirection, float scaleFactor = 0.5f)
    {
        animBoltGO = _animBoltPref.Spawn(null, point, _rotation);

        Vector3 currScale = animBoltGO.transform.localScale;
        animBoltGO.GetComponent<MeshRenderer>().sortingOrder = 20;
        animBoltGO.transform.localScale = new Vector3(scaleFactor, scaleFactor, 1);
    }

    private void PlayExplosion(GameObject target, bool attachToTarget = true)
    {
        Vector3 _targetV2 = new Vector2(target.GetComponent<Collider2D>().bounds.center.x + target.GetComponent<Collider2D>().offset.x,
                                        target.GetComponent<Collider2D>().bounds.center.y + target.GetComponent<Collider2D>().offset.y);
        ////Vector3 explPos = target.GetComponent<BoxCollider2D>() != null ? target.GetComponent<BoxCollider2D>().bounds.center : _targetV2; /* target.transform.position;*/
        Vector3 explPos = _targetV2;
        float _explosionRadius = 10;
        _explosionEffect = (GameObject)Instantiate(_weaponSettings.effectPrefab, explPos, Quaternion.identity);
        _explosionEffect.GetComponent<ExplEffect>().SetupEffect(_explosionRadius, true, 4);
        if (attachToTarget)
        {
            _explosionEffect.transform.SetParent(target.transform);
        }

        NetworkServer.Spawn(_explosionEffect);
    }
    #endregion
}