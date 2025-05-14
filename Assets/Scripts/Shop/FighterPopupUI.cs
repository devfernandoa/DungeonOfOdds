using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FighterPopupUI : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI rarityText;
    public Button closeButton;

    public static FighterPopupUI Instance;
    public GameObject panelRoot; // assign this in Inspector (the panel part)
    private void Awake()
    {
        Instance = this;
        panelRoot.SetActive(false); // just hide the content
        closeButton.onClick.AddListener(() => panelRoot.SetActive(false));
    }

    public void Show(Fighter fighter)
    {
        icon.sprite = fighter.icon;
        nameText.text = fighter.fighterName;
        rarityText.text = fighter.rarity.ToString();
        rarityText.color = GetRarityColor(fighter.rarity);

        panelRoot.SetActive(true); // now show the popup
    }

    private Color GetRarityColor(Fighter.Rarity rarity)
    {
        return rarity switch
        {
            Fighter.Rarity.Common => Color.white,
            Fighter.Rarity.Rare => Color.cyan,
            Fighter.Rarity.Epic => new Color(0.6f, 0.2f, 1f),
            Fighter.Rarity.Legendary => new Color(1f, 0.6f, 0.1f),
            _ => Color.white
        };
    }
}
