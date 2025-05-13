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
}
