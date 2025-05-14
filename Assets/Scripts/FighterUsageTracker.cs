using System.Collections.Generic;
using UnityEngine;

public class FighterUsageTracker : MonoBehaviour
{
    public static FighterUsageTracker Instance;

    private void Awake()
    {
        Instance = this;
    }

    private HashSet<FighterUI> usedUIs = new HashSet<FighterUI>();

    public void MarkUsed(FighterUI ui)
    {
        usedUIs.Add(ui);
    }

    public void UnmarkUsed(FighterUI ui)
    {
        usedUIs.Remove(ui);
    }

    public bool IsUsed(FighterUI ui)
    {
        return usedUIs.Contains(ui);
    }

    public void RebuildUsageFromSlots()
    {
        // Clear the current usage tracking
        usedUIs.Clear();

        // Find all fighter slots in the scene
        FighterSlot[] slots = FindObjectsOfType<FighterSlot>();

        // Create a dictionary to track how many instances of each fighter are used
        Dictionary<Fighter, int> usedFighterCounts = new Dictionary<Fighter, int>();

        // Count used fighters
        foreach (var slot in slots)
        {
            if (slot.slotIndex < BattleDataManager.Instance.selectedFighters.Count &&
                BattleDataManager.Instance.selectedFighters[slot.slotIndex] != null)
            {
                Fighter fighter = BattleDataManager.Instance.selectedFighters[slot.slotIndex];
                if (!usedFighterCounts.ContainsKey(fighter))
                {
                    usedFighterCounts[fighter] = 0;
                }
                usedFighterCounts[fighter]++;
            }
        }

        // Now mark the correct number of UIs for each fighter
        foreach (var fighterEntry in usedFighterCounts)
        {
            Fighter fighter = fighterEntry.Key;
            int countNeeded = fighterEntry.Value;
            int countMarked = 0;

            // Debug info
            Debug.Log($"Fighter {fighter.fighterName} needs {countNeeded} instances marked as used");

            // Loop through all available fighter UIs
            foreach (var go in AvailableFightersPanel.Instance.currentFighterUIs)
            {
                FighterUI ui = go.GetComponent<FighterUI>();
                if (ui == null) continue;

                // If this UI represents the fighter we're looking for and we need more
                if (ui.fighterData == fighter && countMarked < countNeeded)
                {
                    MarkUsed(ui);
                    countMarked++;
                    Debug.Log($"Marked Fighter {fighter.fighterName} instance {countMarked} as used");
                }
            }
        }
    }
}