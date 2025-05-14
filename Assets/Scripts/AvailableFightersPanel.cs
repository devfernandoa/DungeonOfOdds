using System.Collections.Generic;
using UnityEngine;

public class AvailableFightersPanel : MonoBehaviour
{
    public static AvailableFightersPanel Instance;

    public GameObject fighterUIPrefab;
    public Transform contentHolder;
    public List<Fighter> unlockedFighters;

    public List<GameObject> currentFighterUIs = new List<GameObject>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        unlockedFighters.Clear();

        BattleDataManager.Instance.LoadUnlockedFighters(BattleDataManager.Instance.allAvailableFighters);

        PopulateFighters();

        foreach (var slot in FindObjectsOfType<FighterSlot>())
        {
            slot.LoadFromSavedData();
        }

        // ✅ Fix: Make sure to refresh fighter states after loading all slots
        Invoke("RefreshAllFighterStates", 0.1f); // Small delay to ensure all slots are loaded
    }

    public void PopulateFighters()
    {
        foreach (Transform child in contentHolder)
            Destroy(child.gameObject);
        currentFighterUIs.Clear();

        foreach (var fighter in unlockedFighters)
        {
            // ✅ Only rebuild the UI, don't touch the list!
            if (fighter != null)
            {
                GameObject go = Instantiate(fighterUIPrefab, contentHolder);
                FighterUI ui = go.GetComponent<FighterUI>();
                ui.Setup(fighter);
                currentFighterUIs.Add(go);
            }
        }
    }

    public void AddFighterToList(Fighter fighter)
    {
        if (fighter == null) return;

        // ✅ Prevent duplicate entries in unlocked list
        unlockedFighters.Add(fighter);

        // ✅ Prevent duplicate visual cards
        if (currentFighterUIs.Exists(ui => ui.GetComponent<FighterUI>().fighterData == fighter))
            return;

        GameObject go = Instantiate(fighterUIPrefab, contentHolder);
        FighterUI ui = go.GetComponent<FighterUI>();
        ui.Setup(fighter);
        currentFighterUIs.Add(go);
    }

    public void RemoveFighterFromList(Fighter fighter)
    {
        for (int i = 0; i < currentFighterUIs.Count; i++)
        {
            FighterUI ui = currentFighterUIs[i].GetComponent<FighterUI>();
            if (ui != null && ui.fighterData == fighter)
            {
                Destroy(currentFighterUIs[i]);
                currentFighterUIs.RemoveAt(i);
                break; // remove only one
            }
        }
    }

    public void ReturnFighter(Fighter fighter)
    {
        AddFighterToList(fighter);
    }

    void OnEnable()
    {
        // ✅ Always fully rebuild the UI
        PopulateFightersUIOnly();

        // ✅ Fix: Update the visual state of all fighters to reflect usage
        RefreshAllFighterStates();
    }

    public void PopulateFightersUIOnly()
    {
        foreach (Transform child in contentHolder)
            Destroy(child.gameObject);
        currentFighterUIs.Clear();

        foreach (var fighter in unlockedFighters)
        {
            GameObject go = Instantiate(fighterUIPrefab, contentHolder);
            FighterUI ui = go.GetComponent<FighterUI>();
            ui.Setup(fighter);
            currentFighterUIs.Add(go);
        }
    }

    // ✅ New method to refresh the visual state of all fighters
    public void RefreshAllFighterStates()
    {
        // First, rebuild the FighterUsageTracker with current fighter slots
        FighterUsageTracker.Instance.RebuildUsageFromSlots();

        // Then update the visual state of all fighter UIs
        foreach (var go in currentFighterUIs)
        {
            FighterUI ui = go.GetComponent<FighterUI>();
            if (ui != null)
            {
                ui.UpdateVisualState();
            }
        }
    }
}