using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System;

public class ShopManager : MonoBehaviour
{
    public Transform dailyShopContainer;
    public GameObject shopItemPrefab;

    public TextMeshProUGUI randomFighterButtonText;
    public TextMeshProUGUI pointsText;
    public TextMeshProUGUI countdownText; // New countdown text UI element

    public int baseRandomPrice = 50;
    public int randomBuyCount = 0;
    private const string BuyCountKey = "RandomBuyCount";
    private const string LastShopRefreshKey = "LastShopRefresh";
    private const string DailyShopOffersKey = "DailyShopOffers";

    private List<Fighter> fighterPool => BattleDataManager.Instance.allAvailableFighters;
    private List<ShopOffer> dailyOffers = new();
    private DateTime nextRefreshTime;

    private void Start()
    {
        // wait 1 second to ensure all fighters are loaded
        Invoke(nameof(StartShop), 1f);
    }

    private void StartShop()
    {
        LoadBuyCount();
        CheckAndRefreshShop();
        UpdateUI();

        // Start updating the countdown each second
        InvokeRepeating(nameof(UpdateCountdown), 0f, 1f);
    }

    private void CheckAndRefreshShop()
    {
        // Get current date (local time)
        DateTime now = DateTime.Now;
        DateTime today = now.Date; // Get today at midnight

        // Get last refresh date from PlayerPrefs
        string lastRefreshStr = PlayerPrefs.GetString(LastShopRefreshKey, "");
        DateTime lastRefresh;

        if (string.IsNullOrEmpty(lastRefreshStr) || !DateTime.TryParse(lastRefreshStr, out lastRefresh))
        {
            // First time running or invalid data, consider shop never refreshed
            lastRefresh = today.AddDays(-1); // Force refresh
        }

        // Calculate next refresh time (midnight tonight)
        nextRefreshTime = today.AddDays(1);

        // If last refresh was before today, we need to refresh the shop
        if (lastRefresh.Date < today)
        {
            GenerateDailyShop();
            SaveLastRefreshTime();
            SaveShopOffers(); // Save the newly generated offers
        }
        else
        {
            // Load the previous shop offers
            LoadShopOffers();
        }
    }

    private void SaveShopOffers()
    {
        // Convert shop offers to serializable format
        List<SerializableShopOffer> serializableOffers = new List<SerializableShopOffer>();
        foreach (var offer in dailyOffers)
        {
            serializableOffers.Add(new SerializableShopOffer
            {
                fighterName = offer.fighter.fighterName,
                price = offer.price
            });
        }

        // Convert to JSON and save
        string offersJson = JsonUtility.ToJson(new SerializableShopOffersList { offers = serializableOffers });
        PlayerPrefs.SetString(DailyShopOffersKey, offersJson);
        PlayerPrefs.Save();
    }

    private void LoadShopOffers()
    {
        string offersJson = PlayerPrefs.GetString(DailyShopOffersKey, "");

        if (string.IsNullOrEmpty(offersJson))
        {
            // No saved offers, generate new ones
            GenerateDailyShop();
            SaveShopOffers();
            return;
        }

        // Clear existing offers and UI
        dailyOffers.Clear();
        foreach (Transform child in dailyShopContainer)
            Destroy(child.gameObject);

        // Load and deserialize the saved offers
        SerializableShopOffersList savedOffers = JsonUtility.FromJson<SerializableShopOffersList>(offersJson);

        // Recreate the shop items
        foreach (var savedOffer in savedOffers.offers)
        {
            // Find the fighter by name in the fighter pool
            Fighter fighter = fighterPool.Find(f => f.fighterName == savedOffer.fighterName);

            if (fighter != null)
            {
                // Create a shop offer with the saved price
                var offer = new ShopOffer { fighter = fighter, price = savedOffer.price };
                dailyOffers.Add(offer);

                // Create and setup UI
                GameObject item = Instantiate(shopItemPrefab, dailyShopContainer);
                item.GetComponent<ShopItemUI>().Setup(offer, this);
            }
        }

        // If no valid offers were loaded (maybe fighter data changed), generate new ones
        if (dailyOffers.Count == 0)
        {
            GenerateDailyShop();
            SaveShopOffers();
        }
    }

    private void SaveLastRefreshTime()
    {
        // Save the current date as string
        PlayerPrefs.SetString(LastShopRefreshKey, DateTime.Now.ToString("o")); // ISO 8601 format
        PlayerPrefs.Save();
    }

    private void UpdateCountdown()
    {
        if (countdownText == null) return;

        DateTime now = DateTime.Now;
        TimeSpan timeRemaining = nextRefreshTime - now;

        // Format the countdown as "23:59:59"
        if (timeRemaining.TotalSeconds > 0)
        {
            countdownText.text = string.Format("Shop Refresh: {0:D2}:{1:D2}:{2:D2}",
                timeRemaining.Hours,
                timeRemaining.Minutes,
                timeRemaining.Seconds);
        }
        else
        {
            countdownText.text = "Refreshing...";
            CheckAndRefreshShop();
            UpdateUI();
        }
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

        float roll = UnityEngine.Random.Range(0f, total);
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

        // Don't regenerate the shop after purchase, wait for the 24-hour refresh
        // Instead, just remove the item from the shop
        RemoveOfferFromShop(offer);

        // Save the updated shop offers after removing one
        SaveShopOffers();

        FighterPopupUI.Instance.Show(offer.fighter);
    }

    private void RemoveOfferFromShop(ShopOffer offer)
    {
        // Find and remove the offer from the list
        dailyOffers.Remove(offer);

        // Refresh the UI without regenerating the offers
        foreach (Transform child in dailyShopContainer)
        {
            ShopItemUI itemUI = child.GetComponent<ShopItemUI>();
            if (itemUI != null && itemUI.offer == offer)
            {
                Destroy(child.gameObject);
                break;
            }
        }

        UpdateUI();
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