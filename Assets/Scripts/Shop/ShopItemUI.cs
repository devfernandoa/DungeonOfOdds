using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI rarityText;
    public Button buyButton;

    private ShopOffer offer;
    private ShopManager shop;

    public void Setup(ShopOffer offer, ShopManager shop)
    {
        this.offer = offer;
        this.shop = shop;

        Fighter f = offer.fighter;

        icon.sprite = f.icon;
        nameText.text = f.fighterName;
        priceText.text = $"{offer.price} pts";
        rarityText.text = f.rarity.ToString().ToUpper();

        // Optional: color code rarity
        rarityText.color = GetRarityColor(f.rarity);

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() => shop.BuyDailyOffer(offer));
    }

    Color GetRarityColor(Fighter.Rarity rarity)
    {
        return rarity switch
        {
            Fighter.Rarity.Common => Color.grey,
            Fighter.Rarity.Rare => Color.cyan,
            Fighter.Rarity.Epic => new Color(0.6f, 0.2f, 1f),  // purple
            Fighter.Rarity.Legendary => new Color(1f, 0.6f, 0.1f), // orange
            _ => Color.gray
        };
    }
}
