using System.Collections.Generic;
using UnityEngine;

public class BattleDataManager : MonoBehaviour
{
    public static BattleDataManager Instance;

    public List<Fighter> selectedFighters = new List<Fighter>();
    public List<Fighter> allAvailableFighters = new List<Fighter>();
    public List<Fighter> unlockedFighters = new();

    private const string UnlockedFightersKey = "UnlockedFighters";

    public int currentFloor = 1;

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

    private void Start()
    {
        LoadProgress();
    }

    public bool IsBossFloor(int floor)
    {
        return floor % 10 == 0;
    }

    public int GetLastBossCheckpoint(int floor)
    {
        return ((floor - 1) / 10) * 10 + 1;
    }

    public void SaveProgress()
    {
        PlayerPrefs.SetInt("CurrentFloor", currentFloor);

        // Save fighter names
        for (int i = 0; i < selectedFighters.Count; i++)
        {
            string key = $"SelectedFighter_{i}";
            string fighterName = selectedFighters[i] != null ? selectedFighters[i].fighterName : "";
            PlayerPrefs.SetString(key, fighterName);
        }

        PlayerPrefs.Save();
    }
    public void LoadProgress()
    {
        currentFloor = PlayerPrefs.GetInt("CurrentFloor", 1);

        selectedFighters.Clear();

        for (int i = 0; i < 3; i++)
        {
            string key = $"SelectedFighter_{i}";
            string name = PlayerPrefs.GetString(key, "");

            if (!string.IsNullOrEmpty(name))
            {
                Fighter f = FindFighterByName(name);
                selectedFighters.Add(f);
            }
            else
            {
                selectedFighters.Add(null);
            }
        }
    }

    private Fighter FindFighterByName(string name)
    {
        return allAvailableFighters.Find(f => f.fighterName == name);
    }
    public void UnlockFighter(Fighter f)
    {
        if (!unlockedFighters.Contains(f))
            unlockedFighters.Add(f);
    }

    public void SaveUnlockedFighters()
    {
        var panel = AvailableFightersPanel.Instance;
        if (panel == null)
        {
            Debug.LogError("AvailableFightersPanel.Instance is null when saving.");
            return;
        }

        List<string> fighterNames = new();

        foreach (var f in panel.unlockedFighters)
        {
            fighterNames.Add(f.fighterName);
        }

        string json = JsonUtility.ToJson(new StringListWrapper { values = fighterNames });
        PlayerPrefs.SetString("UnlockedFighters", json);
        PlayerPrefs.Save();

        Debug.Log("Saved fighters: " + string.Join(", ", fighterNames));
    }

    public void LoadUnlockedFighters(List<Fighter> allFighters)
    {
        if (!PlayerPrefs.HasKey("UnlockedFighters")) return;

        string json = PlayerPrefs.GetString("UnlockedFighters");
        StringListWrapper wrapper = JsonUtility.FromJson<StringListWrapper>(json);

        var panel = AvailableFightersPanel.Instance;

        foreach (string name in wrapper.values)
        {
            Fighter found = allFighters.Find(f => f.fighterName == name);
            if (found != null)
            {
                panel.unlockedFighters.Add(found); // ✅ Only update list
            }
        }
    }


    [System.Serializable]
    private class StringListWrapper
    {
        public List<string> values;
    }

    [ContextMenu("Clear Saved Fighters")]
    public void ClearSavedFighters()
    {
        PlayerPrefs.DeleteKey("UnlockedFighters");
        PlayerPrefs.Save();
        Debug.Log("✅ Cleared saved fighters from PlayerPrefs.");
    }

}
