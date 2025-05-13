using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class FighterSlot : MonoBehaviour, IDropHandler
{
    public int slotIndex;
    public Image icon;
    public Sprite defaultIcon; // Assign in Inspector

    public TextMeshProUGUI nameText;

    private FighterUI assignedUI;

    public void OnDrop(PointerEventData eventData)
    {
        FighterUI draggedUI = eventData.pointerDrag.GetComponent<FighterUI>();
        if (draggedUI == null || FighterUsageTracker.Instance.IsUsed(draggedUI)) return;

        // Unmark old
        if (assignedUI != null)
        {
            FighterUsageTracker.Instance.UnmarkUsed(assignedUI);
        }

        assignedUI = draggedUI;
        FighterUsageTracker.Instance.MarkUsed(assignedUI);

        icon.sprite = draggedUI.fighterData.icon;
        icon.enabled = true;
        nameText.text = draggedUI.fighterData.fighterName;

        if (BattleDataManager.Instance.selectedFighters.Count <= slotIndex)
        {
            while (BattleDataManager.Instance.selectedFighters.Count <= slotIndex)
                BattleDataManager.Instance.selectedFighters.Add(null);
        }

        BattleDataManager.Instance.selectedFighters[slotIndex] = assignedUI.fighterData;

        BattleDataManager.Instance.SaveProgress();

        RefreshAllUIs();
    }

    public void ClearSlot()
    {
        if (assignedUI != null)
        {
            FighterUsageTracker.Instance.UnmarkUsed(assignedUI);
            assignedUI = null;
        }

        icon.sprite = defaultIcon;
        icon.enabled = true;
        nameText.text = "Empty";

        BattleDataManager.Instance.selectedFighters[slotIndex] = null;

        BattleDataManager.Instance.SaveProgress();

        RefreshAllUIs();
    }

    private void RefreshAllUIs()
    {
        foreach (var ui in FindObjectsOfType<FighterUI>())
        {
            ui.UpdateVisualState();
        }
    }
    public void LoadFromSavedData()
    {
        if (slotIndex >= BattleDataManager.Instance.selectedFighters.Count)
            return;

        Fighter fighter = BattleDataManager.Instance.selectedFighters[slotIndex];
        if (fighter == null)
        {
            ClearSlot();
            return;
        }

        // Find matching FighterUI in the AvailableFightersPanel
        foreach (var go in AvailableFightersPanel.Instance.currentFighterUIs)
        {
            FighterUI ui = go.GetComponent<FighterUI>();
            if (ui != null && ui.fighterData == fighter)
            {
                assignedUI = ui;
                icon.sprite = fighter.icon;
                icon.enabled = true;
                nameText.text = fighter.fighterName;

                FighterUsageTracker.Instance.MarkUsed(ui);
                ui.UpdateVisualState();

                return;
            }
        }

        // If we didn't find a matching UI (perhaps not yet loaded), just show basic info
        icon.sprite = fighter.icon;
        icon.enabled = true;
        nameText.text = fighter.fighterName;
    }

}

