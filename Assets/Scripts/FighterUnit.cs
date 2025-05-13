using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class FighterUnit : MonoBehaviour
{
    public Fighter data;
    public bool isPlayer;
    public float currentHealth;

    private Transform target;
    private float attackCooldown = 1.5f;
    private float attackTimer;
    private float moveSpeed;
    public Image icon;
    public TextMeshProUGUI nameText;
    public Image healthBarFill;
    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;

    public void Init(Fighter fighterData, bool isPlayer)
    {
        if (fighterData == null)
        {
            Debug.LogError("Tried to init FighterUnit with null fighter!");
            return;
        }

        data = fighterData;
        this.isPlayer = isPlayer;
        currentHealth = data.health;

        attackTimer = data.attackCooldown;
        moveSpeed = data.moveSpeed;

        icon.sprite = data.icon;
        nameText.text = data.fighterName;
        icon.color = isPlayer ? Color.white : Color.red;

        if (!isPlayer && data.fighterName.StartsWith("BOSS:"))
        {
            icon.color = new Color(1f, 0.5f, 0.5f); // pink tint
            nameText.color = Color.yellow;
        }
    }

    void Update()
    {
        if (currentHealth <= 0) return;

        if (target == null || target.GetComponent<FighterUnit>().currentHealth <= 0)
        {
            target = FindNearestEnemy();
            if (target == null) return;
        }

        float dist = Vector3.Distance(transform.position, target.position);
        float range = data.range;

        if (dist > range)
        {
            // Only move if not already in range
            Vector3 dir = (target.position - transform.position).normalized;
            transform.position += dir * moveSpeed * Time.deltaTime;
        }
        else
        {
            // Already in range — attack
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
            {
                Attack();
                attackTimer = attackCooldown;
            }
        }
        UpdateHealthBar();
    }
    void UpdateHealthBar()
    {
        if (healthBarFill != null && data.health > 0)
        {
            float ratio = currentHealth / data.health;
            healthBarFill.fillAmount = Mathf.Clamp01(ratio);
        }
    }

    void Attack()
    {
        if (target == null) return;

        FighterUnit enemy = target.GetComponent<FighterUnit>();
        if (enemy == null) return;

        if (data.isRanged && projectilePrefab != null)
        {
            FireProjectile(enemy);
        }
        else
        {
            enemy.TakeDamage(data.attack);
        }
    }
    void FireProjectile(FighterUnit targetUnit)
    {
        Vector3 spawnPos = projectileSpawnPoint ? projectileSpawnPoint.position : transform.position;
        Transform worldCanvas = GameObject.Find("Projectiles").transform;
        GameObject proj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity, worldCanvas);

        Projectile p = proj.GetComponent<Projectile>();
        p.Init(targetUnit.transform, data.attack, data.projectileSpeed);
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    Transform FindNearestEnemy()
    {
        var all = FindObjectsOfType<FighterUnit>();
        float minDist = float.MaxValue;
        Transform nearest = null;

        foreach (var unit in all)
        {
            if (unit.isPlayer != this.isPlayer && unit.currentHealth > 0)
            {
                float dist = Vector3.Distance(transform.position, unit.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = unit.transform;
                }
            }
        }

        return nearest;
    }
}
