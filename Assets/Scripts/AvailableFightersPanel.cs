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
        PopulateFighters();

        foreach (var slot in FindObjectsOfType<FighterSlot>())
        {
            slot.LoadFromSavedData();
        }
    }

    public void PopulateFighters()
    {
        foreach (Transform child in contentHolder)
            Destroy(child.gameObject);

        currentFighterUIs.Clear();

        foreach (var fighter in unlockedFighters)
        {
            AddFighterToList(fighter);
        }
    }

    public void AddFighterToList(Fighter fighter)
    {
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
}
