using UnityEngine;
using System.Collections.Generic;

public class PointsManager : MonoBehaviour
{
    public static PointsManager Instance;
    private Dictionary<Fighter.Rarity, float> rarityMultipliers = new Dictionary<Fighter.Rarity, float>
{
    { Fighter.Rarity.Common, 1f },
    { Fighter.Rarity.Rare, 1.5f },
    { Fighter.Rarity.Epic, 2.5f },
    { Fighter.Rarity.Legendary, 4f },
};

    public int currentPoints = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadPoints();
    }

    public void AddPoints(int amount)
    {
        currentPoints += amount;
        SavePoints();
        Debug.Log($"Gained {amount} points! Total: {currentPoints}");
    }

    public void AddPointsForKill(Fighter fighter, int floor)
    {
        float basePoints = 10f + (floor * 2f);

        float rarityMultiplier = 1f;
        if (rarityMultipliers.TryGetValue(fighter.rarity, out float multiplier))
        {
            rarityMultiplier = multiplier;
        }

        int finalReward = Mathf.RoundToInt(basePoints * rarityMultiplier);
        AddPoints(finalReward);
    }


    public void SpendPoints(int amount)
    {
        currentPoints = Mathf.Max(0, currentPoints - amount);
        SavePoints();
    }

    public void SavePoints()
    {
        PlayerPrefs.SetInt("PlayerPoints", currentPoints);
        PlayerPrefs.Save();
    }

    public void LoadPoints()
    {
        currentPoints = PlayerPrefs.GetInt("PlayerPoints", 500);
        if (currentPoints == 0)
        {
            // Give some starting points if none are saved
            currentPoints = 500;
        }
    }

    public void ResetPoints()
    {
        currentPoints = 0;
        SavePoints();
    }
}
