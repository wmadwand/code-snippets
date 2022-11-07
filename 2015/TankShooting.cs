using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class PlayerShooting : MonoBehaviour, IPointerClickHandler
{
    public int m_PlayerNumber = 1;
    public GameObject Shell;
    public Transform fireTransform;
    public AudioSource m_ShootingAudio;
    public AudioClip m_ChargingClip;
    public AudioClip m_FireClip;
    public float m_MinLaunchForce = 15f;
    public float m_MaxLaunchForce = 30f;

    private string m_FireButton;
    private float m_CurrentLaunchForce;
    private float nextShot;
    public float fireRate;
    public GameObject target;

    private MouseCursorController mouseCursorController;
    public Camera camera;
    Ray ray;

    public float turretDegreesPerSecond = 30.0f;
    public float gunDegreesPerSecond = 30.0f;

    public float maxGunAngle = 45.0f;

    private Quaternion qTurret;
    private Quaternion qGun;
    private Quaternion qGunStart;
    private Transform turretTransform;
    private Transform gunTransform;
    private float gunAngle;

    private void Start()
    {
        m_FireButton = "Fire1";
        m_CurrentLaunchForce = m_MinLaunchForce;
        mouseCursorController = GetComponent<MouseCursorController>();
        turretTransform = transform.Find("turret").gameObject.transform;
        gunTransform = transform.Find("turret/turret_cannon").gameObject.transform;
    }

    private void Update()
    {
        ray = camera.ScreenPointToRay(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Shootable")) && mouseCursorController.cursorTextureIn)
        {
            target.transform.position = hit.point;
            UpdateTrajectory(fireTransform.position, BallisticVelocity(target.transform, gunAngle + 20), Physics.gravity);

            RotateTurret(hit, turretTransform, gunTransform);

            if (Input.GetButton(m_FireButton) && Time.time > nextShot && mouseCursorController.shotOutOfBoundary == true)
            {
                nextShot = Time.time + fireRate;
                Fire(target);
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        print("It's a gameobject!");
        Debug.Log("It's a gameobject!");
    }

    private void Fire(GameObject target)
    {
        GameObject shellInstance = Instantiate(Shell, fireTransform.position, fireTransform.rotation) as GameObject;
        shellInstance.transform.LookAt(target.transform, shellInstance.transform.forward);
        shellInstance.GetComponent<Rigidbody>().velocity = BallisticVelocity(target.transform, gunAngle + 20); //50
        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();
    }

    Vector3 BallisticVelocity(Transform target, float angle)
    {
        Vector3 dir = target.position - fireTransform.position;
        float height = dir.y;
        dir.y = 0;
        float dist = dir.magnitude;
        float a = angle * Mathf.Deg2Rad;
        dir.y = dist * Mathf.Tan(a);
        dist += height / Mathf.Tan(a);

        float velocity = Mathf.Sqrt(dist * Physics.gravity.magnitude / Mathf.Sin(2 * a));
        return velocity * dir.normalized;
    }

    void UpdateTrajectory(Vector3 initialPosition, Vector3 initialVelocity, Vector3 gravity)
    {
        int numSteps = 1000;
        float timeDelta = 1.0f / initialVelocity.magnitude;

        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.SetVertexCount(numSteps);

        Vector3 position = initialPosition;
        Vector3 velocity = initialVelocity;
        for (int i = 0; i < numSteps; ++i)
        {
            lineRenderer.SetPosition(i, position);

            position += velocity * timeDelta + 0.5f * gravity * timeDelta * timeDelta;
            velocity += gravity * timeDelta;
        }
    }

    void RotateTurret(RaycastHit hit, Transform turretTransform, Transform gunTransform)
    {
        float _deltaTime = Time.deltaTime;
        Vector3 targetPosition = hit.point;
        Vector3 turretPosition = turretTransform.position;
        Vector3 turretUpTransform = turretTransform.up;
        float distanceToPlane = Vector3.Dot(turretUpTransform, targetPosition - turretPosition);
        Vector3 planePoint = targetPosition - turretUpTransform * distanceToPlane;

        //turret
        Quaternion qTurret = Quaternion.LookRotation(turretPosition - planePoint, turretUpTransform);
        turretTransform.rotation = Quaternion.RotateTowards(turretTransform.rotation, qTurret, 90.0f * _deltaTime);

        //gun
        Vector3 v = new Vector3(0, -distanceToPlane, (turretPosition - planePoint).magnitude);
        Quaternion qGun = Quaternion.LookRotation(v);
        gunAngle = Quaternion.Angle(Quaternion.identity, qGun);
        if (gunAngle <= 30.0f) //&& gunAngle >= 0.0f
            gunTransform.localRotation = Quaternion.RotateTowards(gunTransform.localRotation, qGun, 30.0f * _deltaTime);
    }
}


