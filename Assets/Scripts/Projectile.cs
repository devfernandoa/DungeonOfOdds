using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Transform target;
    private float speed;
    private float damage;
    private float hitThreshold = 0.25f; // increased slightly for safety

    public void Init(Transform target, float damage, float speed)
    {
        this.target = target;
        this.damage = damage;
        this.speed = speed;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 direction = (target.position - transform.position);
        float distanceThisFrame = speed * Time.deltaTime;

        if (direction.sqrMagnitude <= hitThreshold * hitThreshold)
        {
            HitTarget();
            return;
        }

        transform.Translate(direction.normalized * distanceThisFrame, Space.World);
    }

    void HitTarget()
    {
        FighterUnit unit = target.GetComponent<FighterUnit>();
        if (unit != null)
        {
            unit.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
