using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvailableFightersPanel : MonoBehaviour
{
    public static AvailableFightersPanel Instance;

    public GameObject fighterUIPrefab;
    public Transform contentHolder;
    public List<Fighter> unlockedFighters;

    public List<GameObject> currentFighterUIs = new List<GameObject>();

    // Flag to track if initialization is complete
    private bool isInitialized = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(InitializeWithDelay());
    }

    // Use a coroutine to ensure proper initialization sequence
    private IEnumerator InitializeWithDelay()
    {
        unlockedFighters.Clear();

        // Make sure BattleDataManager is initialized
        yield return new WaitUntil(() => BattleDataManager.Instance != null);

        // Load unlocked fighters
        BattleDataManager.Instance.LoadUnlockedFighters(BattleDataManager.Instance.allAvailableFighters);

        // Populate the fighter UI elements
        PopulateFighters();

        // Short delay to ensure UI elements are created
        yield return new WaitForSeconds(0.2f);

        // Now load saved data into fighter slots
        foreach (var slot in FindObjectsOfType<FighterSlot>())
        {
            slot.LoadFromSavedData();
        }

        // Additional delay to ensure slots are fully loaded
        yield return new WaitForSeconds(0.3f);

        // Final refresh of all fighter states
        RefreshAllFighterStates();

        // Mark initialization as complete
        isInitialized = true;
    }

    public void PopulateFighters()
    {
        // Clear existing UI elements
        foreach (Transform child in contentHolder)
            Destroy(child.gameObject);
        currentFighterUIs.Clear();

        // Create UI elements for each unlocked fighter
        foreach (var fighter in unlockedFighters)
        {
            if (fighter != null)
            {
                GameObject go = Instantiate(fighterUIPrefab, contentHolder);
                FighterUI ui = go.GetComponent<FighterUI>();
                ui.Setup(fighter);
                currentFighterUIs.Add(go);
            }
        }

        // Log for debugging
        Debug.Log($"Populated {currentFighterUIs.Count} fighters in UI panel");
    }

    public void AddFighterToList(Fighter fighter)
    {
        if (fighter == null) return;

        // Add to unlocked list if not already present
        if (!unlockedFighters.Contains(fighter))
        {
            unlockedFighters.Add(fighter);
        }

        // Check if fighter UI already exists
        bool uiExists = false;
        foreach (var ui in currentFighterUIs)
        {
            if (ui.GetComponent<FighterUI>().fighterData == fighter)
            {
                uiExists = true;
                break;
            }
        }

        // Create UI if doesn't exist
        if (!uiExists)
        {
            GameObject go = Instantiate(fighterUIPrefab, contentHolder);
            FighterUI ui = go.GetComponent<FighterUI>();
            ui.Setup(fighter);
            currentFighterUIs.Add(go);
        }

        // Save unlocked fighters
        BattleDataManager.Instance.SaveUnlockedFighters();
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

        // Also remove from unlocked fighters list
        unlockedFighters.Remove(fighter);

        // Save updated unlocked fighters
        BattleDataManager.Instance.SaveUnlockedFighters();
    }

    public void ReturnFighter(Fighter fighter)
    {
        AddFighterToList(fighter);
    }

    void OnEnable()
    {
        // Only rebuild UI if we're past initialization
        if (isInitialized)
        {
            // Rebuild UI elements
            PopulateFightersUIOnly();

            // Update the visual state of all fighters
            RefreshAllFighterStates();
        }
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

    // Method to refresh the visual state of all fighters
    public void RefreshAllFighterStates()
    {
        // First, rebuild the FighterUsageTracker with current fighter slots
        if (FighterUsageTracker.Instance != null)
        {
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
}