using System.Collections.Generic;
using UnityEngine;

public class FighterUsageTracker : MonoBehaviour
{
    public static FighterUsageTracker Instance;

    private Dictionary<FighterUI, bool> usedFighters = new Dictionary<FighterUI, bool>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void MarkUsed(FighterUI fighter)
    {
        if (fighter == null) return;

        // Make sure we don't have duplicates
        if (!usedFighters.ContainsKey(fighter))
        {
            usedFighters.Add(fighter, true);
        }
        else
        {
            usedFighters[fighter] = true;
        }

        fighter.UpdateVisualState();
    }

    public void UnmarkUsed(FighterUI fighter)
    {
        if (fighter == null) return;

        if (usedFighters.ContainsKey(fighter))
        {
            usedFighters[fighter] = false;
        }

        fighter.UpdateVisualState();
    }

    public bool IsUsed(FighterUI fighter)
    {
        if (fighter == null) return false;

        return usedFighters.TryGetValue(fighter, out bool used) && used;
    }

    // Rebuild the usage state based on currently occupied slots
    public void RebuildUsageFromSlots()
    {
        // Clear current state
        usedFighters.Clear();

        // Find all fighter slots in the scene
        FighterSlot[] slots = FindObjectsOfType<FighterSlot>();

        foreach (var slot in slots)
        {
            // If the slot has a fighter assigned
            if (slot.slotIndex < BattleDataManager.Instance.selectedFighters.Count)
            {
                Fighter slotFighter = BattleDataManager.Instance.selectedFighters[slot.slotIndex];

                if (slotFighter != null)
                {
                    // Find the matching FighterUI
                    foreach (var ui in FindObjectsOfType<FighterUI>())
                    {
                        if (ui.fighterData == slotFighter)
                        {
                            // Mark this fighter as used
                            MarkUsed(ui);
                            break;
                        }
                    }
                }
            }
        }

        // Log for debugging
        Debug.Log($"Rebuilt usage tracker with {usedFighters.Count} fighters");
    }
}