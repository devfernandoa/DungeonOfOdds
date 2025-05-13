using UnityEngine;

[CreateAssetMenu(fileName = "NewFighter", menuName = "Fighter")]
public class Fighter : ScriptableObject
{
    public string fighterName;
    public Sprite icon;
    public enum Rarity { Common, Rare, Epic, Legendary }
    public Rarity rarity;
    public int health;
    public int attack;
    public float moveSpeed = 2f;
    public float attackCooldown = 1.5f;
    public int luck;
    public bool isRanged;
    public float range;
    public float projectileSpeed = 5f; // Only used for ranged
}
