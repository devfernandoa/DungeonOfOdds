using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class BattleController : MonoBehaviour
{
    public Transform playerSpawnArea;
    public Transform enemySpawnArea;
    public GameObject fighterPrefab; // Use same prefab for both sides
    public float spacing = 200f;
    public TextMeshProUGUI floorHeaderText; // Assign in Inspector
    private List<FighterUnit> playerUnits = new List<FighterUnit>();
    private List<FighterUnit> enemyUnits = new List<FighterUnit>();
    private bool battleEnded = false;
    private bool battleStarted = false;
    public VictoryDefeatUI victoryUI; // Assign in Inspector

    private Dictionary<Fighter.Rarity, float> rarityWeights = new Dictionary<Fighter.Rarity, float>
    {
        { Fighter.Rarity.Common, 1f },
        { Fighter.Rarity.Rare, 0.1f },
        { Fighter.Rarity.Epic, 0.01f },
        { Fighter.Rarity.Legendary, 0.001f },
    };

    void Start()
    {
        battleEnded = false;
        battleStarted = false;
    }

    private void FixedUpdate()
    {
        bool hasValidFighters = BattleDataManager.Instance.selectedFighters.Exists(f => f != null);
        if (!hasValidFighters)
        {
            return;
        }

        if (BattleDataManager.Instance == null ||
        !BattleDataManager.Instance.selectedFighters.Exists(f => f != null))
        {
            Debug.LogWarning("No valid fighters selected!");
            return;
        }

        if (!battleStarted || battleEnded) return;

        CheckBattleEnd();
    }


    public void StartBattle()
    {
        if (BattleDataManager.Instance == null ||
        !BattleDataManager.Instance.selectedFighters.Exists(f => f != null))
        {
            Debug.LogWarning("No valid fighters selected!");
            return;
        }


        battleEnded = false;

        floorHeaderText.color = Color.white;
        floorHeaderText.text = $"Floor {BattleDataManager.Instance.currentFloor}";
        if (BattleDataManager.Instance.IsBossFloor(BattleDataManager.Instance.currentFloor))
        {
            floorHeaderText.color = Color.yellow;
            floorHeaderText.text += " - Boss Floor";
        }
        if (BattleDataManager.Instance == null)
        {
            Debug.LogError("BattleDataManager not found!");
            return;
        }

        bool hasValidFighters = BattleDataManager.Instance.selectedFighters.Exists(f => f != null);
        if (!hasValidFighters)
        {
            Debug.LogWarning("No valid fighters selected. Preventing battle start.");
            return;
        }

        // Clear existing units if any (important for re-entry)
        foreach (Transform child in playerSpawnArea)
            Destroy(child.gameObject);
        foreach (Transform child in enemySpawnArea)
            Destroy(child.gameObject);

        playerUnits.Clear();
        enemyUnits.Clear();

        SpawnPlayerUnits();
        SpawnEnemies();

        battleStarted = true;
    }

    void SpawnPlayerUnits()
    {
        var fighters = BattleDataManager.Instance.selectedFighters;
        List<Fighter> validFighters = new List<Fighter>();

        foreach (var f in fighters)
            if (f != null) validFighters.Add(f);

        List<Vector3> positions = GetEvenlySpacedPositions(playerSpawnArea, validFighters.Count);

        for (int i = 0; i < validFighters.Count; i++)
        {
            GameObject unitGO = Instantiate(fighterPrefab, positions[i], Quaternion.identity, playerSpawnArea);
            FighterUnit unit = unitGO.GetComponent<FighterUnit>();
            unit.Init(validFighters[i], isPlayer: true);
            playerUnits.Add(unit);
        }
    }
    void SpawnEnemies()
    {
        int floor = BattleDataManager.Instance.currentFloor;
        bool IsBossFloor(int floor) => floor % 10 == 0;

        System.Random rng = new System.Random(BattleDataManager.Instance.currentFloor);
        bool isBoss = IsBossFloor(floor);
        int enemyCount = isBoss ? 1 : rng.Next(2, 4);

        List<Fighter> enemies = isBoss
            ? new List<Fighter> { GenerateBossFighter(floor, rng) }
            : GenerateEnemiesForFloor(floor, enemyCount, rng);

        List<Vector3> positions = GetEvenlySpacedPositions(enemySpawnArea, enemyCount);

        for (int i = 0; i < enemies.Count; i++)
        {
            GameObject unitGO = Instantiate(fighterPrefab, positions[i], Quaternion.identity, enemySpawnArea);
            FighterUnit unit = unitGO.GetComponent<FighterUnit>();
            unit.Init(enemies[i], isPlayer: false);
            enemyUnits.Add(unit);
        }
    }

    List<Vector3> GetEvenlySpacedPositions(Transform area, int count)
    {
        List<Vector3> positions = new List<Vector3>();

        if (count <= 0) return positions;

        float width = area.GetComponent<RectTransform>().rect.width;
        Vector3 start = area.position - area.right * width / 2f;
        Vector3 end = area.position + area.right * width / 2f;

        for (int i = 0; i < count; i++)
        {
            float t = count == 1 ? 0.5f : (float)i / (count - 1); // center if only 1 unit
            Vector3 pos = Vector3.Lerp(start, end, t);
            positions.Add(pos);
        }

        return positions;
    }
    public List<Fighter> GenerateEnemiesForFloor(int floor, int enemyCount, System.Random rng)
    {
        List<Fighter> pool = BattleDataManager.Instance.allAvailableFighters;

        List<Fighter> result = new List<Fighter>();

        for (int i = 0; i < enemyCount; i++)
        {
            Fighter selected = GetRandomFighterByRarity(pool, rng);
            result.Add(CloneAndScaleFighter(selected, floor));
        }

        return result;
    }
    private Fighter GetRandomFighterByRarity(List<Fighter> pool, System.Random rng)
    {
        // Build weighted list
        List<(Fighter fighter, float weight)> weighted = new();

        foreach (var f in pool)
        {
            if (rarityWeights.TryGetValue(f.rarity, out float weight))
            {
                weighted.Add((f, weight));
            }
        }

        // Total weight
        float total = 0f;
        foreach (var entry in weighted)
            total += entry.weight;

        // Roll a random number
        float roll = (float)(rng.NextDouble() * total);
        float cumulative = 0f;

        foreach (var entry in weighted)
        {
            cumulative += entry.weight;
            if (roll <= cumulative)
                return entry.fighter;
        }

        return weighted[0].fighter; // fallback
    }
    private Fighter CloneAndScaleFighter(Fighter baseFighter, int floor)
    {
        Fighter f = ScriptableObject.CreateInstance<Fighter>();

        f.fighterName = baseFighter.fighterName;
        f.icon = baseFighter.icon;
        f.rarity = baseFighter.rarity;
        f.isRanged = baseFighter.isRanged;
        f.range = baseFighter.range;
        f.projectileSpeed = baseFighter.projectileSpeed;
        f.moveSpeed = baseFighter.moveSpeed;
        f.attackCooldown = baseFighter.attackCooldown;
        f.luck = 0;

        float healthScale = 1f + 0.25f * floor;
        float attackScale = 1f + 0.15f * floor;

        f.health = Mathf.RoundToInt(baseFighter.health * healthScale);
        f.attack = Mathf.RoundToInt(baseFighter.attack * attackScale);

        return f;
    }
    Fighter GenerateBossFighter(int floor, System.Random rng)
    {
        var pool = BattleDataManager.Instance.allAvailableFighters;

        Fighter baseFighter = pool[rng.Next(pool.Count)];
        Fighter boss = ScriptableObject.CreateInstance<Fighter>();

        boss.fighterName = "BOSS: " + baseFighter.fighterName;
        boss.icon = baseFighter.icon; // You could tint or swap this later
        boss.rarity = baseFighter.rarity;
        boss.isRanged = baseFighter.isRanged;
        boss.range = baseFighter.range;
        boss.projectileSpeed = baseFighter.projectileSpeed;
        boss.moveSpeed = baseFighter.moveSpeed;
        boss.attackCooldown = baseFighter.attackCooldown;
        boss.luck = baseFighter.luck;

        // Extra scaling for bosses
        float healthScale = 2.5f + 0.3f * floor;
        float attackScale = 2.0f + 0.2f * floor;

        boss.health = Mathf.RoundToInt(baseFighter.health * healthScale);
        boss.attack = Mathf.RoundToInt(baseFighter.attack * attackScale);

        return boss;
    }

    public void HandleBattleOutcome(bool playerWon)
    {
        victoryUI.Show(playerWon);

    }
    void CheckBattleEnd()
    {
        if (battleEnded) return;

        // Remove any null or dead fighters
        playerUnits.RemoveAll(u => u == null || u.currentHealth <= 0);
        enemyUnits.RemoveAll(u => u == null || u.currentHealth <= 0);

        if (playerUnits.Count == 0)
        {
            Debug.Log("Player units defeated!");
            battleEnded = true;
            battleStarted = false;
            HandleBattleOutcome(false); // defeat
        }
        else if (enemyUnits.Count == 0)
        {
            Debug.Log("Enemy units defeated!");
            battleEnded = true;
            battleStarted = false;
            HandleBattleOutcome(true); // victory
        }
    }

}
