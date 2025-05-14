using UnityEngine;

public class TabController : MonoBehaviour
{
    public GameObject fightPanel;
    public GameObject teamPanel;
    public GameObject shopPanel;
    public GameObject gamblePanel;
    public GameObject victoryPanel;
    public GameObject tempPanel;

    // Add a flag to prevent multiple battle starts
    private bool battleStarted = false;

    // Add a reference to the BattleController
    private BattleController battleController;

    void Start()
    {
        // Get battle controller reference
        battleController = FindObjectOfType<BattleController>();

        // First load and initialize the team panel to ensure all fighter data is ready
        teamPanel.SetActive(true);
        tempPanel.SetActive(true);

        // Make sure all fighter data has been loaded before switching to fight tab
        StartCoroutine(InitializeGameSequence());
    }

    System.Collections.IEnumerator InitializeGameSequence()
    {
        // Give time for the UI to initialize and fighters to be loaded
        yield return new WaitForSeconds(0.5f);
        tempPanel.SetActive(false);

        // Now switch to the fight tab
        ShowFightTab();
    }

    public void ShowFightTab()
    {
        fightPanel.SetActive(true);
        teamPanel.SetActive(false);
        shopPanel.SetActive(false);
        gamblePanel.SetActive(false);
        victoryPanel.SetActive(false);

        // Wait a moment before starting the battle to ensure UI is ready
        if (!battleStarted)
        {
            Invoke("StartBattle", 0.2f);
        }
    }

    public void ShowTeamTab()
    {
        fightPanel.SetActive(false);
        teamPanel.SetActive(true);
        shopPanel.SetActive(false);
        gamblePanel.SetActive(false);
        victoryPanel.SetActive(false);

        // Ensure fighter states are refreshed when showing team tab
        if (AvailableFightersPanel.Instance != null)
        {
            AvailableFightersPanel.Instance.RefreshAllFighterStates();
        }
    }

    public void ShowShopTab()
    {
        fightPanel.SetActive(false);
        teamPanel.SetActive(false);
        shopPanel.SetActive(true);
        gamblePanel.SetActive(false);
        victoryPanel.SetActive(false);
    }

    public void ShowGambleTab()
    {
        fightPanel.SetActive(false);
        teamPanel.SetActive(false);
        shopPanel.SetActive(false);
        gamblePanel.SetActive(true);
        victoryPanel.SetActive(false);
    }

    private void StartBattle()
    {
        if (battleController != null)
        {
            battleController.StartBattle();
            battleStarted = true;
        }
        else
        {
            Debug.LogError("BattleController not found!");
        }
    }
}