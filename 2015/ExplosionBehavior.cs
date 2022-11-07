using UnityEngine;
using System.Collections;

public class ExplosionBehavior : MonoBehaviour
{
    public int valueDamage;
    public float radiusDamage;
    EnemyHealth enemyHealth;

    void Update()
    {
        AreaDamageEnemies(transform.position, radiusDamage, valueDamage);
    }

    void AreaDamageEnemies(Vector3 location, float radius, int damage)
    {
        Collider[] objectsInRange = Physics.OverlapSphere(location, radius);
        foreach (Collider col in objectsInRange)
        {
            EnemyHealth enemyHealth = col.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                RaycastHit hit;
                Vector3 enemyPos = enemyHealth.gameObject.transform.position - location;

                if (Physics.Raycast(location, enemyPos, out hit, radius, 1 << LayerMask.NameToLayer("Shootable")))
                {
                    Debug.DrawRay(location, enemyPos, Color.red);

                    if (hit.collider.gameObject.GetComponent<EnemyHealth>() != null)
                    {
                        hit.collider.gameObject.GetComponent<EnemyHealth>().TakeDamage(valueDamage);                        
                    }
                }
            }
        }
    }
}
