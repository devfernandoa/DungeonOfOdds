using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Transform target;
    private float speed;
    private float damage;
    private float hitThreshold = 10f;
    private bool hasHit = false;

    // Add a maximum lifetime to prevent stuck projectiles
    private float maxLifetime = 5f;
    private float lifetime = 0f;

    public void Init(Transform target, float damage, float speed)
    {
        this.target = target;
        this.damage = damage;
        this.speed = speed;
        hasHit = false;
        lifetime = 0f;
    }

    void Update()
    {
        // Increment lifetime and destroy if it exceeds maximum
        lifetime += Time.deltaTime;
        if (lifetime > maxLifetime)
        {
            Destroy(gameObject);
            return;
        }

        // If target is gone, destroy the projectile
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // Get the direction to the target
        Vector3 direction = (target.position - transform.position);
        float distanceThisFrame = speed * Time.deltaTime;

        // Check if we're close enough to hit
        if (direction.magnitude <= hitThreshold && !hasHit)
        {
            hasHit = true; // Prevent multiple hits
            HitTarget();
            return;
        }

        // Move toward the target
        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            distanceThisFrame
        );
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