using System.Collections.Generic;
using UnityEngine;

public class BattleDataManager : MonoBehaviour
{
    public static BattleDataManager Instance;

    public List<Fighter> selectedFighters = new List<Fighter>();
    public List<Fighter> allAvailableFighters = new List<Fighter>();

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


}
