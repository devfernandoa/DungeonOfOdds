using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class FighterUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Fighter fighterData;
    public Image icon;
    public TextMeshProUGUI nameText;
    public CanvasGroup canvasGroup;

    private Transform originalParent;
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Setup(Fighter data)
    {
        fighterData = data;
        icon.sprite = data.icon;
        nameText.text = data.fighterName;
        UpdateVisualState();
    }

    public void UpdateVisualState()
    {
        bool used = FighterUsageTracker.Instance.IsUsed(this);
        icon.color = used ? new Color(1, 1, 1, 0.4f) : Color.white;
        canvasGroup.blocksRaycasts = !used;

        // Change text color based on rarity
        switch (fighterData.rarity)
        {
            case Fighter.Rarity.Common:
                nameText.color = Color.gray;
                break;
            case Fighter.Rarity.Rare:
                nameText.color = new Color(0.1f, 0.5f, 1f); // blue
                break;
            case Fighter.Rarity.Epic:
                nameText.color = new Color(0.6f, 0.2f, 1f); // purple
                break;
            case Fighter.Rarity.Legendary:
                nameText.color = new Color(1f, 0.6f, 0.1f); // orange
                break;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (FighterUsageTracker.Instance.IsUsed(this)) return;

        originalParent = transform.parent;
        transform.SetParent(transform.root);
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.7f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!canvasGroup.blocksRaycasts) transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        if (transform.parent == transform.root)
        {
            // Reset to original position
            transform.SetParent(originalParent);
            rectTransform.localPosition = Vector3.zero;
        }
    }
}
