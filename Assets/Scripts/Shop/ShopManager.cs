using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public Transform dailyShopContainer;
    public GameObject shopItemPrefab;

    public TextMeshProUGUI randomFighterButtonText;
    public TextMeshProUGUI pointsText;

    public int baseRandomPrice = 50;
    public int randomBuyCount = 0;
    private const string BuyCountKey = "RandomBuyCount";

    private List<Fighter> fighterPool => BattleDataManager.Instance.allAvailableFighters;
    private List<ShopOffer> dailyOffers = new();

    private void Start()
    {
        // wait 1 second to ensure all fighters are loaded
        Invoke(nameof(StartShop), 1f);
    }

    private void StartShop()
    {
        LoadBuyCount();
        GenerateDailyShop();
        UpdateUI();
    }

    public void BuyRandomFighter()
    {
        int cost = GetRandomFighterCost();
        if (PointsManager.Instance.currentPoints < cost) return;

        Fighter fighter = GetRandomFighterByRarity();
        if (fighter == null) return;

        AvailableFightersPanel.Instance.AddFighterToList(fighter);
        BattleDataManager.Instance.SaveUnlockedFighters();
        PointsManager.Instance.SpendPoints(cost);
        randomBuyCount++;
        SaveBuyCount();

        FighterPopupUI.Instance.Show(fighter);

        UpdateUI();
    }

    int GetRandomFighterCost()
    {
        return baseRandomPrice + (randomBuyCount * 100);
    }

    Fighter GetRandomFighterByRarity()
    {
        // Same logic as in BattleController's rarity weights
        Dictionary<Fighter.Rarity, float> rarityWeights = new()
        {
            { Fighter.Rarity.Common, 1f },
            { Fighter.Rarity.Rare, 0.1f },
            { Fighter.Rarity.Epic, 0.01f },
            { Fighter.Rarity.Legendary, 0.001f },
        };

        List<(Fighter, float)> weighted = new();
        foreach (var f in fighterPool)
        {
            if (rarityWeights.TryGetValue(f.rarity, out float weight))
                weighted.Add((f, weight));
        }

        float total = 0f;
        foreach (var (f, w) in weighted) total += w;

        float roll = Random.Range(0f, total);
        float cumulative = 0f;
        foreach (var (f, w) in weighted)
        {
            cumulative += w;
            if (roll <= cumulative) return f;
        }

        return fighterPool[0];
    }

    public void GenerateDailyShop()
    {
        dailyOffers.Clear();
        foreach (Transform child in dailyShopContainer)
            Destroy(child.gameObject);

        for (int i = 0; i < 3; i++)
        {
            Fighter fighter = GetRandomFighterByRarity();
            int cost = CalculateShopPrice(fighter);
            var offer = new ShopOffer { fighter = fighter, price = cost };
            dailyOffers.Add(offer);

            GameObject item = Instantiate(shopItemPrefab, dailyShopContainer);
            item.GetComponent<ShopItemUI>().Setup(offer, this);
        }
    }

    int CalculateShopPrice(Fighter f)
    {
        int baseCost = 50 + (randomBuyCount * 20);
        return f.rarity switch
        {
            Fighter.Rarity.Common => baseCost,
            Fighter.Rarity.Rare => baseCost + 100,
            Fighter.Rarity.Epic => baseCost + 300,
            Fighter.Rarity.Legendary => baseCost + 700,
            _ => baseCost
        };
    }

    public void BuyDailyOffer(ShopOffer offer)
    {
        if (PointsManager.Instance.currentPoints < offer.price) return;

        AvailableFightersPanel.Instance.AddFighterToList(offer.fighter);
        BattleDataManager.Instance.SaveUnlockedFighters();
        PointsManager.Instance.SpendPoints(offer.price);
        GenerateDailyShop(); // Optional: refresh after buy

        FighterPopupUI.Instance.Show(offer.fighter);

    }

    void UpdateUI()
    {
        pointsText.text = $"Points: {PointsManager.Instance.currentPoints}";
        randomFighterButtonText.text = $"Buy Random ({GetRandomFighterCost()})";
    }

    private void SaveBuyCount()
    {
        PlayerPrefs.SetInt(BuyCountKey, randomBuyCount);
        PlayerPrefs.Save();
    }

    private void LoadBuyCount()
    {
        randomBuyCount = PlayerPrefs.GetInt(BuyCountKey, 0);
    }
}
